using MichaelsWebApplication.Models;
using MichaelsWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MichaelsWebApplication.Controllers {
    public class HomeController : Controller {
        private readonly ProductService _productService;

        public HomeController (ProductService productService){
            _productService = productService;
        }

        public async Task<IActionResult> Index (){
            var products = await _productService.GetAllAsync();
            return View(products);
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
