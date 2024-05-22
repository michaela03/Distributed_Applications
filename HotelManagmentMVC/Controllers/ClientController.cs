using Microsoft.AspNetCore.Mvc;

namespace HotelManagmentMVC.Controllers
{
    public class ClientController : Controller
    {
        Uri uri = new Uri("https://localhost:44364/api/Client/GetClients");

        private readonly HttpClient _httpClient;

        public ClientController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = uri;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
