using Blob_service.Models;

namespace Blob_service.Services.Interfaces
{
    public interface IDeckService
    {
        string?[] GetHand(int gameID, int player);
        ActiveHand GetActiveHand(int gameID);
        void PlayCard(int gameID, int player, bool leadingCard, string card);
    }
}