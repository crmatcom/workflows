using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM.Model
{
    public class Ficha
    {
        public static Entity getFichaByNumber(int nro, IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("new_fichadetrabajo")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("new_numeroficha", ConditionOperator.Equal, nro),
                         new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                     }
            });

            EntityCollection fichas = service.RetrieveMultiple(query);
            if (fichas.Entities.Count > 0)
                return fichas.Entities[0];
            //Check if the relationship was not found
            return null;
        }

        public static EntityCollection getFichasAnuladas(IOrganizationService service)
        {

            DateTime now = DateTime.Now.AddHours(3);
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            QueryExpression query = new QueryExpression("new_fichadetrabajo")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.FilterOperator = LogicalOperator.And;
            query.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.And,
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                         new ConditionExpression("new_fechasincronizacion", ConditionOperator.LessThan, today)
                }

            });
            query.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Conditions =
                {
                    new ConditionExpression("new_fechatermino", ConditionOperator.GreaterEqual, today),
                    new ConditionExpression("new_fechatermino", ConditionOperator.Null)
                }
            });
            query.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Conditions =
                {
                    new ConditionExpression("new_fechaegreso", ConditionOperator.GreaterEqual, today),
                    new ConditionExpression("new_fechaegreso", ConditionOperator.Null)
                }
            });

            return service.RetrieveMultiple(query);

        }

        internal static Entity getDocByNumber(Entity ficha, int p, IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("new_documento")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("new_ficha", ConditionOperator.Equal, ficha.Id),
                         new ConditionExpression("new_idintranet", ConditionOperator.Equal, p),
                         new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                     }
            });

            EntityCollection fichas = service.RetrieveMultiple(query);
            if (fichas.Entities.Count > 0)
                return fichas.Entities[0];
            //Check if the relationship was not found
            return null;
        }
    }
}
