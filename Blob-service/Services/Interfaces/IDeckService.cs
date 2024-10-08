using Blob_service.Models;

namespace Blob_service.Services.Interfaces
{
    public interface IDeckService
    {
        string?[] GetHand(string gameID, int player);

        ActiveHand? GetActiveHand(string gameID);

        ActiveHand[] GetActiveHands(string gameID);

        void PlayCard(string gameID, int player, bool leadingCard, string card);
    }
}