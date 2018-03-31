using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
