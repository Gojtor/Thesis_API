using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Thesis_ASP.Resources;

namespace Thesis_ASP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TCGController : ControllerBase
{
    private readonly ILogger<TCGController> logger;
    private TCGDbContext tcgDbContext;
    public TCGController(ILogger<TCGController> logger,TCGDbContext tcgDbContext)
    {
        this.logger = logger;
        this.tcgDbContext = tcgDbContext;
    }

    [HttpGet("GetCard/{id}")]
    public async Task<IActionResult> GetCard(long id)
    {
        Card? card = await tcgDbContext.Cards.FindAsync(id);
        if (card == null) { return NotFound(); }
        return Ok(card);
    }

    [HttpGet("GetCardByCardID/{cardID}")]
    public async Task<IActionResult> GetCardByCardID(string cardID)
    {
        Card? card = tcgDbContext.Cards.Where(x => x.cardID == cardID).Single();
        if (card == null) { return NotFound(); }
        return Ok(card);
    }

    [HttpPost("SetCard")]
    public async Task<IActionResult> SetCard(string cardID, string cardName, string effect, int cost, int power, 
        int counter, string trigger, CardType cardType, CharacterType characterType, Attributes attribute, Colors color)
    {
        Card card = new Card() {
            cardID = cardID,
            cardName = cardName,
            effect = effect,
            cost = cost,
            power=power,
            counter = counter,
            trigger = trigger,
            cardType = cardType,
            characterType = characterType,
            attribute = attribute,
            color = color
        };
        tcgDbContext.Cards.Add(card);
        await tcgDbContext.SaveChangesAsync();
        return Ok(card);
    }
}
