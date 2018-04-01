using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCard.Blockchain
{
    public class Acess
    {
        public string acess_id { get; set; }
        public string hash { get; set; }
        public string user { get; set; }
        public string timestamp { get; set; }
        public bool is_archive { get; set; }
    }
}
