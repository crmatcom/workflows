using INTRANET_CRM.Recursos;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk;
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
using Microsoft.Crm.Sdk.Messages;

namespace INTRANET_CRM
{
    public class FichasNosActualizadas : CodeActivity
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
            EntityCollection fichas = Ficha.getFichasAnuladas(service);
            String resp = "";
            foreach(Entity ficha in fichas.Entities){
                if (ficha.Contains("new_numeroficha"))
                {
                    resp += ficha["new_numeroficha"] + "\n";
                    SqlCommand estadoFicha = new SqlCommand("SELECT F.ID_FICHA" +
                        ",F.ANULADA " +
                        ",E.FECHA_EGRESO " +
                        "FROM FICHA f " +
                        "LEFT JOIN EGRESO E ON f.ID_FICHA = e.ID_FICHA " +
                        "WHERE f.ID_FICHA = " + ficha["new_numeroficha"], connZeus);
                    SqlDataReader data = estadoFicha.ExecuteReader();
                    try
                    {
                        if (data.Read())
                        {
                            Entity ficha2 = ficha;
                            resp += "ANU:" + (data.GetValue(1) != null && data.GetValue(1).ToString() != "" && data.GetInt32(1) == 1) + "  EGRE:" + (data.GetValue(2) != null && data.GetValue(2).ToString() != "" && data.GetDateTime(2) > new DateTime(1900, 1, 1)) + "\n";


                            if (data.GetValue(2) != null && data.GetValue(2).ToString() != "" && data.GetDateTime(2) > new DateTime(1900, 1, 1))
                            {
                                Utils.addOrUpdateParameter(ref ficha2, "new_fechaegreso", data.GetDateTime(2));
                                service.Update(ficha2);
                            }
                            if (data.GetValue(1) != null && data.GetValue(1).ToString() != "" && data.GetInt32(1) == 1)
                            {
                                SetStateRequest setStateReq = new SetStateRequest();
                                setStateReq.EntityMoniker = new EntityReference(ficha2.LogicalName, ficha2.Id);
                                setStateReq.State = new OptionSetValue(1);
                                setStateReq.Status = new OptionSetValue(2);

                                SetStateResponse response = (SetStateResponse)service.Execute(setStateReq);
                            }




                        }
                        else
                        {
                            resp += "NO DATA\n";
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("FICHA:" + ficha["new_numeroficha"] + e.Message);
                    }
                    
                    data.Dispose();
                    data.Close();
                }
            }

            connZeus.Close();
        }

    }
}
