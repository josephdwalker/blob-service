using Blob_service.Models;

namespace Blob_service.Services.Interfaces
{
    public interface IScoresService
    {
        void StartGame(GameDetails gameDetails);

        public CumulativeScores[] GetScores(int gameID);

        int TrickEnded(int gameID, string?[] played, char leadingSuit);
    }
}