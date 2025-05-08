using Asp.Versioning;
using McKIESales.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace McKIESales.API.Controllers {
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase {
        private readonly ShopContext _shopContext;

        public ProductController (ShopContext shopContext){
            _shopContext = shopContext;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult> GetAllProducts ([FromQuery] ProductParameterQuery parameterQuery){
            var filterBuilder = Builders<Product>.Filter;
            var filter = filterBuilder.Eq(p => p.IsAvailable, true);

            if (parameterQuery.MinPrice != null){
                filter &= filterBuilder.Gte(p => p.Price, parameterQuery.MinPrice);
            }
            
            if (parameterQuery.MaxPrice != null){
                filter &= filterBuilder.Lte(p => p.Price, parameterQuery.MaxPrice);
            }

            if (!string.IsNullOrEmpty(parameterQuery.SearchTerm)){
                var text = parameterQuery.SearchTerm;

                filter &= filterBuilder.Or(
                    filterBuilder.Regex(p => p.Colour, new BsonRegularExpression(text, "i")),
                    filterBuilder.Regex(p => p.Name, new BsonRegularExpression(text, "i")),
                    filterBuilder.Regex(p => p.LaneConditions, new BsonRegularExpression(text, "i")),
                    filterBuilder.Regex(p => p.Coverstock, new BsonRegularExpression(text, "i")),
                    filterBuilder.Regex(p => p.Core, new BsonRegularExpression(text, "i")) 
                );
            }
            
            var sort = Builders<Product>.Sort.Ascending(p => p.Id);

            if (!string.IsNullOrEmpty(parameterQuery.SortBy)){
                sort = parameterQuery.SortOrder == "desc" ? Builders<Product>.Sort.Descending(parameterQuery.SortBy) : Builders<Product>.Sort.Ascending(parameterQuery.SortBy);
            }

            var products = await _shopContext.Products.Find(filter).Sort(sort).Skip(parameterQuery.Size * (parameterQuery.Page - 1)).Limit(parameterQuery.Size).ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<Product>> GetProduct ([FromRoute] int id){
            var product = await _shopContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            return product == null ? NotFound() : Ok(product);
        }

        [HttpGet("by-manufacturer/{manufacturerName}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> GetProductsByManufacturer (string manufacturerName){
            var categoryFilter = Builders<Category>.Filter.Regex(
                c => c.ManufacturerName,
                new BsonRegularExpression(manufacturerName, "i")
            );
            
            var matchingCategories = await _shopContext.Categories.Find(categoryFilter).ToListAsync();

            if (!matchingCategories.Any()){
                return NotFound($"No categories found for manufacturer '{manufacturerName}'.");
            }

            var catId = matchingCategories.Select(c => c.Id).ToList();
            var productFilter = Builders<Product>.Filter.In(p => p.CategoryId, catId);
            var products = await _shopContext.Products.Find(productFilter).ToListAsync();

            return Ok(products);
        }

        //  Create
        [HttpPost]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> PostProduct (Product product){
            await _shopContext.Products.InsertOneAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        //  Update
        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateProduct (int id, Product updatedProduct){
            var result = await _shopContext.Products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
            
            if (result.MatchedCount == 0){
                return NotFound();
            }

            return NoContent();
        }

        //  Delete
        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> DeleteProduct (int id){
            var result = await _shopContext.Products.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount == 0 ? NotFound() : NoContent();
        }
    }
}