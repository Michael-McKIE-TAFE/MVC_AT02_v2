using MichaelsWebApplication.Models;

namespace MichaelsWebApplication.Services {
    public class ProductService {
        private readonly HttpClient _httpClient;

        public ProductService(IHttpClientFactory factory){
            _httpClient = factory.CreateClient("McKIESales.API");
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync (){
            var response = await _httpClient.GetAsync("products");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        }
    }
}