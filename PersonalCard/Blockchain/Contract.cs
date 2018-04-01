using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCard.Blockchain
{
    public class Contract
    {
        public string contractID;
        public string hash_сustomer;
        public string hash_еxecutor;
        public float order_sum;
        public string condition;
        public string prepaid_expense;
        public bool is_Done;
        public bool is_freze;
    }
}
