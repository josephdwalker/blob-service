using Blob_service.Models;

namespace Blob_service.Services.Interfaces
{
    public interface IGameDetailsService
    {
        GameDetails GetDetails(int gameID);
    }
}
