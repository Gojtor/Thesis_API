using System.ComponentModel.DataAnnotations;

namespace Thesis_ASP.Controllers.ViewModellsForControllers
{
    public class LoginModel
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
