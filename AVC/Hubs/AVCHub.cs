using AVC.Constant;
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
            if (HubConstant.carDic == null)
            {
                HubConstant.carDic = new Dictionary<string, int>();
            }
        }

        public async Task ConnectCar(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, HubConstant.carGroup);
            Log.Information(deviceId + " is connected with ClientID: " + Context.ConnectionId);

            var message = _carService.HandleCarConnected(deviceId);

            if (message.carConnectedMessage != null)
            {
                if (!HubConstant.carDic.ContainsKey(Context.ConnectionId))
                    HubConstant.carDic.Add(Context.ConnectionId, message.carConnectedMessage.CarId);
                await SendCarConnectedToStaff(message.carConnectedMessage);
            }

            await Clients.Caller.SendAsync("WhenCarConnectedReturn", message.carMessageDto);

        }

        public async Task SendCarConnectedToStaff(CarConnectedMessage message)
        {
            await Clients.Group(HubConstant.accountGroup).SendAsync("WhenCarConnected", message);
        }

        public async Task StartCar(int carId)
        {
            Log.Information("startcar(" + carId + ")");
            var car = _carService.GetCarModel(carId);

            if (car != null)
            {
                string deviceId = car.DeviceId;

                if (car.IsRunning)
                {
                    Log.Error("Car Is Running");
                    throw new HubException("Car is Running");

                }
                else if (!car.IsConnecting)
                {
                    Log.Error("Car is not connected");
                    throw new HubException("Car is not connected");
                }

                await Clients.Group(HubConstant.carGroup).SendAsync("WhenCarStart", deviceId);
            }
            else
            {
                //send start Car Fail notification
                Log.Error("Car not found to start.");
                throw new HubException("Car not found");
            }

        }

        public async Task RunningCar(string deviceId)
        {
            Log.Information("RunningCar(" + deviceId + ")");

            var message = _carService.HandleWhenCarRunning(deviceId);

            if (message != null)
            {
                await Clients.Group(HubConstant.accountGroup).SendAsync("WhenCarRunning", message);
            }
            else
            {
                Log.Error("Car not found when Running Car SignalR");
            }
        }

        public async Task StopCar(int carId)
        {
            Log.Information("StopCar(" + carId + ")");

            var car = _carService.GetCarModel(carId);
            if (car != null)
            {
                if (car.IsRunning)
                {
                    string deviceId = car.DeviceId;

                    await Clients.Group(HubConstant.carGroup).SendAsync("WhenCarStop", deviceId);

                }
                else if (car.IsConnecting)
                {
                    throw new HubException("Car is Stopping");
                }
                else
                {
                    throw new HubException("Car is not connected");
                }
            }
            else
            {
                Log.Error("Car not found to stop.");
                throw new HubException("Car Not Found");
                //send start Car Fail notification
            }
        }

        public async Task StoppingCar(string deviceId)
        {
            Log.Information("StoppingCar(" + deviceId + ")");

            var message = _carService.HandleWhenCarStopping(deviceId);

            if (message != null)
            {
                await Clients.Group(HubConstant.accountGroup).SendAsync("WhenCarStopping", message);
            }
            else
            {
                Log.Error("Car not found when Stopping Car SignalR");
            }
        }

        public async Task ConnectAccount(int accountId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, HubConstant.accountGroup);
            Log.Information(accountId + " is connected with ClientID: " + Context.ConnectionId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information(Context.ConnectionId + " is disconnected");

            if (HubConstant.carDic.ContainsKey(Context.ConnectionId))
            {
                var message = _carService.HandleWhenCarDisconnected(HubConstant.carDic[Context.ConnectionId]);
                Clients.Group(HubConstant.accountGroup).SendAsync("WhenCarDisconnected", message);
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