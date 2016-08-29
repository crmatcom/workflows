using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INTRANET_CRM.IntranetNumeraciones
{
    class EstadoCivilException : Exception
    {
        public EstadoCivilException(string message) : base(message) { }
    }
}
