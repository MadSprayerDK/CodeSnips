using System.Threading.Tasks;
using ManoSoftware.SpotifyLocalApi.Model;

namespace ManoSoftware.SpotifyLocalApi
{
    public interface ISpotifyLocalApi
    {
        Task<bool> Init();
        Task<Status> Status();
        Task<Status> Play(string uri);
        Task<Status> Pause();
        Task<Status> Resume();
    }
}
