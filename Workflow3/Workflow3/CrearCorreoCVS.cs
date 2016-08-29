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
    public class CrearCorreoCVS : CodeActivity
    {

        [Input("OPORTUNIDAD")]
        [ReferenceTarget("opportunity")]
        public InArgument<EntityReference> oportunidadIN { get; set; }

        [Input("EMAIL")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> emailIN { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference oportunidad = oportunidadIN.Get(executionContext);
            EntityReference email = emailIN.Get(executionContext);

            EntityCollection notas = getCVSNotas(oportunidad, service);
            foreach (Entity nota in notas.Entities)
            {
                Entity adjunto = new Entity("activitymimeattachment");
                adjunto.Attributes.Add("objectid", email);
                adjunto.Attributes.Add("objecttypecode", email.LogicalName);
                adjunto.Attributes.Add("filename", nota["filename"]);
                adjunto.Attributes.Add("mimetype", nota["mimetype"]);
                adjunto.Attributes.Add("body", nota["documentbody"]);
                service.Create(adjunto);

            }

            
        }

        private EntityCollection getCVSNotas(EntityReference oportunidad, IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("new_colaborador")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.LinkEntities.Add(new LinkEntity("new_colaborador", "new_colaboradorenoportunidad", "new_colaboradorid", "new_colaborador", JoinOperator.Inner));
            query.LinkEntities[0].EntityAlias = "COE";
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("COE","new_oportunidad", ConditionOperator.Equal, oportunidad.Id),
                         new ConditionExpression("COE","new_aprobadojefaventas", ConditionOperator.Equal, 100000000),
                         new ConditionExpression("new_cvid", ConditionOperator.NotNull)
                     }
            });

            EntityCollection colaboradores = service.RetrieveMultiple(query);
            EntityCollection notas = new EntityCollection();
            foreach(Entity col in colaboradores.Entities){
                notas.Entities.Add(service.Retrieve("annotation",new Guid((string)col["new_cvid"]), new ColumnSet(true)));
            }
            return notas;
        }

    }
}
