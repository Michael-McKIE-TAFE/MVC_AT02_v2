using McKIESales.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace McKIESales.WEB.Controllers {
    /// <summary>
    /// This class is set up to manage HTTP requests. It initializes an HttpClient that targets the base 
    /// address https://localhost:7171/. The Index action asynchronously fetches data from the api/product
    /// endpoint, and if the request succeeds, it deserializes the JSON response into a list of Product 
    /// objects and sends them to the view. If it fails, the user is redirected to an error page. The Privacy 
    /// action just returns a view, and the Error action handles error pages by providing a unique request ID.
    /// </summary>
    public class HomeController : Controller {
        private readonly HttpClient _httpClient;

        public HomeController (){
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7171/");
        }

        //  This method sends a GET request to the `api/product` endpoint.
        //  If the response is successful, it deserializes the JSON into a
        //  list of products and returns that list to the view. If the request
        //  fails, it returns an error view.
        public async Task<IActionResult> Index (){
            var response = await _httpClient.GetAsync("api/product");

            if (response.IsSuccessStatusCode){
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);
                return View(items);
            }
            return View("Error");
        }

        //  Returns the Privacy view when called
        public IActionResult Privacy (){
            return View();
        }

        //  This action is decorated with the `ResponseCache` attribute to disable caching.
        //  It returns the `Error` view, passing an `ErrorViewModel` with the current request
        //  ID or trace identifier for error tracking.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error (){
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}