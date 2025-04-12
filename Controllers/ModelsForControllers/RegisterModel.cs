using System.ComponentModel.DataAnnotations;

namespace Thesis_ASP.Controllers.ViewModellsForControllers
{
    public class RegisterModel
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
