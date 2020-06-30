using System.Threading.Tasks;

namespace ClientAppAuthenticated
{
    public interface IMySessionStorage
    {
        Task<string> GetStringAsync(string key);

        Task<string> GetLocalStringAsync(string key);
    }
}