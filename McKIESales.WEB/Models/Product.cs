using System.ComponentModel.DataAnnotations;

namespace McKIESales.WEB.Models {
    /// <summary>
    /// A copy of the model that was used in the API for displaying the details on the
    /// application front end.
    /// </summary>
    public class Product {
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