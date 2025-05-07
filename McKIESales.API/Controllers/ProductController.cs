using McKIESales.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace McKIESales.API.Controllers {
   [Route("/products")]
   [ApiController]
    public class ProductController : ControllerBase {
        private readonly ShopContext _shopContext;

        public ProductController (ShopContext shopContext){
            _shopContext = shopContext;
        }

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

            /*IQueryable<Product> products = _shopContext.Products.Where(p => p.IsAvailable == true);

            if (parameterQuery.MinPrice != null){
                products = products.Where(p => p.Price >= parameterQuery.MinPrice.Value);
            }

            if (parameterQuery.MaxPrice != null){
                products = products.Where(p => p.Price <= parameterQuery.MaxPrice.Value);
            }

            if (!string.IsNullOrEmpty(parameterQuery.SearchTerm)){
                products = products.Where(p => 
                    p.Colour.Contains(parameterQuery.SearchTerm) || p.Name.Contains(parameterQuery.SearchTerm) ||
                    p.LaneConditions.Contains(parameterQuery.SearchTerm) || p.Coverstock.Contains(parameterQuery.SearchTerm) ||
                    p.Core.Contains(parameterQuery.SearchTerm)
                );
            }

            if (!string.IsNullOrEmpty(parameterQuery.SortBy)){
                if (parameterQuery.SortOrder == "desc"){
                    products = products.OrderByDescending(p => EF.Property<object>(p, parameterQuery.SortBy));
                } else {
                    products = products.OrderBy(p => EF.Property<object>(p, parameterQuery.SortBy));
                }
            }

            products = products.Skip(parameterQuery.Size * (parameterQuery.Page - 1)).Take(parameterQuery.Size);
            return Ok(await products.ToArrayAsync());*/
        }

        [Route("/[controller]/{id}")]
        [HttpGet]
        public async Task<ActionResult<Product>> GetProduct ([FromRoute] int id){
            var product = await _shopContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            return product == null ? NotFound() : Ok(product);
        }

        //  Create
        [HttpPost]
        public async Task<ActionResult> PostProduct (Product product){
            await _shopContext.Products.InsertOneAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            
            /*_shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);*/
        }

        //  Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct (int id, Product updatedProduct){
            var result = await _shopContext.Products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
            
            if (result.MatchedCount == 0){
                return NotFound();
            }

            return NoContent();

            /*if (id != product.Id){
                return BadRequest();
            }

            _shopContext.Entry(product).State = EntityState.Modified;

            try {
                await _shopContext.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex) {
                if (!_shopContext.Products.Any(p => p.Id == id)){
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();*/
        }

        //  Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct (int id){
            var result = await _shopContext.Products.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount == 0 ? NotFound() : NoContent();
            
            
            /*var product = await _shopContext.Products.FindAsync(id);

            if(product == null){ 
                return NotFound(); 
            }

            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();
            return product;*/
        }
    }
}