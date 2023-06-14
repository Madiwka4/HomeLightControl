using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HomeLightControl.Controllers;

public class ToolsController : Controller
{
    private readonly JObject _keys;
    private readonly ILogger<WeatherController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    public ToolsController(ILogger<WeatherController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        //read the API key from keys.json and store it in a variable
        _keys = JObject.Parse(System.IO.File.ReadAllText("App_Data/keys.json"));
    }
    // GET
    public IActionResult Index()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://gnews.io/api/v4/top-headlines?category=technology&max=4&lang=en&apikey=" + _keys["news"]);
        var client = _clientFactory.CreateClient();
        var response = client.Send(request);
        if (response.IsSuccessStatusCode)
        {
            var responseString = response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseString.Result);
            return View(json);
        }
        _logger.LogError("Error: {StatusCode}", response.StatusCode);
        return View();
    }
}