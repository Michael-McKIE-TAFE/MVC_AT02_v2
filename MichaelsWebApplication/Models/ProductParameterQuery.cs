namespace MichaelsWebApplication.Models {
    public class ProductParameterQuery {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchTerm { get; set; }
    }
}