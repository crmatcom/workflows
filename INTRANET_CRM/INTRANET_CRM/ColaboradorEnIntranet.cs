using INTRANET_CRM.Recursos;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM
{
    public class ColaboradorEnIntranet : CodeActivity
    {
        [Input("COLABORADOR")]
        [ReferenceTarget("new_colaborador")]
        public InArgument<EntityReference> colID { get; set; }

        [Input("USUARIO")]
        [ReferenceTarget("systemuser")]
        public InArgument<EntityReference> usuarioID { get; set; }

        [Output("RESPUESTA")]
        public OutArgument<int> outResp { get; set; }


        /*
         * 0 OK,
         * -3 Usuario ya existe
         * -1 rut nulo
         * -2 rut invalido
         * -100 faltan datos colaborador
         * -101 faltan datos usuario
         */
        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference colRef = colID.Get(executionContext);
            EntityReference UsuarioRef = usuarioID.Get(executionContext);
            Entity colaborador = service.Retrieve("new_colaborador", colRef.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            Entity usuario = service.Retrieve("systemuser", UsuarioRef.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            if (!usuario.Contains("new_rut"))
            {
                outResp.Set(executionContext, -101);
                return;
            }
            Recurso recurso = new Recurso(colaborador, (string)usuario["new_rut"]);
            int ret = recurso.createInIntranet();
            outResp.Set(executionContext, ret);

        }
    }
}
