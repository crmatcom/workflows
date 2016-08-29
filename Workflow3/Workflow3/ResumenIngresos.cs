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
    public class ResumenIngresos : CodeActivity
    {

        [Input("Unidad de Negocio")]
        [ReferenceTarget("businessunit")]
        public InArgument<EntityReference> unit { get; set; }

        [Output("Respuesta")]
        [ReferenceTarget("new_juego")]
        public OutArgument<string> outArg { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference unitRef = unit.Get(executionContext);

            EntityCollection result = Utils.getResumenIngresosHoy(service, unitRef);
            Entity juego = new Entity("new_juego");
            juego.Attributes.Add("new_name","prueba" + DateTime.Now);
            String asd = "";
            foreach (var r in result.Entities)
            {
                Entity colaborador = service.Retrieve("new_colaborador", ((EntityReference)r["new_colaborador"]).Id, new ColumnSet(true));
                Entity oportunidad = service.Retrieve("opportunity", ((EntityReference)r["new_oportunidad"]).Id, new ColumnSet(true));
                Entity cuenta = service.Retrieve("account", ((EntityReference)oportunidad["parentaccountid"]).Id, new ColumnSet(true));
                switch (((OptionSetValue)r["new_ingreso"]).Value)
                {
                    case 100000002: 
                        asd += "No se ha actualizado el estado de " + colaborador["new_nombrecompleto"] + " que entraría hoy a " + cuenta["name"] + ".\n"; 
                        break;
                    case 100000000: 
                        asd += colaborador["new_nombrecompleto"] + " ingreso hoy a " + cuenta["name"] + ".\n"; 
                        break;
                    case 100000001: case 100000003: case 100000004:
                        asd += colaborador["new_nombrecompleto"] + " no ingreso hoy a " + cuenta["name"] + ".\n"; 
                        break;
                }

            }
            if (asd != "")
            {
                outArg.Set(executionContext, asd);
            }
            else
                outArg.Set(executionContext, null);
            //service.Create(juego);

            
        }

    }
}
