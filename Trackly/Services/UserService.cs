using Trackly.Models;

namespace Trackly.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public void CreateUser(UserModel user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public UserModel GetUser(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public UserModel GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public UserModel Login(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }
    }
}
