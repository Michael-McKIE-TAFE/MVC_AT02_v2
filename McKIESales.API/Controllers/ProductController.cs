using McKIESales.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace McKIESales.API.Controllers {
   [Route("/products")]
    public class ProductController : ControllerBase {
        private readonly ShopContext _shopContext;

        public ProductController (ShopContext shopContext){
            _shopContext = shopContext;
            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts ([FromQuery] ProductParameterQuery parameterQuery){
            IQueryable<Product> products = _shopContext.Products.Where(p => p.IsAvailable == true);

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
            return Ok(await products.ToArrayAsync());
        }

        [Route("/[controller]/{id}")]
        [HttpGet]
        public async Task<ActionResult> GetProduct ([FromRoute] int id){
            var product = await _shopContext.Products.FindAsync(id);

            if (product == null){
                return NotFound();
            } else {
                return Ok(product);
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostProduct (Product product){
            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct (int id, [FromBody] Product product){
            if (id != product.Id){
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

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult<Product>> DeleteProduct (int id){
            var product = await _shopContext.Products.FindAsync(id);

            if(product == null){ 
                return NotFound(); 
            }

            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();
            return product;
        }
    }
}