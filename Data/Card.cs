using System.ComponentModel.DataAnnotations;
using Thesis_ASP.Resources;

namespace Thesis_ASP.Data
{
    public class Card
    {
        [Key]
        public long id { get; set; }
        public string cardID { get; set; }
        public string cardName { get; set; }
        public string effect { get; set; }
        public int cost { get; set; }
        public int power { get; set; }
        public int counter { get; set; }
        public string trigger { get; set; }
        public CardType cardType { get; set; }
        public CharacterType characterType { get; set; }
        public Attributes attribute { get; set; }
        public Colors color { get; set; }

    }
}
