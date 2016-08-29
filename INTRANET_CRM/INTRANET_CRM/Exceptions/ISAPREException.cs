using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INTRANET_CRM.IntranetNumeraciones
{
    public class ISAPREException : Exception
    {
        public ISAPREException(string message) : base(message) { }
    }
}
