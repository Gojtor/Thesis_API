using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Thesis_ASP.Data
{
    public class User : IdentityUser
    {
        [Key]
        public long userID { get; set; }
        public DateTime lastLoggedIn { get; set; }
        public DateTime registeredTimne { get; set; }

    }
}
