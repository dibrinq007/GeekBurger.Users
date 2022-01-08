using GeekBurger.Users.Model;
using System;
using System.Collections.Generic;

namespace GeekBurger.Users.Repository
{
    public interface IUsersRepository
    {
        User GetUserById(Guid idUser);

        User GetUserByFace(string faceUser);

        List<User> GetUsers();

        bool Add(User user);

        bool Update(User user);

        void Delete(User user);

        void Save();
    }
}
