using McKIESales.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MichaelsWebApplication.Controllers {
    public class ProductsController: Controller {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductsController(IHttpClientFactory clientFactory){
            _httpClientFactory = clientFactory;
        }
        
        public async Task<IActionResult> Index (){
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("api/v1/products");

            if (!response.IsSuccessStatusCode){
                return View("Error");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var products = await JsonSerializer.DeserializeAsync<List<Product>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return View(products);
        }
    }
}