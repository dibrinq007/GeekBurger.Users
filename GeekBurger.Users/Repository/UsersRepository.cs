using GeekBurger.Users.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeekBurger.Users.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private UsersDbContext _context;

        public UsersRepository(UsersDbContext context)
        {
            _context = context;
        }

        public bool Add(User user)
        {
            _context.Users.Add(user);
            return true;
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);      
        }

        public User GetUserByFace(string faceUser)
        {
            var user = _context.Users?.Where(x => x.Face.Equals(faceUser, StringComparison.InvariantCultureIgnoreCase))
                       .FirstOrDefault();

            return user;
        }

        public User GetUserById(Guid idUser)
        {
            var user = _context.Users?.Where(x => x.UserId == idUser).FirstOrDefault();

            return user;
        }

        public List<User> GetUsers()
        {
            var user = _context.Users?.ToList();

            return user;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Update(User user)
        {
            _context.Users.Update(user);
            return true;
        }
    }
}
