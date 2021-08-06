using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SbigSharp;

namespace ASCOM.HomeMade.SBIGCommon
{
    public class SBIGRequest
    {
        public string type;
        public ushort command;
        public string parameters = "";
    }
}
