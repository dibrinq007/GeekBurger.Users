using System;

namespace GeekBurger.Users.Contract
{
    public class FoodRestriction
    {
        public string Restrictions { get; set; }
        public string Others { get; set; }
        public Guid UserId { get; set; }
        public int RequesterId { get; set; }
    }
}
  