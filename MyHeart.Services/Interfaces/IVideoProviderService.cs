using System.Threading.Tasks;

namespace MyHeart.Services.Interfaces
{
    public interface IVideoProviderService
    {
        Task<RoomDetail> ConnectToRoom(int questionId);
    }
}