namespace McKIESales.API.Models {
    public class QueryParameters {
        private const int MaxSize = 100;
        private int _pageSize = 50;
        private string sortOrder = "asc";

        public int Page { get; set; } = 1;
        public int Size { 
            get { return _pageSize; } 
            set { _pageSize = Math.Min(_pageSize, value); }        
        }
        public string SortBy { get; set; } = "Id";
        public string SortOrder {
            get { return SortOrder; } 
            set {
                if (value == "asc" || value == "desc"){
                    sortOrder = value;
                }
            }
        }
    }
}