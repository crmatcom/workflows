using INTRANET_CRM.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM
{
    public class SincronizacionDocumentos : CodeActivity
    {

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            string connectionZeus = "Data Source=ZEUS.atcom.cl; Initial Catalog=IntranetV5;User ID=zeus;Password=Atcom2012";
            SqlConnection connZeus = new SqlConnection(connectionZeus);
            connZeus.Open();
            //String resp = "";

            SqlCommand estadoFicha = new SqlCommand("SELECT ID_PM_CONTRATO_ID " +
                ",ID_FICHA " +
                ",TIPO_DOC " +
                ",NUMERO_EXTENSION " +
                ",FECHA_GENERACION " +
                ",FECHA_ANULACION " +
                ",FECHA_INICIO " +
                "FROM [IntranetV5].[dbo].[PM_CONTRATOS] " +
                "WHERE NOT (TIPO_DOC IN ('CPD', 'CDT') AND NUMERO_EXTENSION > 0) " +
                "AND FECHA_GENERACION > DATEADD(DAY, -2, GETDATE())",
                connZeus);
            SqlDataReader data = estadoFicha.ExecuteReader();
            while (data.Read())
            {
                Entity ficha = null;
                try
                {
                    ficha = Ficha.getFichaByNumber((int)data.GetInt64(1), service);
                }
                catch (Exception e)
                {
                    throw new Exception("ID_FICHA: " + data.GetValue(1).ToString() + ";" + e.Message);
                }
                if (ficha != null)
                {

                    Entity doc = null;
                    try
                    {
                        doc = Ficha.getDocByNumber(ficha, (int)data.GetInt64(0), service);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("ID_DOC: " + data.GetValue(1).ToString() + ";" + e.Message);
                    }
                    Guid id;
                    if (doc == null)
                    {
                        doc = new Entity("new_documento");
                        Utils.addOrUpdateParameter(ref doc, "new_idintranet", (int)data.GetInt64(0));
                        Utils.addOrUpdateParameter(ref doc, "new_ficha", ficha.ToEntityReference());
                        //if(doc.Contains("new_ficha"))
                        //  throw new Exception("ERROR EN FICHA: " + doc.Contains("new_ficha") + "-" + doc["new_ficha"]);
                        addDocType(ref doc, data.GetString(2));
                        try
                        {
                            Utils.addOrUpdateParameter(ref doc, "new_nanexo", Int32.Parse(data.GetString(3)));
                        }
                        catch (Exception e)
                        {
                            throw new Exception("ID_TIPO: " + data.GetValue(1).ToString() + ";" + e.Message);
                        }
                        Utils.addOrUpdateParameter(ref doc, "new_fechageneracion", data.GetDateTime(4));
                        Utils.addOrUpdateParameter(ref doc, "new_fechainicio", data.GetDateTime(6));
                        if (!doc.Contains("new_ficha"))
                            throw new Exception("FICHA NO AGREGADA");
                        id = service.Create(doc);
                        Entity e2 = service.Retrieve("new_documento", id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                        if (!doc.Contains("new_ficha"))
                            throw new Exception("FICHA NO AGREGADA2");
                    }
                    else
                    {
                        id = doc.Id;
                        try
                        {
                            Utils.addOrUpdateParameter(ref doc, "new_nanexo", Int32.Parse(data.GetString(3)));
                        }
                        catch (Exception e)
                        {
                            throw new Exception("ID_TIPO: " + data.GetValue(1).ToString() + ";" + e.Message);
                        }
                        Utils.addOrUpdateParameter(ref doc, "new_fechageneracion", data.GetDateTime(4));
                        Utils.addOrUpdateParameter(ref doc, "new_fechainicio", data.GetDateTime(6));
                        service.Update(doc);
                    }

                    if (data.GetValue(5) == null || data.GetValue(5).ToString() == "" || data.GetDateTime(5) <= new DateTime(1900, 0, 0))
                    {
                        SetStateRequest setStateReq = new SetStateRequest();
                        setStateReq.EntityMoniker = new EntityReference("new_colaborador", id);
                        setStateReq.State = new OptionSetValue(1);
                        setStateReq.Status = new OptionSetValue(2);
                    }

                }
            }

            data.Dispose();
            data.Close();
            connZeus.Close();

        }

        private void addDocType(ref Entity doc, string p)
        {
            switch (p)
            {
                case "CPD": Utils.addOrUpdateParameter(ref doc, "new_tipodedocumento", new OptionSetValue(100000007)); break;
                case "CDT": Utils.addOrUpdateParameter(ref doc, "new_tipodedocumento", new OptionSetValue(100000001)); break;
                case "CDT_EXT": Utils.addOrUpdateParameter(ref doc, "new_tipodedocumento", new OptionSetValue(100000000)); break;
                case "CPD_EXT": Utils.addOrUpdateParameter(ref doc, "new_tipodedocumento", new OptionSetValue(100000008)); break;

            }
        }

    }
}
