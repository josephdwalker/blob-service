using Blob_service.Data;
using Blob_service.Hubs;
using Blob_service.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Blob_service.Services
{
    public class BidsService : IBidsService
    {
        private readonly DeckDbContext _db;
        private readonly IHubContext<GameHub> _hub;

        public BidsService(DeckDbContext db, IHubContext<GameHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        public int?[] GetBids(int gameID)
        {
            var numberOfPlayers = _db.GameDetails.Where(x => x.GameID == gameID)?.FirstOrDefault()?.NumberOfPlayers;
            var bids = _db.Bids.Where(x => x.GameID == gameID)?.OrderByDescending(x => x.ID).FirstOrDefault();
            int?[] bidList = { bids?.PlayerOneBid, bids?.PlayerTwoBid, bids?.PlayerThreeBid, bids?.PlayerFourBid, bids?.PlayerFiveBid, bids?.PlayerSixBid };
            return bidList.Take(numberOfPlayers ?? 0).ToArray();
        }

        public int GetTricks(int gameID)
        {
            var tricks = _db.Scores.Where(x => x.GameID == gameID && x.PlayerOneScore != null).OrderByDescending(x => x.ID).First().Tricks;
            return tricks;
        }

        public void SetBids(int gameID, int player, int?[] bidList)
        {
            var bids = _db.Bids.Where(x => x.GameID == gameID).OrderByDescending(x => x.ID).First();

            bids.PlayerOneBid = bidList.ElementAtOrDefault(0);
            bids.PlayerTwoBid = bidList.ElementAtOrDefault(1);
            bids.PlayerThreeBid = bidList.ElementAtOrDefault(2);
            bids.PlayerFourBid = bidList.ElementAtOrDefault(3);
            bids.PlayerFiveBid = bidList.ElementAtOrDefault(4);
            bids.PlayerSixBid = bidList.ElementAtOrDefault(5);

            _db.SaveChanges();

            var numberOfPlayers = _db.GameDetails.Where(x => x.GameID == gameID).First().NumberOfPlayers;

            _hub.Clients.Group(gameID.ToString()).SendAsync("BidUpdate", bidList.Contains(null) ? (player + 1) % numberOfPlayers : -1, bidList[player], player);
            
            if (!bidList.Contains(null))
            {
                var score = _db.Scores.Where(x => x.GameID == gameID && x.PlayerOneScore != null).OrderByDescending(x => x.ID).First();
                _hub.Clients.Group(gameID.ToString()).SendAsync("CardsUpdate", (score.Round - 1) % numberOfPlayers, true);
            }
        }
    }
}
