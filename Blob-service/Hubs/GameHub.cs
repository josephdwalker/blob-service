using Microsoft.AspNetCore.SignalR;

namespace Blob_service.Hubs
{
    public class GameHub : Hub
    {
        private const int MaxUsersInGame = 6;
        public static Dictionary<string, string> users = new Dictionary<string, string>();
        public static Dictionary<string, List<string>> currentGames = new Dictionary<string, List<string>>();

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = users[Context.ConnectionId];
            users.Remove(username);
            var gameID = currentGames.Where(x => x.Value.Contains(username)).FirstOrDefault().Key;
            if (gameID != default)
            {
                await Clients.Group(gameID).SendAsync("GameChatMessage", username + " has left the game ");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameID);
                currentGames[gameID].Remove(username);
                if (currentGames[gameID].Count == 0)
                {
                    currentGames.Remove(gameID);
                }
            }
        }

        public async Task Register(string username, string gameID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
            if (!users.ContainsValue(username))
            {
                users.Add(Context.ConnectionId, username);
                if (currentGames.TryGetValue(gameID, out var currentGame))
                {
                    if (currentGame.Count < MaxUsersInGame)
                    {
                        currentGames[gameID].Add(username);
                    }
                }
                else
                {
                    currentGames.Add(gameID, new List<string> { username });
                }
                await Clients.Group(gameID).SendAsync("GameDetails", currentGames[gameID]);
                await Clients.Group(gameID).SendAsync("GameChatMessage", username + " has joined the lobby!");
            }
            await Clients.Caller.SendAsync("GameDetails", currentGames[gameID]);
        }

        public async Task SendGameChatMessage(string gameId, string username, string message)
        {
            await Clients.Group(gameId).SendAsync("GameChatMessage", username + ": " + message);
        }
    }
}
