using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Assignment
{
    class Utils
    {

        public static bool addOrUpdateParameter(ref Entity e, string parameterName, Object o)
        {
            if (e.Contains(parameterName))
            {
                bool resp = e[parameterName] != o;
                e[parameterName] = o;
                return resp;
            }
            else
            {
                e.Attributes.Add(parameterName, o);
                return true;
            }
        }
    }
}
