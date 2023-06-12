using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HomeLightControl.Controllers;

public class WeatherController : Controller
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private JObject keys;
    public WeatherController(ILogger<WeatherController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        //read the API key from keys.json and store it in a variable
        keys = JObject.Parse(System.IO.File.ReadAllText("App_Data/keys.json"));
        
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.openweathermap.org/data/2.5/weather?lat=51.13&lon=71.46&appid=" + keys["weather"]);
        var client = _clientFactory.CreateClient();
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseString);
            return View(json);
        }
        else
        {
            _logger.LogError("Error: {StatusCode}", response.StatusCode);
            return View();
        }
        return View();
    }
}