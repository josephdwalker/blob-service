using Microsoft.AspNetCore.SignalR;

namespace Blob_service.Hubs
{
    public class GameHub : Hub
    {
        private const int MaxUsersInGame = 6;
        public static Dictionary<string, string> users = new Dictionary<string, string>();
        public static Dictionary<string, string> availableGames = new Dictionary<string, string>();
        public static Dictionary<int, List<string>> currentGames = new Dictionary<int, List<string>>();

        public async override Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("AvailableGames", availableGames.Keys);
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = users[Context.ConnectionId];
            await Clients.All.SendAsync("LobbyChatMessage", username + " has left the lobby");
            users.Remove(username);
            var group = currentGames.Where(x => x.Value.Contains(username)).FirstOrDefault().Key;
            if (group != default)
            {
                await LeaveGame(group, username);
            }
            var removeKey = availableGames.Where(x => x.Value == Context.ConnectionId).FirstOrDefault().Key;
            if (removeKey != null)
            {
                availableGames.Remove(removeKey);
            }
            await Clients.All.SendAsync("AvailableGames", availableGames.Keys);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Register(string username)
        {
            if (!users.ContainsValue(username))
            {
                users.Add(Context.ConnectionId, username);
                await Clients.All.SendAsync("LobbyChatMessage", username + " has joined the lobby!");
            }
        }

        public async Task SendChatMessage(string username, string message)
        {
            await Clients.All.SendAsync("LobbyChatMessage", username + ": " + message);
        }

        public async Task CreateGame(string username)
        {
            var gameId = GetGameId(username);
            availableGames.Add(username, Context.ConnectionId);
            currentGames.Add(gameId, new List<string> { username });
            await Clients.Caller.SendAsync("GameDetails", gameId, currentGames[gameId]);
            await Clients.All.SendAsync("AvailableGames", availableGames.Keys);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
        }

        public async Task JoinGame(string opponentUsername, string username)
        {
            var gameId = GetGameId(opponentUsername);
            if (currentGames[gameId].Count < MaxUsersInGame)
            {
                currentGames[gameId].Add(username);
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
                await Clients.Group(gameId.ToString()).SendAsync("GameDetails", gameId, currentGames[gameId]);
                await Clients.Group(gameId.ToString()).SendAsync("GameChatMessage", username + " has joined the game!");
                if (currentGames[gameId].Count >= MaxUsersInGame)
                {
                    availableGames.Remove(opponentUsername);
                    await Clients.All.SendAsync("AvailableGames", availableGames.Keys);
                }
            }
        }

        public async Task LeaveGame(int gameId, string username)
        {
            await Clients.Group(gameId.ToString()).SendAsync("GameChatMessage", username + " has left the game ");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
            currentGames[gameId].Remove(username);
            if (currentGames[gameId].Count == 0)
            {
                currentGames.Remove(gameId);
            }
        }

        public async Task SendGameChatMessage(int gameId, string username, string message)
        {
            await Clients.Group(gameId.ToString()).SendAsync("GameChatMessage", username + ": " + message);
        }

        private static int GetGameId(string username)
        {
            return Math.Abs(username.GetHashCode());
        }
    }
}
