using System.Threading.Tasks;
using datingApp.api.Models;

namespace datingApp.api.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user,string password);
         Task<User> login(string username,string password);
         Task<bool> UserExists(string username);
void createPasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt);
    }
}