using System.ComponentModel.DataAnnotations;

namespace McKIESales.WEB.Models {
    /// <summary>
    /// This class includes two required fields, Email and Password, 
    /// both of which are used in the login process for user authentication.
    /// </summary>
    public class LoginViewModel {
        [Required]
        public string? Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}