using AVC.Dtos.HubMessages;
using AVC.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVC.Hubs
{
    public class AVCHub : Hub
    {
        private readonly ICarService _carService;

        public AVCHub(ICarService carService)
        {
            _carService = carService;
        }

        public async Task ConnectCar(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Car");
            Log.Information(deviceId + " is connected with ClientID: " + Context.ConnectionId);
            CarConnectedMessage message = _carService.HandleCarConnected(deviceId);

            if(message.AccountIdList.Count > 0)
            {
                await SendCarConnectedToStaff(message);
            }
        }

        public async Task SendCarConnectedToStaff(CarConnectedMessage message)
        {
            await Clients.Group("Staff").SendAsync("CarConnectedNoti", message);
        }

        public async Task ConnectStaff(int staffId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Staff");
            Log.Information(Groups.ToString());
            Log.Information(staffId + " is connected with ClientID: " + Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information(Context.ConnectionId + " is disconnected");
            return base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            System.Console.WriteLine(Context.ConnectionId + " is connected");
            return base.OnConnectedAsync();

        }

    }
}