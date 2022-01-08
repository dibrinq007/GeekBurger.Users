using GeekBurger.Users.Model;
using GeekBurger.Users.Repository;
using System;
using System.Collections.Generic;

namespace GeekBurger.Users.Extensions
{
    public static class UsersContextExtensions
    {
        public static void Seed(this UsersDbContext context)
        {            
            context.Users.RemoveRange(context.Users);

            context.Users.AddRange(
                new List<User> {
                    new User { UserId = new Guid("8d618778-85d7-411e-878b-846a8eef30c0") ,
                                    Face = "Alex",                                    
                                    AreRestrictionsSet = false                                     
                    },
                    new User { UserId = new Guid("8d618778-85d7-411e-878b-846a8eef30c1") ,
                                    Face = "Cesar",
                                    AreRestrictionsSet = true,
                                    Restrictions = "Ovos"
                    },
                    new User { UserId = new Guid("8d618778-85d7-411e-878b-846a8eef30c2") ,
                                    Face = "Diego",
                                    AreRestrictionsSet = true,
                                    Restrictions = "Gluten"
                    },
                    new User { UserId = new Guid("8d618778-85d7-411e-878b-846a8eef30c3"),
                                    Face = "Lilian",
                                    AreRestrictionsSet = true,
                                    Restrictions = "Leite"
                    },
                    new User { UserId = new Guid("8d618778-85d7-411e-878b-846a8eef30c4"),
                                    Face = "Mel",
                                    AreRestrictionsSet = false
                    }

        });

            context.SaveChanges();            
        }
    }
}
