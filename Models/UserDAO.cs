using System.Collections.Generic;
using System.Linq;

namespace PharmacityStore.Models
{
    public class UserDAO
    {
        private readonly PharmacityContext _context;

        public UserDAO(PharmacityContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }
    }
}
