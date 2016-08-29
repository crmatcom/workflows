using INTRANET_CRM.Recursos;
using System.Data;
using INTRANET_CRM.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Windows.Forms;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.ServiceModel;



namespace INTRANET_CRM
{
    public class Facturas : CodeActivity
    {


        string nombre;
        
        string cliente;
        
        int bono;
        int sueldo;
        string dias;
        int MES = DateTime.Now.Month;
        int AÑO = DateTime.Now.Year;

        int Fichas;



        [Input("EMPRESA")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> opID { get; set; }

        //[Input("COLABORADOR")]
        //[ReferenceTarget("new_colaborador")]
        //public InArgument<EntityReference> colID { get; set; }




        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference cuenta = opID.Get(executionContext);
            Entity prue = service.Retrieve("account", cuenta.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));

            //EntityReference colRef = colID.Get(executionContext);
            //Entity colaborador = service.Retrieve("new_colaborador", colRef.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));



            string connection = "Data Source=ZEUS.atcom.cl; Initial Catalog=IntranetV5;User ID=zeus;Password=Atcom2012";
            SqlConnection conn = new SqlConnection(connection);
            conn.Open();
            try
            {
                string name = Utils.intranetRut(prue["new_rut"].ToString());
                var Rut = Utils.Getcuentas(cuenta, service, name);

                SqlCommand ficha = new SqlCommand("select F.ID_FICHA, FVC.CLIENTE, FVC.RUT_CLIENTE  " +
                                                   "from FICHAS_VIGENTES_CIERRE ('" + AÑO + "','" + MES + "') FVC join FICHA F on F.ID_FICHA = FVC.ID_FICHA " +
                                                   "join EMPRESA E on E.RUT_EMPRESA = FVC.RUT_CLIENTE " +
                                                    "where FVC.RUT_CLIENTE = '" + Rut + "' ", conn);

                SqlDataReader NUM = ficha.ExecuteReader();

                try
                {
                    while (NUM.Read())
                    {
                        try
                        {
                            Entity Empresa = Utils.Getcuentas(cuenta, service, NUM.GetString(2));

                            Guid fact;

                            if (Empresa != null)
                            {
                                Empresa = new Entity("new_ordedecompra");


                                cliente = NUM.GetString(1);
                                Utils.addOrUpdateParameter(ref Empresa, "new_cuenta", cliente);
                                try
                                {

                                    Fichas = Convert.ToInt32(NUM.GetInt32(0));
                                    Utils.addOrUpdateParameter(ref Empresa, "new_numerodeorden", Fichas);
                                }
                                catch (Exception A)
                                {

                                    throw new Exception("FALLA : " + Fichas + A.Message); ;
                                }



                                //consulta recurso y cliente

                                SqlCommand test = new SqlCommand("select FVC.ID_FICHA, F.RUT_RECURSO, R.APELLIDO_RECURSO, " +
                                                                    "(DATEDIFF(DAY,FVC.FECHA_INICIO,FVC.FECHA_TERMINO)) as dias, F.SUELDO_LIQUIDO, FVC.FECHA_INICIO " +
                                                            "from FICHAS_VIGENTES_CIERRE ('" + AÑO + "','" + MES + "') FVC join FICHA F on FVC.ID_FICHA = F.ID_FICHA " +
                                                                    "join RECURSO R on R.RUT_RECURSO = F.RUT_RECURSO and FVC.ID_FICHA = '" + Fichas + "' " +
                                                            "where F.ID_ESTADO_FICHA = '2' or F.ID_ESTADO_FICHA = '3' and R.ESTADO_RECURSO = '0' ", conn);


                                SqlDataReader leer = test.ExecuteReader();

                                while (leer.Read())
                                {

                                    nombre = leer.GetString(2);
                                    try
                                    {
                                        Entity recurso = Utils.getColaboradorFromRutCRM(nombre, service);
                                        Guid rec;
                                        if (recurso != null)
                                        {

                                            dias = Convert.ToString(leer.GetInt32(3));
                                            Utils.addOrUpdateParameter(ref Empresa, "new_diastrabajados", dias);


                                            sueldo = leer.GetInt32(4);
                                            Utils.addOrUpdateParameter(ref Empresa, "new_tarifa", sueldo);

                                            Utils.addOrUpdateParameter(ref Empresa, "new_fechaingreso", leer.GetDateTime(5));
                                        }
                                        try
                                        {
                                            rec = service.Create(recurso);
                                        }
                                        catch (Exception re)
                                        {
                                            throw new Exception("FALLA re:  " + re.Message);
                                        }

                                    }

                                    catch (Exception T)
                                    {
                                        throw new Exception("FALLA :" + T.Message);
                                    }
                                }
                                leer.Close();


                                //metodo bonos               
                                SqlCommand srrt = new SqlCommand("select FB.ID_BONOS, CB.MONTO_BONO, FB.TOT_BONO " +
                                                                "from FICHA_BONOS FB join CARGO_BONOS CB on FB.ID_BONOS = CB.ID_BONO " +
                                                                        "join FICHA F on F.ID_FICHA = FB.ID_FICHA " +
                                                                "where F.ID_FICHA = '" + Fichas + "'", conn);

                                SqlDataReader line = srrt.ExecuteReader();

                                while (line.Read())
                                {

                                    bono = line.GetInt32(2);
                                    Utils.addOrUpdateParameter(ref Empresa, "new_bonos", bono);

                                }
                                line.Close();
                                
                                try
                                {
                                    fact = service.Create(Empresa);
                                }
                                catch (Exception fac)
                                {
                                    throw new Exception("FALLA fac: " + fac.Message);
                                }
                            }
                        }
                        catch (Exception R)
                        {
                            throw new Exception("FALLA : " + R.Message);
                        }

                    }
                }
                catch (Exception E)
                {
                    throw new Exception("FALLA :" + NUM + " " + E.Message);
                }

                NUM.Close();
                conn.Close();


            }

            catch (Exception PO)
            {
                throw new Exception("FALLA name : " + PO.Message);
            }


        }


    }
}




    


    


