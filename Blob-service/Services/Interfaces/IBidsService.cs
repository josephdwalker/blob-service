namespace Blob_service.Services.Interfaces
{
    public interface IBidsService
    {
        int?[] GetBids(string gameID);

        void SetBids(string gameID, int player, int?[] bidList);
    }
}