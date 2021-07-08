﻿using AVC.Dtos.HubMessages;
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
        private readonly string accountGroup = "Account";
        private readonly string carGroup = "Car";
        private static Dictionary<string, int> CarDic;


        public AVCHub(ICarService carService)
        {
            _carService = carService;
            if (CarDic == null)
            {
                CarDic = new Dictionary<string, int>();
            }
        }

        public async Task ConnectCar(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, carGroup);
            Log.Information(deviceId + " is connected with ClientID: " + Context.ConnectionId);

            var message = _carService.HandleCarConnected(deviceId);

            if (!CarDic.ContainsKey(Context.ConnectionId))
                CarDic.Add(Context.ConnectionId, message.carConnectedMessage.CarId);

            if (message != null)
            {
                await SendCarConnectedToStaff(message.carConnectedMessage);
                await Clients.Caller.SendAsync("WhenCarConnectedReturn", message.carMessageDto);
            }

        }

        public async Task SendCarConnectedToStaff(CarConnectedMessage message)
        {
            await Clients.Group(accountGroup).SendAsync("WhenCarConnected", message);
        }

        public async Task StartCar(int carId)
        {
            string deviceId = _carService.GetDeviceIdByCarId(carId);

            if (deviceId != null)
            {
                await Clients.Group(carGroup).SendAsync("WhenCarStart", deviceId);
            }
            else
            {
                //send start Car Fail notification
                Log.Error("Car not found to start.");
            }
        }

        public async Task RunningCar(string deviceId)
        {
            var message = _carService.HandleWhenCarRunning(deviceId);

            if (message != null)
            {
                await Clients.Group(accountGroup).SendAsync("WhenCarRunning", message);
            }
            else
            {
                Log.Error("Car not found when Running Car SignalR");
            }
        }

        public async Task StopCar(int carId)
        {
            string deviceId = _carService.GetDeviceIdByCarId(carId);

            if (deviceId != null)
            {
                await Clients.Group(carGroup).SendAsync("WhenCarStop", deviceId);
            }
            else
            {
                //send start Car Fail notification
                Log.Error("Car not found to stop.");
            }
        }

        public async Task StoppingCar(string deviceId)
        {
            var message = _carService.HandleWhenCarStopping(deviceId);

            if (message != null)
            {
                await Clients.Group(accountGroup).SendAsync("WhenCarStopping", message);
            }
            else
            {
                Log.Error("Car not found when Stopping Car SignalR");
            }
        }

        public async Task ConnectAccount(int accountId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, accountGroup);
            Log.Information(accountId + " is connected with ClientID: " + Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information(Context.ConnectionId + " is disconnected");

            if (CarDic.ContainsKey(Context.ConnectionId))
            {
                var message = _carService.HandleWhenCarDisconnected(CarDic[Context.ConnectionId]);
                Clients.Group(accountGroup).SendAsync("WhenCarDisconnected", message);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public override Task OnConnectedAsync()
        {
            System.Console.WriteLine(Context.ConnectionId + " is connected");
            return base.OnConnectedAsync();

        }

    }
}