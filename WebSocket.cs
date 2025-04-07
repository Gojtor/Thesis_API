using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Thesis_ASP
{
    public class WebSocket : Hub
    {
        private readonly GameGroupManager groupManager;

        public WebSocket(GameGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        public async Task AddClientToGroupByGameID(string gameID,string name)
        {
            if (!groupManager.Groups.ContainsKey(gameID))
            {
                await Clients.Caller.SendAsync("GroupDoesntExist", "Game with this ID doesnt exists");
                Console.WriteLine("Game with this ID doesnt exists, ID: "+gameID);
            }
            if (groupManager.Groups[gameID].Contains(name))
            {
                await Clients.Caller.SendAsync("PlayerAlreadyInThisGroup", "A player with this name already in game");
                Console.WriteLine("A player with this name already in game, player name: "+ name+" , game id: "+gameID);
            }
            else if (groupManager.Groups[gameID].Count>1)
            {
                await Clients.Caller.SendAsync("TwoPlayerAlreadyInGame", "Two player is already in game");
                Console.WriteLine("Two player is already in game, game id: "+gameID);
            }
            else
            {
                groupManager.Groups[gameID].Add(name);
                await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
                Console.WriteLine("Player: " + name + " added to the game with the following ID: " + gameID + " GROUPS COUNT: " + groupManager.Groups.Count);
                await Clients.Caller.SendAsync("AddedToGroup", "The player is added to the group");
            }
                
        }

        public async Task CreateGroupByGameIDAndAddFirstClient(string gameID, string name)
        {
            if (groupManager.Groups.ContainsKey(gameID))
            {
                await Clients.Caller.SendAsync("GroupAlreadyExist", "Game with this ID already exists");
                Console.WriteLine("Game with this ID: "+gameID+" already exists");
            }
            else
            {
                groupManager.Groups.Add(gameID, new List<string>());
                groupManager.Groups[gameID].Add(name);
                await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
                Console.WriteLine("Player: " + name + " added to the game with the following ID: " + gameID+" GROUPS COUNT: "+ groupManager.Groups.Count);
                await Clients.Caller.SendAsync("GameGroupCreated", "Player: " + name + " created game with the following ID: " + gameID);
            } 
        }

        public async Task ReceiveMessageFromClient(string gameID,string message)
        {
            Console.WriteLine(gameID+" "+message);
            await Clients.Group(gameID).SendAsync("ReceiveMessage", "Message: "+message+" received from client");
        }

        public async Task ReAssureEnemyConnected(string gameID,string name)
        {
            Console.WriteLine("Enemy connected to my game: "+gameID + " GROUPS COUNT: " + groupManager.Groups.Count);
            await Clients.OthersInGroup(gameID).SendAsync("Connected", "The following player connected to the game: ", name);
            Random random = new Random();
            string whoIsFirst =groupManager.Groups[gameID][random.Next(0, 2)];
            Console.WriteLine("First player is: "+whoIsFirst);
            await Clients.Group(gameID).SendAsync("WhoIsFirst",whoIsFirst);
        }

        public async Task DoneWithMulliganOrKeep(string gameID)
        {
            Console.WriteLine("Done with mulligan");
            await Clients.OthersInGroup(gameID).SendAsync("DoneWithStartingHand", "Done to the mulligan or keeping hand");
        }

        public async Task UpdateMyBoardAtEnemy(string gameID)
        {
            await Clients.OthersInGroup(gameID).SendAsync("UpdateEnemyBoard", "Update enemy board!");
        }

        public async Task ConnectedAsEnemy(string gameID, string name)
        {
            await Clients.OthersInGroup(gameID).SendAsync("EnemyConnected", "Player: " + name + " added to the game with the following ID: " + gameID,name);
        }
        public async Task UpdateMyCardAtEnemy(string gameID,string customCardID)
        {
            Console.WriteLine("Update this card: " + customCardID);
            await Clients.OthersInGroup(gameID).SendAsync("UpdateThisEnemyCard", customCardID);
        }

        public async Task ChangeEnemyGameStateToPlayerPhase(string gameID)
        {
            Console.WriteLine("SWITCHING PHASES!");
            await Clients.OthersInGroup(gameID).SendAsync("ChangeGameStateToPlayerPhase","Changing game phases!");
        }

        public async Task UpdateMyLeaderCardAtEnemy(string gameID, string customCardID)
        {
            Console.WriteLine("Update this card: " + customCardID);
            await Clients.OthersInGroup(gameID).SendAsync("UpdateEnemyLeaderCard", customCardID);
        }

        public async Task AttackedEnemyCard(string gameID,string cardThatAttacksID, string attackedCard, int power)
        {
            Console.WriteLine("Card: " + cardThatAttacksID +" with this power: "+power+" attacked this card: "+attackedCard);
            await Clients.OthersInGroup(gameID).SendAsync("MyCardIsAttacked", cardThatAttacksID,attackedCard,power);
        }

        public async Task BattleEnded(string gameID,string attackerID,string attackedID)
        {
            Console.WriteLine("The battle ended between attacker: " + attackerID + " and attacked: " + attackedID);
            await Clients.OthersInGroup(gameID).SendAsync("BattleEnded", "The battle ended between attacker: "+attackerID+" and attacked: "+attackedID);
        }
    }
}
