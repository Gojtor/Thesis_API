using System.ComponentModel.DataAnnotations;

namespace Thesis_ASP.Data
{
    public class Friendship
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FriendName { get; set; }

        public bool IsAccepted { get; set; }
    }
}
