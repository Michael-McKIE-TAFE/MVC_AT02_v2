namespace McKIESales.API.Models {
    public class QueryParameters {
        private const int MaxSize = 100;
        private int _pageSize = 50;
        private string _sortOrder = "asc";
        private string _sortBy = "Id";

        public int Page { get; set; } = 1;
        public int Size { 
            get { return _pageSize; } 
            set { _pageSize = Math.Min(MaxSize, value); }        
        }

        public string SortBy { 
            get { return _sortBy; }
            set {
                if (!string.IsNullOrWhiteSpace(value)){
                    _sortBy = value.Trim();
                }
            }
        }

        public string SortOrder {
            get { return _sortOrder; } 
            set {
                var lowerValue = value.ToLower();
                if (lowerValue == "asc" || lowerValue == "desc"){
                    _sortOrder = lowerValue;
                }
            }
        }
    }
}