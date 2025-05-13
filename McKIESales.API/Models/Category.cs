using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace McKIESales.API.Models {
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