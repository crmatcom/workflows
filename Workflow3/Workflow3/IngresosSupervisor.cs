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
    public class IngresosSupervisor : CodeActivity
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

            EntityCollection result = Utils.getPorIngresarMañanaSupervisor(service, userRef);
            String respuesta = "";

            //cuentas->oportunidad->colaborador
            Dictionary<Guid, Entity> cuentas = new Dictionary<Guid,Entity>();
            Dictionary<Guid, Entity> oportunidades = new Dictionary<Guid,Entity>();
            Dictionary<Guid, Dictionary<Guid, List<Entity>>> colaboradores = new Dictionary<Guid, Dictionary<Guid, List<Entity>>>();

            foreach (var r in result.Entities)
            {
                Entity colaborador = service.Retrieve("new_colaborador", ((EntityReference)r["new_colaborador"]).Id, new ColumnSet(true));
                EntityReference oportunidadRef = (EntityReference)r["new_oportunidad"];
                EntityReference cuentaRef = (EntityReference)r["new_cuenta"];
                Entity cuenta = getCuenta(cuentaRef, ref cuentas, service);
                Entity oportunidad = getOportunidad(oportunidadRef, ref oportunidades, service);
                addColaborador(ref colaboradores, cuenta.Id, oportunidad.Id, colaborador, service);
            }

            foreach (KeyValuePair<Guid, Entity> cuenta in cuentas)
            {
                respuesta += cuenta.Value["name"] + ":\n";
                foreach (KeyValuePair<Guid, List<Entity>> oportunidadId in colaboradores[cuenta.Key])
                {
                    Entity oportunidad = oportunidades[oportunidadId.Key];
                    respuesta += "  Oportunidad " + oportunidad["name"] + " (";
                    try
                    {
                        if (oportunidad.Contains("new_sedeberecibiralrecurso") && (bool)oportunidad["new_sedeberecibiralrecurso"])
                            respuesta += "Se debe recibir al recurso,";
                        if (oportunidad.Contains("new_zapato") && (bool)oportunidad["new_zapato"])
                            respuesta += "Se necesita llevar zapatos,";
                        if (oportunidad.Contains("new_pantalon") && (bool)oportunidad["new_pantalon"])
                            respuesta += "Se necesita llevar pantalon,";
                        if (oportunidad.Contains("new_polera") && (bool)oportunidad["new_polera"])
                            respuesta += "Se necesita llevar polera,";
                        if (oportunidad.Contains("new_epp") && (bool)oportunidad["new_epp"])
                            respuesta += "Se debe entregar EPP,";
                    }
                    catch
                    {
                        throw new Exception("Opotunidad: rec:" + oportunidad.Contains("new_sedeberecibiralrecurso") +
                            ", zapato:" + oportunidad.Contains("new_zapato") +
                            ", pantalon:" + oportunidad.Contains("new_pantalon") +
                            ", polera:" + oportunidad.Contains("new_polera"));

                    }
                    respuesta += "):\n";

                    foreach (Entity col in oportunidadId.Value)
                    {
                        try
                        {
                            respuesta += " - " + col["new_rut"] + " " + col["new_nombrecompleto"];
                            if (oportunidad.Contains("new_zapato") && (bool)oportunidad["new_zapato"] && col.Contains("new_tallazapato"))
                                respuesta += ", Zapatos talla:" + Tallas.getTallaZapato(((OptionSetValue)col["new_tallazapato"]).Value);
                            if (oportunidad.Contains("new_pantalon") && (bool)oportunidad["new_pantalon"] && col.Contains("new_tallapantaln"))
                                respuesta += ", Pantalón talla: " + Tallas.getTallaPantalon(((OptionSetValue)col["new_tallapantaln"]).Value);
                            if (oportunidad.Contains("new_polera") && (bool)oportunidad["new_polera"] && col.Contains("new_tallapolera"))
                                respuesta += ", Talla polera:" + Tallas.getTallaPolera(((OptionSetValue)col["new_tallapolera"]).Value);
                            respuesta += "\n";
                        }
                        catch
                        {
                            throw new Exception("oportunidad: rec:" + oportunidad.Contains("new_sedeberecibiralrecurso") +
                            ", zapato:" + oportunidad.Contains("new_zapato") +
                            ", pantalon:" + oportunidad.Contains("new_pantalon") +
                            ", polera:" + oportunidad.Contains("new_polera") +
                            ",XX COLABORADOR rut:" + col.Contains("new_rut") +
                            ", nombre:" + col.Contains("new_nombrecompleto") +
                            ", zapato:" + col.Contains("new_tallazapato") +
                            ", pantalon:" + col.Contains("new_tallazapato") +
                            ", polera:" + col.Contains("new_tallapolera"));
                        }

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

        private void addColaborador(ref Dictionary<Guid, Dictionary<Guid, List<Entity>>> colaboradores, Guid cuentaId, Guid oportunidadId, Entity colaborador, IOrganizationService service)
        {
            if (!colaboradores.ContainsKey(cuentaId))
            {
                List<Entity> colTemp = new List<Entity>();
                colTemp.Add(colaborador);
                Dictionary<Guid, List<Entity>> opoTemp = new Dictionary<Guid, List<Entity>>();
                opoTemp.Add(oportunidadId, colTemp);
                colaboradores.Add(cuentaId, opoTemp);
            }
            else
            {
                if (!colaboradores[cuentaId].ContainsKey(oportunidadId))
                {
                    List<Entity> colTemp = new List<Entity>();
                    colTemp.Add(colaborador);
                    colaboradores[cuentaId].Add(oportunidadId, colTemp);
                }
                else
                    colaboradores[cuentaId][oportunidadId].Add(colaborador);
            }

        }

        private Entity getOportunidad(EntityReference oportunidadRef, ref Dictionary<Guid, Entity> oportunidades, IOrganizationService service)
        {
            if (!oportunidades.ContainsKey(oportunidadRef.Id))
            {
                Entity oportunidad = service.Retrieve("opportunity", oportunidadRef.Id, new ColumnSet(true));
                oportunidades.Add(oportunidad.Id, oportunidad);
                return oportunidad;
            }
            else
                return oportunidades[oportunidadRef.Id];
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
