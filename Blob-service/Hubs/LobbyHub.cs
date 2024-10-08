using Blob_service.Models;
using Microsoft.AspNetCore.SignalR;

namespace Blob_service.Hubs
{
    public class LobbyHub : Hub
    {
        private const int MaxUsersInGame = 4;
        private readonly static Dictionary<string, string> users = new Dictionary<string, string>();
        private readonly static List<AvailableGames> availableGames = new List<AvailableGames>();

        public async override Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("AvailableGames", availableGames);
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = users[Context.ConnectionId];
            await Clients.All.SendAsync("LobbyChatMessage", username + " has left the lobby");
            users.Remove(username);
        }

        public async Task Register(string username, string gameID)
        {
            if (!users.ContainsValue(username))
            {
                users.Add(Context.ConnectionId, username);
                await Clients.All.SendAsync("LobbyChatMessage", username + " has joined the lobby!");
            }
        }

        public async Task CreateGame(string gameID, string username)
        {
            availableGames.Add(new AvailableGames { GameID = gameID, OpponentUsername = username, Players = 1, ConnectionID = Context.ConnectionId });
            await Clients.All.SendAsync("AvailableGames", availableGames);
        }

        public async Task JoinGame(string gameID)
        {
            var currentGame = availableGames.Where(x => x.GameID == gameID).FirstOrDefault();
            if (currentGame?.Players < MaxUsersInGame)
            {
                currentGame.Players += 1;
                if (currentGame.Players >= MaxUsersInGame)
                {
                    var game = availableGames.Where(x => x.GameID == gameID).FirstOrDefault();
                    if (game != null)
                    {
                        availableGames.Remove(game);
                        await Clients.All.SendAsync("AvailableGames", availableGames);
                    }
                }
            }
        }

        public async Task SendChatMessage(string username, string message)
        {
            await Clients.All.SendAsync("LobbyChatMessage", username + ": " + message);
        }
    }
}
