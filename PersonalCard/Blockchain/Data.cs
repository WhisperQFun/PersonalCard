using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCard.Blockchain
{
    public class Medical
    {
        public int medId { get; set; }
        public string diagnosis { get; set; }
        public string diagnosis_fully { get; set; }
        public string first_aid { get; set; }
        public string drugs { get; set; }
        public bool is_important { get; set; }
    }
}
