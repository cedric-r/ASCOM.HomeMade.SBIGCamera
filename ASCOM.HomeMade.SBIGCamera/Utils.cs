using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.HomeMade
{
    public static class Utils
    {
        public static bool IsBitSet(long b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }
    }
}
