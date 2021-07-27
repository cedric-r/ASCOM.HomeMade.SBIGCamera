using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade.SBIGCommon
{
    public static class Utils
    {
        public static bool IsBitSet(long b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        public static string DisplayException(Exception e)
        {
            string temp = "";
            if (e.InnerException != null)
                temp += DisplayException(e.InnerException) + "-----\n";
            temp += e.Message + "\n" + e.StackTrace + "\n";
            return temp;
        }

    }
}
