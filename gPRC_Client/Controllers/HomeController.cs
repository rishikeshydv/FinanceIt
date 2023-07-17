using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using gPRC_Client.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json;
using System.Text;

namespace gPRC_Client.Controllers;

public class HomeController : Controller
{
    record UserDto(string UserName, string Password);

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
     
    }

    public async Task<IActionResult> IndexSync()
    {

        using (var httpClient = new HttpClient())
        {
            var user = new UserDto(UserName: "admin@admin.com", Password: "admin");
            var json = JsonConvert.SerializeObject(user);

            var content = new StringContent(json, Encoding.UTF8, "application/json");


            using (var response = await httpClient.PostAsync("https://localhost:7096/auth/getToken", content))
            {
                var token = await response.Content.ReadAsStringAsync();
                token = token.Substring(1, token.Length - 2);

                var headers = new Metadata();
                headers.Add("Authorization", $"Bearer {token}");

                string GrpcChannelURL = "https://localhost:7006";

                using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                var message = new MessageProviderClient(channel);
                var products = message.PrintMessage(new Empty { }, headers);
            }
        }
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

