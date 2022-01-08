using System;
using System.ComponentModel.DataAnnotations;

namespace GeekBurger.Users.Model
{
    public class User 
    {
        [Key]
        public Guid UserId { get; set; }
        public string Face { get; set; }           
        public string Restrictions { get; set; }
        public bool AreRestrictionsSet { get; set; }
    }
}
