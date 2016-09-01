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
    public class IngresosMañanaSupervisor : CodeActivity
    {
        [Input("Supervisor")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> user { get; set; }

        [Output("Respuesta")]
        [ReferenceTarget("new_juego")]
        public OutArgument<string> outArg { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference userRef = user.Get(executionContext);
            EntityCollection res = Utils.getTomorrowsResources(service, userRef);
            String ans = "";
            String account = "";

            foreach (var r in res.Entities)
            {
                Entity c = service.Retrieve("new_colaborador", ((EntityReference)r["new_colaborador"]).Id, new ColumnSet(true));
                if (account != c["new_cuenta"]+"" )
                {
                    account = c["new_cuenta"] + "";
                    ans += "En cuenta: " + account + " \n";
                }
               
                ans += c["new_apellidopaterno"] + " " + c["new_apellidomaterno"] + " " + c["new_name"] + "\n";
            }

            if (ans != "")
                outArg.Set(executionContext, ans);
            else
                outArg.Set(executionContext, null);
        }

    }
}
