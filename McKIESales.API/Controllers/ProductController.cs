﻿using Asp.Versioning;
using McKIESales.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace McKIESales.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class ProductController : ControllerBase {
        private readonly ShopContext _shopContext;

        public ProductController (ShopContext shopContext){
            _shopContext = shopContext;
        }
        
        //  This API Endpoint retrieves a list of available products from the database,
        //  applying filters based on price range, search term, and optional sorting criteria.
        //  It supports pagination by using the `Page` and `Size` query parameters and returns
        //  the filtered and sorted product list in the response.
        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult> GetAllProducts ([FromQuery] ProductParameterQuery parameterQuery){
            try {
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
            } catch (Exception ex){
                return StatusCode(500, new { message = "An unexpected error occurred, trying to get all products. Please try again later.\nDetails: " + ex });
            }
        }

        //  This API Endpoint retrieves a single product by its ID from the database.
        //  If the product is found, it returns the product details; otherwise,
        //  it responds with a "Not Found" status.
        [HttpGet("{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Product>> GetProduct ([FromRoute] int id){
            try {
                var product = await _shopContext.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
                return product == null ? NotFound() : Ok(product);
            } catch (Exception ex){
                return StatusCode(500, new { message = "An unexpected error occurred, trying to get product via ID. Please try again later.\nDetails: " + ex });
            }
        }

        //  This API Endpoint retrieves products based on the manufacturer name by first finding matching categories.
        //  If no categories are found, it returns a "Not Found" response. If categories exist, it retrieves and
        //  returns all products linked to those categories. Any unexpected errors result in a 500 internal server
        //  error response.
        [HttpGet("by-manufacturer/{manufacturerName}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> GetProductsByManufacturer (string manufacturerName){
            try {
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
            } catch (Exception ex){
                return StatusCode(500, new { message = "An unexpected error occurred, trying to get product by manufacturer name. Please try again later.\nDetails: " + ex });
            }
        }

        //  This API endpoint handles GET requests for version 2.0, retrieving products that match one or more specified lane conditions.
        //  It parses the input query string, constructs a MongoDB regex filter for case-insensitive matching, and returns the filtered
        //  products or an appropriate error response if something goes wrong.
        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> GetProductsByLaneCondition ([FromQuery]string laneCondition){
            try {
                var conditions = laneCondition?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim().ToLower()).ToList();

                if (conditions == null || !conditions.Any()){
                    return BadRequest("At least one lane condition must be provided.");
                }

                var filter = Builders<Product>.Filter.Or(conditions.Select(condition => Builders<Product>.Filter.Regex(p => p.LaneConditions, new MongoDB.Bson.BsonRegularExpression(condition, "i"))).ToArray());
                var products = await _shopContext.Products.Find(filter).ToListAsync();
                return Ok(products);
            } catch (Exception ex){
                return StatusCode(500, new { message = "An unexpected error occurred, trying to get products via lane condition. Please try again later.\nDetails: " + ex });
            }
        }

        //  This API Endpoint adds a new product to the database by inserting it into the `Products` collection.
        //  After the product is created, it returns a "Created" response along with the newly created
        //  product's details and a link to retrieve it by ID.
        [HttpPost]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult> PostProduct (Product product){
            try {
                await _shopContext.Products.InsertOneAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            } catch (Exception ex){
                return StatusCode(500, new { message = "An unexpected error occurred, trying to add a new product. Please try again later.\nDetails: " + ex });
            }
        }

        //  This API Endpoint updates an existing product in the database based on the provided ID.
        //  If no product is found with the given ID, it returns a "Not Found" response; otherwise,
        //  it updates the product and returns a "No Content" status to indicate successful completion.
        [HttpPut("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> UpdateProduct (int id, Product updatedProduct){
            try {
                var result = await _shopContext.Products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
            
                if (result.MatchedCount == 0){
                    return NotFound();
                }

                return CreatedAtAction(nameof(GetProduct), new { id = updatedProduct.Id }, updatedProduct);
            } catch (Exception ex){
                return StatusCode(500, new { message = "An unexpected error occurred, trying to update a product. Please try again later.\nDetails: " + ex });
            }
        }

        //  This API Endpoint deletes a product from the database by its ID.
        //  If no product is found with the specified ID, it returns a "Not Found" response;
        //  otherwise, it returns a "No Content" status to indicate the deletion was successful.
        [HttpDelete("{id}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> DeleteProduct (int id){
            try {
                var result = await _shopContext.Products.DeleteOneAsync(p => p.Id == id);
                return result.DeletedCount == 0 ? NotFound() : NoContent();
            } catch (Exception ex){
                return StatusCode(500, new { message = "An unexpected error occurred, trying to delete a product. Please try again later.\nDetails: " + ex });
            }
        }
    }
}