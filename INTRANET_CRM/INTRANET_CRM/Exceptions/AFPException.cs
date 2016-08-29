using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INTRANET_CRM.IntranetNumeraciones
{
    public class AFPException : Exception
    {
        public AFPException(string message) : base(message) { }
    }
}
