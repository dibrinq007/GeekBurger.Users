using System;
using System.ComponentModel.DataAnnotations;

namespace GeekBurger.Users.Contract
{
    public class UserRetrieved
    {
        [Key]
        public Guid UserId { get; set; }        
        public bool AreRestrictionsSet { get; set; }        
    }
}
