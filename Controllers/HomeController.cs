using System.Diagnostics;
using System.Drawing;
using HomeLightControl.CustomClasses;
using HomeLightControl.HostedServices;
using Microsoft.AspNetCore.Mvc;
using HomeLightControl.Models;
using YeelightAPI;
using YeelightAPI.Models;

namespace HomeLightControl.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    private string ConvertToHex(Color color)
    {
        return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
    }
    public IActionResult ChangeBrightness(int brightness)
    {
        YeelightService.ChangeBrightness(brightness);
        return Json("sent");
    }

    public IActionResult ChangeColor(string color)
    {
        Color newColor = ColorTranslator.FromHtml(color);
        YeelightService.ChangeColor(newColor);
        return Json(" received");
    }
    private void UpdateLampData()
    {
        Task<LampData> lightOne = YeelightService.GetLamp(1);
        Task<LampData> lightTwo = YeelightService.GetLamp(2);
        ViewData["DEV1"] = lightOne.Result.IsOn;
        ViewData["DEV2"] = lightTwo.Result.IsOn;
        ViewData["DEV1BR"] = lightOne.Result.Brightness;
        ViewData["DEV2BR"] = lightTwo.Result.Brightness;
        ViewData["DEV1RGB"] = lightOne.Result.Color;
        ViewData["DEV2RGB"] = lightTwo.Result.Color;
    }
    //GET request for lamp data
    public IActionResult GetLampData()
    {
        try
        {
            UpdateLampData();   
        }
        catch (Exception e)
        {
            if (e.HResult == -2146233088)
            {
                _logger.LogError(e.Message);
                return Json(new { success = false });
            }
        } ;
        string readableColor = ConvertToHex((Color)ViewData["DEV1RGB"]);
        return Json(new { success = true, dev1 = ViewData["DEV1"].ToString(), dev1br = ViewData["DEV1BR"].ToString(), dev1rgb = readableColor});
    }
    public IActionResult Index()
    {
        //get status of the Yeelight lightbulb before passing it to the view
        //get whether the two lights are on or off and send it to view
        /*var power = lightOne.GetProp(PROPERTIES.power);
        var power2 = lightTwo.GetProp(PROPERTIES.power);
        bool DeviceIsOn = power.Result.ToString() != "off";
        bool DeviceIsOn2 = power2.Result.ToString() != "off";
        */
        try
        {
            UpdateLampData();   
        }
        catch (Exception e)
        {
            if (e.HResult == -2146233088)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("RateError");
            }
        }

        return View();
    }
    
    public IActionResult RateError()
    {
        return View();
    }
    //turn on the light
    public IActionResult Toggle()
    {
        YeelightService.ToggleLamps();
        _logger.LogInformation("Turned on the light");
        try
        {
            UpdateLampData();   
        }
        catch (Exception e)
        {
            if (e.HResult == -2146233088)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("RateError");
            }
        }
        return RedirectToAction("Index");
    }

    public IActionResult Reset()
    {
        YeelightService.ResetToWhite();
        _logger.LogInformation("Reset the light to white");
        try
        {
            UpdateLampData();   
        }
        catch (Exception e)
        {
            if (e.HResult == -2146233088)
            {
                _logger.LogError(e.Message);
                return RedirectToAction("RateError");
            }
        }
        return RedirectToAction("Index");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}