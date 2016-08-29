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
using System.Data.SqlClient;
using System.Data;



namespace ATCOM_jtoro
{
    public class Factura : CodeActivity
    {
        [Input("Factura")]
        [ReferenceTarget("invoice")]
        public InArgument<EntityReference> Factura { get; set; }

        
         string nombre;
         string apellido;
         string cuenta;
         string bono;
         string MES;
         string AÑO;
         string diastrabajados;
         DateTime fecha = DateTime.Now;
       


        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference invoice = Factura.Get(executionContext);
            Entity prue = service.Retrieve("invoice", invoice.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

       
            string connection = "Data Source=ZEUS.atcom.cl; Initial Catalog=IntranetV5;User ID=zeus;Password=Atcom2012";
            SqlConnection conn = new SqlConnection(connection);
            conn.Open();

            SqlCommand test = new SqlCommand("GENERAR_CIERRE_MENSUAL", conn);
            test.CommandType = CommandType.StoredProcedure;

            test.Parameters.AddWithValue("@STR_ANNO",AÑO);
            test.Parameters.AddWithValue("@STR_MES",MES);

            
              
            while ()
            {
                SqlCommand op = new SqlCommand("FICHAS_VIGENTES_DETALLE_CIERRE", conn);
                op.CommandType = CommandType.StoredProcedure;

                op.Parameters.AddWithValue("@CLIENTE",cuenta);
                op.Parameters.AddWithValue("@NOMBRES", nombre);
                op.Parameters.AddWithValue("@MONTO",bono);
                op.Parameters.AddWithValue("@")

            }
           
            
        }
    }
}
