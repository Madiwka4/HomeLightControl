using System.Drawing;
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
                var color = _lightOne.GetProp(PROPERTIES.rgb).Result;
                var lampset = _lightOne.GetProp(PROPERTIES.color_mode).Result;
                lampData.IsOn = power.Result.ToString() != "off";
                lampData.Brightness = Convert.ToInt32(brightness.Result);
                lampData.Color = Color.FromArgb(Convert.ToInt32(color));
                if ((string)lampset == "2")
                {
                    //set color to white
                    lampData.Color = Color.White;
                }
            }
            else
            {
                await ReconnectToLamps();
            }
        }
        else
        {
            if (_lightTwo.IsConnected)
            {
                //check if lights are still connected
                var power = _lightTwo.GetProp(PROPERTIES.power);
                var brightness = _lightTwo.GetProp(PROPERTIES.bright);
                var color = _lightTwo.GetProp(PROPERTIES.rgb).Result;
                var lampset = _lightOne.GetProp(PROPERTIES.color_mode).Result;
                lampData.IsOn = power.Result.ToString() != "off";
                lampData.Brightness = Convert.ToInt32(brightness.Result);  
                lampData.Color = Color.FromArgb(Convert.ToInt32(color));
                if ((string)lampset == "2")
                {
                    //set color to white
                    lampData.Color = Color.White;
                }
            }
            else
            {
                await ReconnectToLamps();
            }
        }
        return lampData;
    }

    public static async Task ToggleLamps()
    {
        if (_lightOne.IsConnected && _lightTwo.IsConnected)
        {
            await _lights.Toggle();
        }
        else
        {
            await ReconnectToLamps();
        }
    }
    public static async Task ChangeBrightness(int brightness)
    {
        if (_lightOne.IsConnected && _lightTwo.IsConnected)
        {
            await _lights.SetBrightness(brightness, 500);
        }
        else
        {
            await ReconnectToLamps();
        }
    }

    public static async Task ResetToWhite()
    {
        if (_lightOne.IsConnected && _lightTwo.IsConnected)
        {
            await _lights.TurnOn();
            await _lights.SetBrightness(100);
            await _lights.SetColorTemperature(4000, 500);
        }
        else
        {
            await ReconnectToLamps();
        }
    }
    public static async Task ChangeColor(Color color)
    {
        if (_lightOne.IsConnected && _lightTwo.IsConnected)
        {
            await _lights.SetRGBColor(color.R, color.G, color.B, 500);
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