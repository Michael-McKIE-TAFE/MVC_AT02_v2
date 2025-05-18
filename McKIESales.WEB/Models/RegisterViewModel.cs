using System.ComponentModel.DataAnnotations;

namespace McKIESales.WEB.Models {
    /// <summary>
    /// The `RegisterViewModel` class is a model used to capture and validate 
    /// the data inputted by the user during registration. It contains three 
    /// properties: `Username`, `Email`, and `Password`. Each property is 
    /// decorated with validation attributes to ensure proper data entry. 
    /// `Username` and `Email` are required fields, and `Email` is further 
    /// validated to check if the input is a valid email address. The `Password` 
    /// field is required and its data type is set to `Password`, which is 
    /// typically used for masking passwords during input.
    /// </summary>
    public class RegisterViewModel {
        [Required]
        public string? Username { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}