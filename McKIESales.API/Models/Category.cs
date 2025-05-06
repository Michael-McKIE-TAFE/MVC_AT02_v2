using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace McKIESales.API.Models {
    public class Category {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        public string? ManufacturerName { get; set; }
        public virtual List<Product>? Products { get; set; }
    }
}