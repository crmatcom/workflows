using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ATCOM_jtoro
{
    public class prueba2 : CodeActivity
    {

        [Input("COLABORADOR")]
        [ReferenceTarget("new_colaborador")]
        public InArgument<EntityReference> colaboradorID { get; set; }

        [Output("CODIGO DE RESPUESTA")]
        public OutArgument<int> outResp { get; set; }

        /*
         *-1 no apellidos
         *-2 no espacios
         *-3 mas de un espacio (apellidos compuestos)
         *-4 apellidos ya separados
        */
        protected override void Execute(CodeActivityContext executionContext)
        {

            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
            Entity colaborador = service.Retrieve("new_colaborador", colaboradorID.Get(executionContext).Id, new ColumnSet(true));
            if (colaborador.Contains("new_apellidos"))
            {
                string apellidos = (string)colaborador["new_apellidos"];
                if (!apellidos.Contains(" "))
                {
                    outResp.Set(executionContext, -2);
                    return;
                }
                if (Regex.Matches(apellidos, " ").Count > 1)
                {
                    outResp.Set(executionContext, -3);
                    return;
                }
                if (colaborador.Contains("new_apellidopaterno"))
                {
                    outResp.Set(executionContext, -4);
                    return;
                }
                string[] ambos_apellidos = apellidos.Split(' ');
                colaborador.Attributes.Add("new_apellidopaterno", ambos_apellidos[0]);
                colaborador.Attributes.Add("new_apellidomaterno", ambos_apellidos[1]);
                service.Update(colaborador);
            }

            outResp.Set(executionContext, 0);
        }

        private Entity getCRMRecursoFromRUT(string rut, IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("new_colaborador")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                    {
                        //Get the row for the relationship where the account and contact are the account and contact passed in
                        new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                        new ConditionExpression("new_rut", ConditionOperator.Equal, rut)
                    }
            });
            EntityCollection result = service.RetrieveMultiple(query);
            if (result != null || result.Entities == null || result.Entities.Count == 0)
                throw new RecursoException("NO EXISTE COLABORADOR ACTIVO CON RUT " + rut);
            if (result.Entities.Count > 1)
                throw new RecursoException("EXISTEN VARIOS RECURSOS CON RUT " + rut);
            
           return result.Entities[0];

            
        }

        public static string crmRUT(string intraRUT)
        {
            return intraRUT.Substring(0, intraRUT.Length - 2) + "-" + intraRUT.Substring(intraRUT.Length - 1);
        }

        
    }
}
