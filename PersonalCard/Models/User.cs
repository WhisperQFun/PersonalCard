using System.Collections.Generic;

namespace PersonalCard.Models
{
    public class User 
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Hash { get; set; }
        public string type_of_bloud { get; set; }
        public bool is_donor { get; set; }
        public float balance { get; set; }
        public string token { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
