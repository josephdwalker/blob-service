namespace Blob_service.Services.Interfaces
{
    public interface IBidsService
    {
        int?[] GetBids(int gameID);
        int GetTricks(int gameID);
        void SetBids(int gameID, int player, int?[] bidList);
    }
}