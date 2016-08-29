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
using INTRANET_CRM.Model;

namespace INTRANET_CRM
{
    public class Cierre : CodeActivity
    {

        [Output("CUENTAS")]
        public OutArgument<string> outResp2 { get; set; }

        [Output("RESPUESTA")]
        public OutArgument<int> outResp { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);
            string añoSTR = DateTime.Now.Year + "";
            string mesSTR = DateTime.Now.Month + "";

            string connectionZeus = "Data Source=ZEUS.atcom.cl; Initial Catalog=IntranetV5;User ID=zeus;Password=Atcom2012";
            SqlConnection connZeus = new SqlConnection(connectionZeus);
            connZeus.Open();
            SqlCommand fichas = new SqlCommand("GENERAR_CIERRE_MENSUAL", connZeus);
            fichas.CommandType = CommandType.StoredProcedure;
            fichas.Parameters.AddWithValue("@STR_ANNO", añoSTR);
            fichas.Parameters.AddWithValue("@STR_MES", mesSTR);
            SqlDataReader data = fichas.ExecuteReader();
            string cuentas = "";
            int nCuentas = 0;
            while (data.Read())
            {
                string rut = data.GetString(3);
                SqlConnection con2 = new SqlConnection(connectionZeus);
                con2.Open();
                Recurso recurso = new Recurso(rut, con2);
                con2.Close();
                Entity colaborador = Recurso.getColaboradorFromRutCRM(recurso.crmRut, service);
                Guid idCol;
                try
                {
                    if (colaborador != null)
                    {
                        idCol = colaborador.Id;
                        recurso.updateColaborador(colaborador, false, service);
                    }
                    else
                        idCol = service.Create(recurso.makeColaborador(service));
                }
                catch (Exception e)
                {
                    throw new Exception("COLABORADOR " + rut +
                        "-" + (colaborador != null) +
                        "-" + recurso.fechaNacimiento +
                        "-" + e.Message);
                }
                int nroFicha = (int)data.GetInt64(0);
                Entity ficha = Ficha.getFichaByNumber(nroFicha, service);
                Entity cuenta = Cuenta.getCuentaByIntranetName( service ,data.GetString(1));
                if (ficha == null && cuenta != null)
                {
                    try
                    {
                        ficha = new Entity("new_fichadetrabajo");
                        Utils.addOrUpdateParameter(ref ficha, "new_cuenta", cuenta.ToEntityReference());
                        Utils.addOrUpdateParameter(ref ficha, "new_colaborador", new EntityReference("new_colaborador", idCol));
                        switch (data.GetString(2))
                        {
                            case "( EST )": Utils.addOrUpdateParameter(ref ficha, "new_empresainterna", new OptionSetValue(100000000)); break;
                            case "( OUT )": Utils.addOrUpdateParameter(ref ficha, "new_empresainterna", new OptionSetValue(100000002)); break;
                        }

                        setFichaDates(ref ficha, data);

                        Utils.addOrUpdateParameter(ref ficha, "new_numeroficha", nroFicha);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("INICIO FICHA " + nroFicha +
                            "-I:" + data.GetValue(6).ToString() +
                            "-T:" + data.GetValue(7).ToString() +
                            "-E:" + data.GetValue(8).ToString() +
                            "-" + e.Message);
                    }
                    service.Create(ficha);

                }
                else
                {
                    if (ficha != null)
                    {
                        setFichaDates(ref ficha, data);
                        Utils.addOrUpdateParameter(ref ficha, "new_numeroficha", nroFicha);
                        service.Update(ficha);
                    }
                    if (cuenta == null)
                    {
                        cuentas += data.GetString(1) + ";";
                        nCuentas++;
                    }
                }


            }
            data.Dispose();
            connZeus.Close();
            outResp.Set(executionContext, nCuentas);
            outResp2.Set(executionContext, cuentas);

        }

        private void setFichaDates(ref Entity ficha, SqlDataReader data)
        {
            Utils.addOrUpdateParameter(ref ficha, "new_fechaingreso", data.GetDateTime(6));
            DateTime termino = data.GetDateTime(7);
            if (termino > new DateTime(1900, 1, 1))
                Utils.addOrUpdateParameter(ref ficha, "new_fechatermino", termino);
            else
                Utils.setEmptyParameter(ref ficha, "new_fechatermino");

            string egreso = data.GetValue(8).ToString();
            if (egreso != null && egreso != "" && data.GetDateTime(8) > new DateTime(1900, 1, 1))
                Utils.addOrUpdateParameter(ref ficha, "new_fechaegreso", data.GetDateTime(8));
            else
                Utils.setEmptyParameter(ref ficha, "new_fechaegreso");
            Utils.addOrUpdateParameter(ref ficha, "new_fechasincronizacion", DateTime.Now);


        }
    }
}
