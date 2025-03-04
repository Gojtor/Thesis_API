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

    [HttpGet("GetCardByFromGameDB")]
    public async Task<IActionResult> GetCardByFromGameDB(string gameCustomID, string playerName, string cardCustomID)
    {
        InGameCard? card = tcgDbContext.InGameCards.Where(x => x.gameCustomID==gameCustomID && x.playerName==playerName && x.customCardID == cardCustomID).Single();
        if (card == null) { return NotFound(); }
        return Ok(card);
    }

    [HttpGet("GetAllCardByFromGameDB")]
    public async Task<IActionResult> GetAllCardByFromGameDB()
    {
        if (tcgDbContext.InGameCards == null) { return NotFound(); }
        return Ok(tcgDbContext.InGameCards);
    }

    [HttpGet("GetCardCountFromGameDB")]
    public async Task<IActionResult> GetCardCountFromGameDB()
    {
        if (tcgDbContext.InGameCards == null) { return NotFound(); }
        return Ok(tcgDbContext.InGameCards.Count());
    }

    [HttpPost("SetCardToGameDB")]
    public async Task<IActionResult> SetCardToGameDB([FromBody] InGameCard card)
    {
        if (card == null)
        {
            Console.WriteLine("�rv�nytelen JSON adat!"); // Log konzolba
            return BadRequest("�rv�nytelen JSON adat!");
        }
        Console.WriteLine($"Kapott k�rtya: {card.cardName} - ID: {card.cardID}");
        tcgDbContext.InGameCards.Add(card);
        await tcgDbContext.SaveChangesAsync();
        return Ok(card);
    }

    [HttpPost("SetCardToGameDBNoJSON")]
    public async Task<IActionResult> SetCardToGameDBNoJSON(string cardID, string cardName, string effect, int cost, int power,
        int counter, string trigger, CardType cardType, CharacterType characterType, Attributes attribute, Colors color,
        bool active, string customCardID, string playerName, string gameCustomID)
    {
        InGameCard card = new InGameCard()
        {
            cardID = cardID,
            cardName = cardName,
            effect = effect,
            cost = cost,
            power = power,
            counter = counter,
            trigger = trigger,
            cardType = cardType,
            characterType = characterType,
            attribute = attribute,
            color = color,
            active = active,
            customCardID = customCardID,
            playerName = playerName,
            gameCustomID = gameCustomID
        };
        tcgDbContext.InGameCards.Add(card);
        await tcgDbContext.SaveChangesAsync();
        return Ok(card);
    }
}
