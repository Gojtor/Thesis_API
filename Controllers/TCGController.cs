using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System;
using System.Net.WebSockets;
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

    [HttpGet("GetAllCardByFromGameDBByGameID")]
    public async Task<IActionResult> GetAllCardByFromGameDBByGameID(string gameCustomID)
    {
        List<InGameCard> cards = tcgDbContext.InGameCards.Where(x => x.gameCustomID == gameCustomID).ToList();
        if (cards == null) { return NotFound(); }
        return Ok(cards);
    }

    [HttpGet("GetAllCardByFromGameDBByGameIDAndPlayer")]
    public async Task<IActionResult> GetAllCardByFromGameDBByGameIDAndPlayer(string gameCustomID,string playerName)
    {
        List<InGameCard> cards = tcgDbContext.InGameCards.Where(x => x.gameCustomID == gameCustomID && x.playerName==playerName).ToList();
        if (cards == null) { return NotFound(); }
        return Ok(cards);
    }

    [HttpGet("GetCardByFromGameDBByGameIDAndPlayerAndCustomCardID")]
    public async Task<IActionResult> GetCardByFromGameDBByGameIDAndPlayerAndCustomCardID(string gameCustomID, string playerName,string customCardID)
    {
        var card = await tcgDbContext.InGameCards.Where(x => x.gameCustomID == gameCustomID && x.playerName == playerName && x.customCardID==customCardID).SingleOrDefaultAsync();
        if (card == null) { return NotFound(); }
        return Ok(card);
    }

    [HttpGet("GetCardCountFromGameDB")]
    public async Task<IActionResult> GetCardCountFromGameDB()
    {
        if (tcgDbContext.InGameCards == null) { return NotFound(); }
        string deckCount=tcgDbContext.InGameCards.Where(x => x.currentParent.Contains("Deck")).Count().ToString();
        string handCount=tcgDbContext.InGameCards.Where(x => x.currentParent.Contains("Hand")).Count().ToString();
        string lifeCount=tcgDbContext.InGameCards.Where(x => x.currentParent.Contains("Life")).Count().ToString();
        return Ok("All cards: "+tcgDbContext.InGameCards.Count()+"\nDeck cards count: "+ deckCount + ", Hand cards count: "+handCount+", Life cards count: "+ lifeCount);
    }

    [HttpGet("DeleteCardsFromGameDB")]
    public async Task<IActionResult> DeleteCardsFromGameDB()
    {
        if (tcgDbContext.InGameCards == null) { return NotFound(); }
        await tcgDbContext.DeleteAllInGameCards();
        return Ok("Cards deleted from GameDB");
    }

    [HttpPost("SetCardToGameDB")]
    public async Task<IActionResult> SetCardToGameDB([FromBody] InGameCard card)
    {
        if (card == null)
        {
            Console.WriteLine("Érvénytelen JSON adat!"); // Log konzolba
            return BadRequest("Érvénytelen JSON adat!");
        }
        Console.WriteLine($"Kapott kártya: {card.cardName} - ID: {card.cardID}");
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

    [HttpPost("SetEnemyCardsToGameDBNoJSON")]
    public async Task<IActionResult> SetEnemyCardsToGameDBNoJSON()
    {
        await tcgDbContext.AddEnemyCardsFromJSON();
        return Ok();
    }

    [HttpPost("EnemyConnectedTest")]
    public async Task<IActionResult> EnemyConnectedTest(string gameID)
    {
        await tcgDbContext.SendEnemyConnectedToClient(gameID);
        return Ok();
    }

    [HttpPost("UpdateCardInGameDB")]
    public async Task<IActionResult> UpdateCardInGameDB([FromBody] InGameCard updatedCard)
    {
        var card = await tcgDbContext.InGameCards
            .Where(x => x.gameCustomID == updatedCard.gameCustomID && x.playerName == updatedCard.playerName && x.customCardID == updatedCard.customCardID)
            .SingleOrDefaultAsync();
        if (card == null)
        {
            return NotFound();
        }
        card.cardID = updatedCard.cardID;
        card.cardName = updatedCard.cardName;
        card.effect = updatedCard.effect;
        card.cost = updatedCard.cost;
        card.power = updatedCard.power;
        card.counter = updatedCard.counter;
        card.trigger = updatedCard.trigger;
        card.cardType = updatedCard.cardType;
        card.characterType = updatedCard.characterType;
        card.attribute = updatedCard.attribute;
        card.color = updatedCard.color;
        card.active = updatedCard.active;
        card.cardVisibility = updatedCard.cardVisibility;
        card.customCardID = updatedCard.customCardID;
        card.playerName = updatedCard.playerName;
        card.gameCustomID = updatedCard.gameCustomID;
        card.currentParent = updatedCard.currentParent;
        await tcgDbContext.SaveChangesAsync();
        return Ok(card);
    }
}
