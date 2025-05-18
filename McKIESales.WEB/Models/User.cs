using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace McKIESales.WEB.Models {
    /// <summary>
    /// This class defines a structure for a user object that will be stored in MongoDB. 
    /// It includes an Id field marked as the primary key with special MongoDB attributes. 
    /// The class also has Username, Email, and Password fields, all of which can be null. 
    /// </summary>
    public class User {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}