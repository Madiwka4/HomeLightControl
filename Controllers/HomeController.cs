using System.Diagnostics;
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
    private void UpdateLampData()
    {
        Task<LampData> lightOne = YeelightService.GetLamp(1);
        Task<LampData> lightTwo = YeelightService.GetLamp(2);
        ViewData["DEV1"] = lightOne.Result.IsOn;
        ViewData["DEV2"] = lightTwo.Result.IsOn;
        ViewData["DEV1BR"] = lightOne.Result.Brightness;
        ViewData["DEV2BR"] = lightTwo.Result.Brightness;
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
        UpdateLampData();
        return View();
    }

    //turn on the light
    public IActionResult Toggle()
    {
        YeelightService.ToggleLamps();
        _logger.LogInformation("Turned on the light");
        UpdateLampData();
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}