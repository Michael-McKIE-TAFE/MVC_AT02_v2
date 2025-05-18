using McKIESales.API.Models;
using MongoDB.Driver;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace McKIESales.API.Services {
    /// <summary>
	/// The `DatabaseSeeder` class populates MongoDB with initial data. 
	/// It creates unique indexes for `Category` and `Product` collections, 
	/// and seeds both with predefined categories and products if they are empty.
	/// </summary>
	public class DatabaseSeeder {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMongoCollection<Product> _productCollection;
		private readonly IMongoDatabase _database;


        public DatabaseSeeder (IMongoDatabase database){
            _database = database;
			_categoryCollection = database.GetCollection<Category>("categories");
            _productCollection = database.GetCollection<Product>("products");
        }

		//	The `SeedAsync` method does the following: it creates unique indexes for the `ManufacturerName`
		//	field in the `Category` collection and the `Name` field in the `Product` collection. It then
		//	checks if there are any categories or products in the database. If they don't exist, it inserts
		//	a predefined list of categories and products into their respective collections. This ensures the
		//	database is properly indexed and populated with initial data.
        public async Task SeedAsync (){
            var categoryIndexKeys = Builders<Category>.IndexKeys.Ascending(c => c.ManufacturerName);
			var categoryIndexOptions = new CreateIndexOptions { Unique = true };
			var categoryIndexModel = new CreateIndexModel<Category>(categoryIndexKeys, categoryIndexOptions);
			await _categoryCollection.Indexes.CreateOneAsync(categoryIndexModel);
			
			var productIndexKeys = Builders<Product>.IndexKeys.Ascending(p => p.Name);
			var productIndexOptions = new CreateIndexOptions { Unique = true };
			var productIndexModel = new CreateIndexModel<Product>(productIndexKeys, productIndexOptions);
			await _productCollection.Indexes.CreateOneAsync(productIndexModel);			
			
			//	Seed the categories if empty
			var categoriesExist = await _categoryCollection.Find(_ => true).AnyAsync();
            if (!categoriesExist){
                var categories = new List<Category> {
                    new() { Id = 1, ManufacturerName = "Brunswick" },
                    new() { Id = 2, ManufacturerName = "Columbia 300" },
                    new() { Id = 3, ManufacturerName = "Ebonite" },
                    new() { Id = 4, ManufacturerName = "Hammer" },
                    new() { Id = 5, ManufacturerName = "Roto Grip" },
                    new() { Id = 6, ManufacturerName = "Storm" },
                    new() { Id = 7, ManufacturerName = "Track" },
                    new() { Id = 8, ManufacturerName = "DV8" },
                };

                await _categoryCollection.InsertManyAsync(categories);
            }

			//	Seed products id empty
			var productsExist = await _productCollection.Find(_ => true).AnyAsync();
            if (!productsExist){
                var products = new List<Product> {
					new Product { Id = 1, CategoryId = 1, Name = "Crown Victory", Weight = 15, Colour = "Sapphire / Black", RG = 2.54, Diff = 0.038, LaneConditions = "Heavy", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 2, CategoryId = 1, Name = "Twist", Weight = 15, Colour = "Black / Gold / Silver", RG = 2.59, Diff = 0.018, LaneConditions = "Light", Coverstock = "Pearl Reactive", Core = "Symmetrical", Price = 200, IsAvailable = true },
					new Product { Id = 3, CategoryId = 1, Name = "Rhino", Weight = 14, Colour = "Red / Black / Gold", RG = 2.54, Diff = 0.030, LaneConditions = "Light", Coverstock = "Solid Reactive", Core = "Symmetrical", Price = 185, IsAvailable = true },
					new Product { Id = 4, CategoryId = 1, Name = "Hypnotize", Weight = 15, Colour = "Black / Blue / Navy / Purple", RG = 2.51, Diff = 0.056, LaneConditions = "Medium", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 425, IsAvailable = true },
					new Product { Id = 5, CategoryId = 1, Name = "Ethos Hybrid", Weight = 15, Colour = "Black / Purple / Grey", RG = 2.48, Diff = 0.053, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Symmetrical", Price = 420, IsAvailable = true },
					new Product { Id = 6, CategoryId = 1, Name = "Vaporize", Weight = 14, Colour = "Carbon", RG = 2.49, Diff = 0.046, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Symmetrical", Price = 450, IsAvailable = true },
					new Product { Id = 7, CategoryId = 2, Name = "Piranha PowerCOR Pearl", Weight = 15, Colour = "Jade / Black", RG = 2.52, Diff = 0.055, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 340, IsAvailable = true },
					new Product { Id = 8, CategoryId = 2, Name = "Super Cuda PowerCOR", Weight = 15, Colour = "Burgundy / Maroon  / Blue", RG = 2.50, Diff = 0.047, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 9, CategoryId = 2, Name = "White Dot", Weight = 14, Colour = "Scarlet / Gold / Black", RG = 0, Diff = 0, LaneConditions = "Light", Coverstock = "Polyester", Core = "Pancake", Price = 130, IsAvailable = true },
					new Product { Id = 10, CategoryId = 2, Name = "Rally", Weight = 14, Colour = "Royal / Blue / Lime / Silver", RG = 2.49, Diff = 0.050, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Symmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 11, CategoryId = 2, Name = "Ricochet Pearl", Weight = 15, Colour = "Midnight Pearl", RG = 2.49, Diff = 0.054, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 12, CategoryId = 2, Name = "Atlas Hybrid", Weight = 15, Colour = "Sky / Blue / Orange / Dark / Blue", RG = 2.52, Diff = 0.054, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 13, CategoryId = 3, Name = "Turbo X", Weight = 14, Colour = "Purple", RG = 2.55, Diff = 0.040, LaneConditions = "Light-Medium", Coverstock = "Solid Reactive", Core = "Symmetrical", Price = 400, IsAvailable = true },
					new Product { Id = 14, CategoryId = 3, Name = "Game Breaker 5 Pearl", Weight = 15, Colour = "Purple / Blue", RG = 2.48, Diff = 0.048, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 280, IsAvailable = true },
					new Product { Id = 15, CategoryId = 3, Name = "Crusher Hybrid", Weight = 14, Colour = "Violet / Purple / Silver", RG = 2.51, Diff = 0.057, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 16, CategoryId = 3, Name = "Real Time", Weight = 15, Colour = "Purple / Violet", RG = 2.51, Diff = 0.054, LaneConditions = "Medium", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 300, IsAvailable = true },
					new Product { Id = 17, CategoryId = 3, Name = "The One Reverb", Weight = 13, Colour = "Teal / Black / Red", RG = 2.60, Diff = 0.041, LaneConditions = "Medium-Heavy", Coverstock = "Pearl Reactive", Core = "Symmetrical", Price = 415, IsAvailable = true },
					new Product { Id = 18, CategoryId = 3, Name = "Fireball", Weight = 15, Colour = "Purple / Gold", RG = 2.52, Diff = 0.039, LaneConditions = "Light-Medium", Coverstock = "Pearl Reactive", Core = "Symmetrical", Price = 170, IsAvailable = true },
					new Product { Id = 19, CategoryId = 4, Name = "Hazmat Pearl", Weight = 13, Colour = "Blue / White / Cobalt", RG = 2.58, Diff = 0.040, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Symmetrical", Price = 275, IsAvailable = true },
					new Product { Id = 20, CategoryId = 4, Name = "Special Effect", Weight = 15, Colour = "Purple / Red / Grape / Black", RG = 2.47, Diff = 0.050, LaneConditions = "Medium-Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 295, IsAvailable = true },
					new Product { Id = 21, CategoryId = 4, Name = "Hammerhead", Weight = 14, Colour = "Blue / Navey / Sky", RG = 2.51, Diff = 0.048, LaneConditions = "Medium-Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 225, IsAvailable = true },
					new Product { Id = 22, CategoryId = 4, Name = "Axe", Weight = 13, Colour = "Green / Smoke", RG = 0, Diff = 0, LaneConditions = "Light", Coverstock = "Polyester", Core = "Pancake", Price = 200, IsAvailable = true },
					new Product { Id = 23, CategoryId = 4, Name = "Black Widow", Weight = 15, Colour = "Cobalt / Black", RG = 2.50, Diff = 0.058, LaneConditions = "Medium-Heavy", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 295, IsAvailable = true },
					new Product { Id = 24, CategoryId = 4, Name = "Scorpion Strike", Weight = 15, Colour = "Black / Magenta / Indigo", RG = 2.48, Diff = 0.045, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 265, IsAvailable = true },
					new Product { Id = 25, CategoryId = 5, Name = "RST Hyperdrive", Weight = 16, Colour = "Mango / Magenta / Royal", RG = 2.51, Diff = 0.051, LaneConditions = "Medium-Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 26, CategoryId = 5, Name = "X-Cell", Weight = 16, Colour = "Turquoise / Navy / Black", RG = 2.50, Diff = 0.050, LaneConditions = "Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 420, IsAvailable = true },
					new Product { Id = 27, CategoryId = 5, Name = "Attention Star S2", Weight = 14, Colour = "Berry / Silver / Black Aqua", RG = 2.53, Diff = 0.046, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 335, IsAvailable = true },
					new Product { Id = 28, CategoryId = 5, Name = "Attention Star", Weight = 15, Colour = "Berry / Silver / Iron", RG = 2.48, Diff = 0.049, LaneConditions = "Medium-Heavy", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 335, IsAvailable = true },
					new Product { Id = 29, CategoryId = 5, Name = "Rockstar", Weight = 15, Colour = "Crimson / Jet / Maroon", RG = 2.48, Diff = 0.050, LaneConditions = "Medium", Coverstock = "Solid Reactive", Core = "Symmetrical", Price = 305, IsAvailable = true },
					new Product { Id = 30, CategoryId = 5, Name = "Optimum Idol Pearl", Weight = 14, Colour = "Bubblegum / Iris", RG = 2.51, Diff = 0.054, LaneConditions = "Medium-Heavy", Coverstock = "Pearl Reactive", Core = "Symmetrical", Price = 285, IsAvailable = true },
					new Product { Id = 31, CategoryId = 6, Name = "Equinox", Weight = 15, Colour = "Goldenrod / Deep Violet / Chromium", RG = 2.48, Diff = 0.054, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 345, IsAvailable = true },
					new Product { Id = 32, CategoryId = 6, Name = "Ion Pro", Weight = 14, Colour = "Navy / Carbon / Steel", RG = 2.51, Diff = 0.036, LaneConditions = "Medium", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 335, IsAvailable = true },
					new Product { Id = 33, CategoryId = 6, Name = "Ion Max", Weight = 15, Colour = "Neon Pink / Indigo / White", RG = 2.47, Diff = 0.055, LaneConditions = "Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 335, IsAvailable = true },
					new Product { Id = 34, CategoryId = 6, Name = "Identity", Weight = 13, Colour = "Cherry / Teal / Black", RG = 2.56, Diff = 0.034, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 400, IsAvailable = true },
					new Product { Id = 35, CategoryId = 6, Name = "Level", Weight = 15, Colour = "Black", RG = 2.59, Diff = 0.027, LaneConditions = "Light-Medium", Coverstock = "RPM Solid", Core = "Symmetrical", Price = 340, IsAvailable = true },
					new Product { Id = 36, CategoryId = 6, Name = "Physix Blackout", Weight = 16, Colour = "Blackout", RG = 2.47, Diff = 0.056, LaneConditions = "Medium-Heavy", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 37, CategoryId = 7, Name = "Cypher Pearl", Weight = 14, Colour = "Blue / Amethyst / Silver", RG = 2.57, Diff = 0.047, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 410, IsAvailable = true },
					new Product { Id = 38, CategoryId = 7, Name = "Criterion Pearl", Weight = 15, Colour = "Black / Aquamarine / Gold", RG = 2.50, Diff = 0.052, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 410, IsAvailable = true },
					new Product { Id = 39, CategoryId = 7, Name = "Stealth Drone", Weight = 13, Colour = "Navy Blue / Raven", RG = 2.57, Diff = 0.040, LaneConditions = "Medium-Heavy", Coverstock = "Solid Reactive", Core = "Symmetrical", Price = 330, IsAvailable = true },
					new Product { Id = 40, CategoryId = 7, Name = "Theorem", Weight = 16, Colour = "Purple / Blue / Sky Blue", RG = 2.48, Diff = 0.044, LaneConditions = "Medium", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 41, CategoryId = 7, Name = "Criterion", Weight = 15, Colour = "Purple / Blue / Teal", RG = 2.50, Diff = 0.052, LaneConditions = "Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 265, IsAvailable = true },
					new Product { Id = 42, CategoryId = 7, Name = "Theorem", Weight = 15, Colour = "Black / Red / Violet", RG = 2.47, Diff = 0.046, LaneConditions = "Medium-Heavy", Coverstock = "Hybrid Reactive", Core = "Symmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 43, CategoryId = 8, Name = "Hater Pearl", Weight = 15, Colour = "Grey / Purple / Black", RG = 2.53, Diff = 0.054, LaneConditions = "Medium-Heavy", Coverstock = "Pearl Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 44, CategoryId = 8, Name = "Dark Side", Weight = 14, Colour = "Red / Purple", RG = 2.50, Diff = 0.035, LaneConditions = "Medium", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 300, IsAvailable = true },
					new Product { Id = 45, CategoryId = 8, Name = "Trouble Maker Solid", Weight = 15, Colour = "Digital Camo", RG = 2.49, Diff = 0.045, LaneConditions = "Medium-Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 435, IsAvailable = true },
					new Product { Id = 46, CategoryId = 8, Name = "Heckler", Weight = 16, Colour = "Black / Citrine / Purple", RG = 2.51, Diff = 0.043, LaneConditions = "Medium-Heavy", Coverstock = "Solid Reactive", Core = "Asymmetrical", Price = 420, IsAvailable = true },
					new Product { Id = 47, CategoryId = 8, Name = "Mantra", Weight = 15, Colour = "Black / Mint / Blue", RG = 2.49, Diff = 0.047, LaneConditions = "Medium", Coverstock = "Hybrid Reactive", Core = "Asymmetrical", Price = 420, IsAvailable = true },
					new Product { Id = 48, CategoryId = 8, Name = "Wicked Collision", Weight = 13, Colour = "Royal Blue / Purple / Teal", RG = 2.59, Diff = 0.041, LaneConditions = "Medium-Heavy", Coverstock = "Solid Reactive", Core = "Symmetrical", Price = 435, IsAvailable = true }
				};

				await _productCollection.InsertManyAsync(products);
            }
        }
    }
}