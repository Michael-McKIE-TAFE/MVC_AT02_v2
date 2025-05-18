using Microsoft.AspNetCore.Routing.Constraints;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace McKIESales.API.Models {
    /// <summary>
    /// This class represents a product in the system, with properties such as 
    /// `Id`, `Name`, `Weight`, `Colour`, `RG`, and other attributes like 
    /// `LaneConditions`, `Coverstock`, `Core`, and `Price`. It includes validation 
    /// annotations (e.g., `Required`) and MongoDB-specific attributes (`BsonId`, 
    /// `BsonRepresentation`) for mapping to the database. The `CategoryId` property 
    /// links the product to a specific category.
    /// </summary>
    public class Product {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string? Name { get; set; } = null!;

        [Required]
        public int? Weight { get; set; }

        [Required]
        public string? Colour { get; set; }

        [Required]
        public double? RG { get; set; }

        [Required]
        public double Diff { get; set; }

        [Required]
        public string? LaneConditions { get; set; }

        [Required]
        public string? Coverstock { get; set; }

        [Required]
        public string? Core { get; set; }

        [Required]
        public decimal? Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}