using Blob_service.Models;

namespace Blob_service.Services.Interfaces
{
    public interface IScoresService
    {
        void StartGame(GameDetails gameDetails);

        public CumulativeScores[] GetScores(string gameID);

        int TrickEnded(string gameID, string?[] played, char leadingSuit);
    }
}