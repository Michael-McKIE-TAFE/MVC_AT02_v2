using McKIESales.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace McKIESales.WEB.Controllers {
    public class HomeController : Controller {
        private readonly HttpClient _httpClient;

        public HomeController (){
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7171/");
        }

        public async Task<IActionResult> Index (){
            var response = await _httpClient.GetAsync("api/product");

            if (response.IsSuccessStatusCode){
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);
                return View(items);
            }
            return View("Error");
        }

        public IActionResult Privacy (){
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error (){
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
