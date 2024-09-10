using Blob_service.Data;
using Blob_service.Models;
using Blob_service.Services.Interfaces;

namespace Blob_service.Services
{
    public class GameDetailsService : IGameDetailsService
    {
        private readonly DeckDbContext _db;

        public GameDetailsService(DeckDbContext db)
        {
            _db = db;
        }

        public GameDetails GetDetails(int gameID)
        {
            var details = _db.GameDetails.Where(x => x.GameID == gameID).First();
            return details;
        }
    }
}
