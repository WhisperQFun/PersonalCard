using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalCard.Blockchain
{
    public static class String2bool
    {
        public static bool convert(string convert)
        {
            if (convert == "true")
            {
                return  true;

            }
            else
            {
                return  false;
            }

        }
    }
}
