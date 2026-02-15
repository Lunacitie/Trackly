using Trackly.Models;

namespace Trackly.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwt;
        public UserService(AppDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public (UserModel user, string token)? Register(string username, string email, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
                return null;

            if (_context.Users.Any(u => u.Email == email))
                return null;

            var user = new UserModel
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var token = _jwt.GenerateToken(user);
            return (user, token);
        }

        public UserModel GetUser(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public UserModel GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public (UserModel user, string token)? Login(string usernameOrEmail, string password)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var token = _jwt.GenerateToken(user);
            return (user, token);
        }
    }
}
