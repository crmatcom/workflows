using INTRANET_CRM.Recursos;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using INTRANET_CRM.IntranetNumeraciones;
using System.Data;

namespace INTRANET_CRM
{
    public class PruebaIntranet : CodeActivity
    {

        [Input("OPORTUNIDAD")]
        [ReferenceTarget("opportunity")]
        public InArgument<EntityReference> opID { get; set; }

        [Input("Fecha Ingreso")]
        public InArgument<DateTime> fechaID { get; set; }

        [Output("RESPUESTA")]
        public OutArgument<int> outResp { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference opRef = opID.Get(executionContext);
            DateTime ingreso = fechaID.Get(executionContext);
            EntityCollection ceos = Utils.getCEOByOpportunity(opRef, service);
            int resp = 0;
            foreach (Entity e in ceos.Entities)
            {
                Entity ceo = e;
                Utils.addOrUpdateParameter(ref ceo, "new_fechaingreso", ingreso);
                service.Update(ceo);
                resp++;
            }


            outResp.Set(executionContext, resp);

        }
    }
}
