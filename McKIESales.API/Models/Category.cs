using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace McKIESales.API.Models {
    /// <summary>
    /// The `Category` class represents a product category, such as a manufacturer, 
    /// with properties like `Id` (the primary key) and `ManufacturerName` (the name 
    /// of the manufacturer). It also includes a `Products` list, which holds the 
    /// products in that category, and is marked as `virtual` to enable lazy loading.
    /// </summary>
    public class Category {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string? ManufacturerName { get; set; }
        
        //  DO NOT DELETE the below line, it will break the entire app
        //  despite being apparerntly unreferenced.
        public virtual List<Product>? Products { get; set; }
    }
}