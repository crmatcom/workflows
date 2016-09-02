using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace MasiveEmails
{
    public class Utils
    {
        internal static EntityCollection getTomorrowsResources(IOrganizationService service, EntityReference userRef)
        {
            QueryExpression query = new QueryExpression("new_colaboradorenoportunidad")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.LinkEntities.Add(new LinkEntity("new_colaboradorenoportunidad", "new_sucursal", "new_sucursal", "new_sucursalid", JoinOperator.Inner));
            query.LinkEntities[0].EntityAlias = "SUCURSAL";
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression("new_fechaingreso", ConditionOperator.Tomorrow),
                    new ConditionExpression("new_ingreso", ConditionOperator.Equal, 100000002),
                    new ConditionExpression("SUCURSAL", "new_supervisorId", ConditionOperator.Equal, userRef.Id)
                }
            });

            var result = service.RetrieveMultiple(query);
            return result;

            
        }
    }
}
