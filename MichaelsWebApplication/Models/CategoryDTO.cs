namespace MichaelsWebApplication.Models {
    public class CategoryDTO {
        public int Id { get; set; }
        public string? ManufacturerName { get; set; }
        
        //  DO NOT DELETE the below line, it will break the entire app
        //  despite being apparerntly unreferenced.
        public virtual List<ProductDTO>? Products { get; set; }
    }
}