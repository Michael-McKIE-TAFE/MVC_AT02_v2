namespace McKIESales.API.Models {
    /// <summary>
    /// This class extends `QueryParameters` but simplifies the product filtering by only including 
    /// `MinPrice`, `MaxPrice`, and `SearchTerm`. It enables querying products with specific 
    /// price ranges and search terms, while still supporting pagination and sorting features from
    /// the base class.
    /// </summary>
    public class ProductParameterQuery : QueryParameters {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
    }
}