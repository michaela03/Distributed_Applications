using HotelManagmentMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HotelManagmentMVC.Controllers
{
   
    public class ClientController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44341/api");
        private readonly HttpClient _client;
        

        public ClientController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ClientViewModel> clientList = new List<ClientViewModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Client/GetClients").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                clientList = JsonConvert.DeserializeObject<List<ClientViewModel>>(data);
            }

            return View(clientList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClientViewModel client)
        {
            string data = JsonConvert.SerializeObject(client);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/Client/CreateClient", content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(client);
        }
    }
}

