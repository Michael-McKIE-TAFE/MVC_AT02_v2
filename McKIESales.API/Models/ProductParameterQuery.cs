namespace McKIESales.API.Models {
    public class ProductParameterQuery : QueryParameters {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }
}