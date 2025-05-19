using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}