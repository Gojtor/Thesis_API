using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Thesis_ASP.Data
{
    public class User : IdentityUser
    {
        public string DeckJson { get; set; } = string.Empty;
        public DateTime lastLoggedIn { get; set; }
        public DateTime registeredTimne { get; set; }

        public ICollection<Friendship> Friends { get; set; }
    }
}
