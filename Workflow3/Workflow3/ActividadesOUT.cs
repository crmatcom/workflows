using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using ATCOM_jtoro;

namespace Workflow1
{
    public class ActividadesOUT : CodeActivity
    {

        [Input("Unidad de Negocio")]
        [ReferenceTarget("businessunit")]
        public InArgument<EntityReference> unidad { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference unidadRef = unidad.Get(executionContext);

            QueryExpression query = new QueryExpression("activitypointer")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.LinkEntities.Add(new LinkEntity("activitypointer", "systemuser", "createdby", "systemuserid", JoinOperator.Inner));
            query.LinkEntities[0].EntityAlias = "USER";
            FilterExpression fe = new FilterExpression();
            fe.AddCondition("isregularacivity", ConditionOperator.Equal, 1);
            fe.AddCondition("isworkflowcreated", ConditionOperator.Equal, 0);
            fe.AddCondition("USER", "businessunitid", ConditionOperator.Equal, unidadRef.Id);

            FilterExpression fe2 = new FilterExpression();
            fe2.FilterOperator = LogicalOperator.Or;
            fe2.AddCondition("statecode", ConditionOperator.Equal, 0);
            fe2.AddFilter(new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions =
                    {
                        new ConditionExpression("actualend", ConditionOperator.Last7Days),
                        new ConditionExpression("statecode", ConditionOperator.Equal, 1)
                    }
                });
            fe.AddFilter(fe2);

            var result = service.RetrieveMultiple(query);

        }

    }
}
