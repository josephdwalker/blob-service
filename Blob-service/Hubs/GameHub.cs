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
            await Clients.All.SendAsync("LobbyChatMessage", username + " has left the lobby");
            users.Remove(username);
            var currentGame = currentGames.Where(x => x.Value.Contains(username)).FirstOrDefault().Key;
            if (currentGame != default)
            {
                await LeaveGame(currentGame, username);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Register(string username, string gameID)
        {
            if (!users.ContainsValue(username))
            {
                users.Add(Context.ConnectionId, username);
                await Groups.AddToGroupAsync(Context.ConnectionId, gameID);
                if (currentGames.TryGetValue(gameID, out var yes))
                {
                    currentGames[gameID].Add(username);
                }
                else
                {
                    currentGames.Add(gameID, new List<string> { username });
                }
                await Clients.Group(gameID).SendAsync("GameDetails", currentGames[gameID]);
                await Clients.Group(gameID).SendAsync("GameChatMessage", username + " has joined the lobby!");
            }
        }

        public async Task LeaveGame(string gameId, string username)
        {
            await Clients.Group(gameId).SendAsync("GameChatMessage", username + " has left the game ");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            currentGames[gameId].Remove(username);
            if (currentGames[gameId].Count == 0)
            {
                currentGames.Remove(gameId);
            }
        }

        public async Task SendGameChatMessage(string gameId, string username, string message)
        {
            await Clients.Group(gameId).SendAsync("GameChatMessage", username + ": " + message);
        }
    }
}
