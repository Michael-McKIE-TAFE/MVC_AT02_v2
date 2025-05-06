using Microsoft.AspNetCore.Routing.Constraints;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace McKIESales.API.Models {
    public class Product {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string? Name { get; set; }

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

        [JsonIgnore]
        public virtual Category? Category { get; set; }
    }
}