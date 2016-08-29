using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Query;
using ATCOM_jtoro;

namespace Workflow3
{
    public class ColaboradorEnCuenta : CodeActivity
    {

        [Input("EntityReference colaborador")]
        [ReferenceTarget("new_colaborador")]
        public InArgument<EntityReference> colaborador { get; set; }

        [Input("EntityReference cuenta")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> cuenta { get; set; }        

        protected override void Execute(CodeActivityContext executionContext)
        {

            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
            EntityReference colaboradorRef = colaborador.Get(executionContext);
            EntityReference cuentaRef= cuenta.Get(executionContext);

            // Creating EntityReferenceCollection for the Contact
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();

            // Add the related entity contact
            relatedEntities.Add(colaboradorRef);

            // Add the Account Contact relationship schema name
            Relationship relationship = new Relationship("new_new_colaborador_account");

            // Associate the contact record to Account
            //if(!Utils.areAsociated(service,"new_new_colaborador_account",colaboradorRef.Id.ToString(), "new_colaboradorid",cuentaRef.Id.ToString(), "accountid"))
            var result = Utils.areAsociated(service, "new_new_colaborador_account",
                 cuentaRef.Id.ToString(), "accountid",
                 colaboradorRef.Id.ToString(), "new_colaboradorid");
            if (result == null || result.Entities == null || result.Entities.Count < 1)
                service.Associate(cuentaRef.LogicalName, cuentaRef.Id, relationship, relatedEntities);

        }
    }
}