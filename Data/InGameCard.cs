using System.ComponentModel.DataAnnotations;
using Thesis_ASP.Resources;

namespace Thesis_ASP.Data
{
    public class InGameCard
    {
        //Card Data-s
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
        public CardVisibility cardVisibility { get; set; } = CardVisibility.NONE;
        public bool active { get; set; }
        public string customCardID { get; set; } //This is needed when there is more than 1 from the same card in the deck

        //Game Data-s
        public string playerName { get; set; }
        public string gameCustomID { get; set; }
        public string currentParent { get; set; }
        [Key]
        public long id { get; set; }
    }
}
