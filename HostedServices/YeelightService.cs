using HomeLightControl.CustomClasses;
using YeelightAPI;
using YeelightAPI.Models;

namespace HomeLightControl.HostedServices;

public class YeelightService : IHostedService
{
    private readonly ILogger<YeelightService> _logger;
    private static DeviceGroup _lights;
    private static Device _lightOne;
    private static Device _lightTwo;
    
    public YeelightService(ILogger<YeelightService> logger)
    {
        _logger = logger;
        _lights = new DeviceGroup();
        _lightOne = new Device("192.168.1.65");
        _lightTwo = new Device("192.168.1.78");
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lights.Add(_lightOne);
        _lights.Add(_lightTwo);
        _lights.Connect();
        return Task.CompletedTask;
    }

    private static async Task ReconnectToLamps()
    {
        await _lights.Connect();
    }
    
    public static async Task<LampData> GetLamp(int num)
    {
        LampData lampData = new LampData();
        if (num == 1)
        {
            if (_lightOne.IsConnected)
            {
                //check if lights are still connected
                var power = _lightOne.GetProp(PROPERTIES.power);
                var brightness = _lightOne.GetProp(PROPERTIES.bright);
                lampData.IsOn = power.Result.ToString() != "off";
                lampData.Brightness = Convert.ToInt32(brightness.Result);  
            }
            else
            {
                await ReconnectToLamps();
                lampData.IsOn = false;
                lampData.Brightness = -1;
            }
        }
        else
        {
            if (_lightTwo.IsConnected)
            {
                //check if lights are still connected
                var power = _lightTwo.GetProp(PROPERTIES.power);
                var brightness = _lightTwo.GetProp(PROPERTIES.bright);
                lampData.IsOn = power.Result.ToString() != "off";
                lampData.Brightness = Convert.ToInt32(brightness.Result);  
            }
            else
            {
                await ReconnectToLamps();
                lampData.IsOn = false;
                lampData.Brightness = -1;
            }
        }
        return lampData;
    }

    public static async Task ToggleLamps()
    {
        if (_lightOne.IsConnected && _lightTwo.IsConnected)
        {
            await _lights.SetBrightness(100);
            await _lights.Toggle();
        }
        else
        {
            await ReconnectToLamps();
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _lights.Disconnect();
        return Task.CompletedTask;
    }
}