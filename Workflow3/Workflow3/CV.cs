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
    public class CV : CodeActivity
    {

        [Input("Colaborador")]
        [ReferenceTarget("new_colaborador")]
        public InArgument<EntityReference> colaborador { get; set; }

        [Input("Nota")]
        [ReferenceTarget("annotation")]
        public InArgument<EntityReference> nota { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference colRef = colaborador.Get(executionContext);
            EntityReference notaRef = nota.Get(executionContext);

            Entity colEntity = service.Retrieve("new_colaborador", colRef.Id, new ColumnSet(true));
            if (colEntity.Contains("new_cvid"))
                colEntity["new_cvid"] = notaRef.Id.ToString();
            else
                colEntity.Attributes.Add("new_cvid", notaRef.Id.ToString());
            service.Update(colEntity);
        }

    }
}
