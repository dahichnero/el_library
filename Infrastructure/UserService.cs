using ElLibrary.Domain.Entities;
using ElLibrary.Domain.Services;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using System.Text;

namespace ElLibrary.Infrastructure
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> users;
        private readonly IRepository<Role> roles;

        public UserService(IRepository<User> users, IRepository<Role> roles)
        {
            this.users = users;
            this.roles = roles;
        }

        private string GetSalt() =>
            DateTime.UtcNow.ToString() + DateTime.Now.Ticks;

        private string GetSha256(string password, string salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes=SHA256.HashData(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }
        public async Task<User?> GetUserAsync(string username, string password)
        {
            username = username.Trim();
            User? user=(await users.FindWhere(u =>u.Login==username)).FirstOrDefault();
            if (user is null)
            {
                return null;
            }
            string hashPassword = GetSha256(password, user.Salt);
            if (user.Password != hashPassword)
            {
                return null;
            }
            return user;
        }

        public async Task<bool> IsUserExistsAsync(string username)
        {
            username = username.Trim();
            User? found= (await users.FindWhere(u=>u.Login == username)).FirstOrDefault();
            return found is not null;
        }

        public async Task<User> RegistrationAsync(string fullname, string username, string password)
        {
            bool userExists = await IsUserExistsAsync(username);
            if (userExists) throw new ArgumentException("Username already exists");
            Role? clientRole=(await roles.FindWhere(r=>r.Name == "client")).FirstOrDefault();
            if (clientRole is null)
                throw new InvalidOperationException("Role 'client' not found in database");
            User toAdd = new User
            {
                Fullname = fullname,
                Login = username,
                Salt = GetSalt(),
                RoleId = clientRole.Id
            };
            toAdd.Password = GetSha256(password, toAdd.Salt);
            return await users.AddAsync(toAdd);
        }
    }
}
