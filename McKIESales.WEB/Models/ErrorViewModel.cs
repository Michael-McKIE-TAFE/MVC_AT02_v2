namespace McKIESales.WEB.Models {
    /// <summary>
    /// This class holds the RequestId property, which is a string. 
    /// The ShowRequestId property is a computed property that returns 
    /// true if the RequestId is not null or empty, helping determine 
    /// whether to display the request ID on error pages.
    /// </summary>
    public class ErrorViewModel {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}