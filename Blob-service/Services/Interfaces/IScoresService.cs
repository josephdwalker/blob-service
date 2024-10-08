using Blob_service.Models;

namespace Blob_service.Services.Interfaces
{
    public interface IScoresService
    {
        void StartGame(GameDetails gameDetails);

        CumulativeScores[] GetScores(string gameID);

        Scores GetCurrentScore(string gameID);

        int TrickEnded(string gameID, string?[] played, char leadingSuit);
    }
}