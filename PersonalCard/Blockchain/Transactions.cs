using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCard.Blockchain
{
    public class Transactions
    {
		public int TransactionsId { get; set; }
        public string original_wallet { get; set; }
        public string destination_wallet { get; set; }
        public string info { get; set; }
        public string timestamp { get; set; }
        public string is_complite { get; set; }
    }
}
