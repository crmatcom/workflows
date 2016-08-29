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

namespace ATCOM_jtoro
{
    public class EgresosProximaSemana : CodeActivity
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

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference userRef = user.Get(executionContext);
            Entity usuario = service.Retrieve("systemuser", userRef.Id, new ColumnSet(true));
            if (!usuario.Contains("businessunitid"))
                throw new Exception("NO HAY UNIDAD DE NEGOCIO");
            EntityCollection result = Utils.getEgresosProximaSemana(service, (EntityReference)usuario["businessunitid"]);
            String respuesta = "";
            
            Dictionary<Guid, Entity> cuentas = new Dictionary<Guid, Entity>();
            //cuentas-> (ficha, colaborador)
            Dictionary<Guid, List<KeyValuePair<Entity, Entity>>> fichas = new Dictionary<Guid, List<KeyValuePair<Entity, Entity>>>();
            foreach (Entity ficha in result.Entities)
            {
                try
                {
                    EntityReference cuentaRef = (EntityReference)ficha["new_cuenta"];
                    Entity cuenta = getCuenta(cuentaRef, ref cuentas, service);
                    Entity colaborador = service.Retrieve("new_colaborador", ((EntityReference)ficha["new_colaborador"]).Id, new ColumnSet(true));
                    addFicha(ref fichas, cuenta.Id, ficha, colaborador, service);
                }
                catch
                {
                    throw new Exception("CUENTA: " + ficha.Contains("new_cuenta") +
                        " ,COLABORADOR: " + ficha.Contains("new_colaborador"));
                }
            }

            foreach (KeyValuePair<Guid, Entity> cuenta in cuentas)
            {
                respuesta += "Cliente: " + cuenta.Value["name"] + ":\n";
                respuesta += "   N° Ficha  --  Rut Recurso  -- Nombre          -- Fecha Inicio    -- Fecha Terminoo    -- Fecha Egreso \n";
                foreach (KeyValuePair<Entity, Entity> fichaColaborador in fichas[cuenta.Key])
                {
                    try
                    {
                        Entity ficha = fichaColaborador.Key;
                        Entity colaborador = fichaColaborador.Value;
                        respuesta += " - " + ficha["new_numeroficha"] + ", " +
                            colaborador["new_rut"] + ", " +
                            colaborador["new_nombrecompleto"] + ", " +
                            ficha["new_fechaingreso"] + ", ";
                        if (ficha.Contains("new_fechatermino"))
                            respuesta += ficha["new_fechatermino"] + ", ";
                        else
                            respuesta += "SIN FECHA DE TERMINO, ";
                        if (ficha.Contains("new_fechaegreso"))
                            respuesta += ficha["new_fechaegreso"] + ", ";
                        else
                            respuesta += "SIN FECHA DE EGRESO, ";
                        respuesta += "\n";
                    }
                    catch
                    {
                        throw new Exception("ERROR FICHA");
                    }
                    
                }
                respuesta += "\n\n";
            }

            if (respuesta != "")
            {
                outArg.Set(executionContext, respuesta);
            }
            else
                outArg.Set(executionContext, null);
            //service.Create(juego);


        }

        private void addFicha(ref Dictionary<Guid, List<KeyValuePair<Entity, Entity>>> fichas, Guid cuentaID, Entity ficha, Entity colaborador, IOrganizationService service)
        {
            if (fichas.ContainsKey(cuentaID))
                fichas[cuentaID].Add(new KeyValuePair<Entity, Entity>(ficha, colaborador));
            else
            {
                List<KeyValuePair<Entity, Entity>> temp = new List<KeyValuePair<Entity, Entity>>();
                temp.Add(new KeyValuePair<Entity, Entity>(ficha, colaborador));
                fichas.Add(cuentaID, temp);
            }
        }

        private Entity getCuenta(EntityReference cuentaRef, ref Dictionary<Guid, Entity> cuentas, IOrganizationService service)
        {
            if (!cuentas.ContainsKey(cuentaRef.Id))
            {
                Entity cuenta = service.Retrieve("account", cuentaRef.Id, new ColumnSet(true));
                cuentas.Add(cuenta.Id, cuenta);
                return cuenta;
            }
            else
                return cuentas[cuentaRef.Id];
        }

    }
}