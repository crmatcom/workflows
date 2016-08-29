using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM.Model
{
    public class Cuenta
    {
        public static Entity getCuentaByIntranetName(IOrganizationService service, string name)
        {
            QueryExpression query = new QueryExpression("account")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("new_nombreintranet", ConditionOperator.Equal, name),
                         new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                     }
            });

            EntityCollection cuentas = service.RetrieveMultiple(query);
            if (cuentas.Entities.Count == 1)
                return cuentas.Entities[0];
            //Check if the relationship was not found
            return null;
        }
    }
}
