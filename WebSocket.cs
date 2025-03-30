using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Thesis_ASP
{
    public class WebSocket : Hub
    {
        public async Task AddClientToGroupByGameID(string gameID,string name)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
            Console.WriteLine("Player: "+name+" added to the game with the following ID: "+gameID);
            await Clients.Group(gameID).SendAsync("AddedToClient", "Player: " + name + " added to the game with the following ID: " + gameID);
        }

        public async Task ReceiveMessageFromClient(string gameID,string message)
        {
            Console.WriteLine(gameID+" "+message);
            await Clients.Group(gameID).SendAsync("ReceiveMessage", "Message: "+message+" received from client");
        }
    }
}
