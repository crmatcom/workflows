using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATCOM_jtoro
{
    public class RecursoException : Exception
    {
        public RecursoException(string message) : base(message) { }
    }
}
