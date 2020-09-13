using System.Threading.Tasks;
using datingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace datingApp.api.Data
{
    public class AuthRepository : IAuthRepository
    {
        DataContext _context;
        public AuthRepository(DataContext context)
        {
               _context = context; 
        }

        

        public async Task<User> login(string userName, string password)
        {
            var User = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
            if(User == null) return null;
            if(!verifyPasswordHash(password,User.passwordHash,User.passwordSalt))
            return null;

            return User;


        }

        private bool  verifyPasswordHash(string password,byte[] passwordHash,byte[] passwordSalt)
        {
            using(var HMAC = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                 passwordSalt = HMAC.Key;
                 var computedHash = HMAC.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));       
                  for(int i=0;i<computedHash.Length;i++)
                  {
                      if(computedHash[i] != passwordHash[i])return false;

                  }
            }   
            return true;             
        }

        public async  Task<User> Register(User user, string password)
        {
            byte[] passwordHash,passwordSalt;
            createPasswordHash(password,out passwordHash,out passwordSalt);
            user.passwordHash = passwordHash;
            user.passwordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;    
        }

        public void createPasswordHash(string password,out byte[] passwordHash,out byte[] passwordSalt)
        {
            using(var HMAC = new System.Security.Cryptography.HMACSHA512())
            {
                 passwordSalt = HMAC.Key;
                 passwordHash = HMAC.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));       
            }
        }

        

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.UserName == username))
            return true;

            return false;
        }
    }
}