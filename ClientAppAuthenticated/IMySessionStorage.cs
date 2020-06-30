using System.Threading.Tasks;

namespace ClientAppAuthenticated
{
    public interface IMySessionStorage
    {
        Task<string> GetStringAsync(string key);
    }
}