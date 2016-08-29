using INTRANET_CRM.Recursos;
using System.Data;
using INTRANET_CRM.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Crm.Sdk;
using System.Windows.Forms;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using INTRANET_CRM.IntranetNumeraciones;

namespace INTRANET_CRM
{
    public class pruebacargos : CodeActivity
    {
       // private string name;
        [Input("EMPRESA")]
        [ReferenceTarget("account")]
        public InArgument<EntityReference> opID { get; set; }

        //[Input("unidad de negocio")]
        //[ReferenceTarget("businessunit")]
        //public InArgument<EntityReference> ESTOUT { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            //Create an Organization Service
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

            EntityReference cuenta = opID.Get(executionContext);
            Entity prue = service.Retrieve("account", cuenta.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            //EntityReference unidadnegocio = ESTOUT.Get(executionContext);



            string connection = "Data Source=ZEUS.atcom.cl; Initial Catalog=IntranetV5;User ID=zeus;Password=Atcom2012";
            SqlConnection conn = new SqlConnection(connection);
            conn.Open();

            string rut_empresa = Utils.intranetRut(prue["new_rut"].ToString());

            SqlCommand test = new SqlCommand("select CLIENTE_CARGO.DSC_TIPO_CONTRATO, CLIENTE_CARGO.ID_DSC_CARGO, CLIENTE_CARGO.ID_CARGO, CLIENTE_CARGO.TIPO_PAGO, CARGO_TARIFA.ID_MONEDA " +
                                            ",CLIENTE_CARGO.COMUNA_TRABAJO,  CLIENTE_CARGO.MONTO_SUELDO, CARGO_TARIFA.ID_TIPO_TARIFA, CLIENTE_CARGO.DSC_COM_SUELDO " +
                                            ",CLIENTE_CARGO.MONTO_COLACION, CLIENTE_CARGO.MONTO_LOCOMOCION, CLIENTE_CARGO.DSC_HORARIO, CLIENTE_CARGO.FECHA_INGRESO " +
                                            "from CARGO_TARIFA, CLIENTE_CARGO " +
                                            "where CARGO_TARIFA.ID_CARGO = CLIENTE_CARGO.ID_CARGO and CLIENTE_CARGO.ESTADO = '1' and CLIENTE_CARGO.RUT_EMPRESA = '" + rut_empresa + "'", conn);

            SqlDataReader leer = test.ExecuteReader();
            // Entity juego = new Entity("new_juego");
            // string resp = "";
            while (leer.Read())
            {
                Entity cargos = Utils.getExisteCargos(cuenta, service, leer.GetString(1), (decimal)leer.GetInt32(2));
                //Entity bussinesunit = Utils.ESTyOUT(unidadnegocio, service, name);

                Guid IDCARGO;

                if (cargos == null)
                {
                    cargos = new Entity("new_cargo");

                    try
                    {
                        Utils.addOrUpdateParameter(ref cargos, "new_idintranet", (decimal)leer.GetInt32(2));
                    }
                    catch(Exception e)
                    {
                        throw new Exception("FALLA id: " + leer.GetValue(2).ToString() + e.Message);
                    }
                    Utils.addOrUpdateParameter(ref cargos, "new_cuenta", cuenta);

                    Utils.addOrUpdateParameter(ref cargos, "new_name", leer.GetString(1));

                    decimal colacion = Convert.ToDecimal(leer.GetValue(9).ToString());
                    Utils.addOrUpdateParameter(ref cargos, "new_montocolacion2", colacion);

                    decimal locomocion = Convert.ToDecimal(leer.GetValue(10).ToString());
                    Utils.addOrUpdateParameter(ref cargos, "new_montolocomocion2", locomocion);
                    try
                    {
                        decimal sueldo = Int32.Parse(leer.GetValue(6).ToString());
                        Utils.addOrUpdateParameter(ref cargos, "new_montosueldo2", sueldo);
                    }
                    catch(Exception e )
                    {
                        throw new Exception("FALLA sueldo: " + leer.GetValue(6).ToString() + " " + e.Message);
                    }
                    try
                    {
                        int tipopago = Convert.ToInt32(leer.GetValue(3).ToString());
                        if (tipopago == 0)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipopago", new OptionSetValue(100000000));
                        }
                        else if (tipopago == 1)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipopago", new OptionSetValue(100000001));
                        }
                    }
                    catch(Exception e)
                    {
                        throw new Exception("FALLA pago: " + leer.GetValue(3)+ e.Message);
                    }

                    Utils.addOrUpdateParameter(ref cargos, "new_comunatrabajo", leer.GetString(5));
                    try
                    {
                        Utils.addOrUpdateParameter(ref cargos, "new_tarifa2", (decimal)leer.GetInt32(4));
                    }
                    catch(Exception e)
                    {
                        throw new Exception("FALLA tarifa: " + leer.GetValue(4) + e.Message);
                    }
                    int tiposueldo = Int32.Parse(leer.GetValue(7).ToString());
                    if (tiposueldo == 0)
                    {
                        Utils.addOrUpdateParameter(ref cargos, "new_divisatarifa", new OptionSetValue(100000000));
                    }
                    else if (tiposueldo == 1)
                    {
                        Utils.addOrUpdateParameter(ref cargos, "new_divisatarifa", new OptionSetValue(100000001));
                    }


                    Utils.addOrUpdateParameter(ref cargos, "new_fechaingreso", leer.GetDateTime(12));

                    Utils.addOrUpdateParameter(ref cargos, "new_descripcionhorario", leer.GetString(11));
                    try
                    {
                        if (leer.GetInt32(0) == 0)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000000));
                        }
                        else if (leer.GetInt32(0) == 1)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000001));
                        }
                        else if (leer.GetInt32(0) == 2)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000002));
                        }
                        else if (leer.GetInt32(0) == 4)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000004));
                        }
                    }
                    catch(Exception e)
                    {
                        throw new Exception("FALLA contrato: " + leer.GetValue(0) + e.Message);
                    }
                    //resp += "CARGO NUEVO: " + cargos["new_idintranet"] + " " + cargos["new_name"] + "\n";
                    //creacion cargo
                    IDCARGO = service.Create(cargos);


                }
                else
                {
                    IDCARGO = cargos.Id;


                    Utils.addOrUpdateParameter(ref cargos, "new_idintranet", (decimal)leer.GetInt32(2));

                    decimal colacion = Convert.ToDecimal(leer.GetValue(9).ToString());
                    Utils.addOrUpdateParameter(ref cargos, "new_montocolacion2", colacion);

                    decimal locomocion = Convert.ToDecimal(leer.GetValue(10).ToString());
                    Utils.addOrUpdateParameter(ref cargos, "new_montolocomocion2", locomocion);

                    Utils.addOrUpdateParameter(ref cargos, "new_fechaingreso", leer.GetDateTime(12));
                    try
                    {
                        decimal sueldo = Convert.ToDecimal(leer.GetValue(6).ToString());
                        Utils.addOrUpdateParameter(ref cargos, "new_montosueldo2", sueldo);
                    }
                    catch(Exception e)
                    {
                        throw new Exception("FALLA sueldo(U): " + leer.GetValue(6) + e.Message);
                    }
                    try
                    {
                        int tipopago = Convert.ToInt32(leer.GetValue(3).ToString());
                        if (tipopago == 0)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipopago", new OptionSetValue(100000000));
                        }
                        else if (tipopago == 1)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipopago", new OptionSetValue(100000001));
                        }
                    }
                    catch(Exception e)
                    {
                        throw new Exception("FALLA pago(U): " + leer.GetValue(3) + e.Message);
                    }

                    Utils.addOrUpdateParameter(ref cargos, "new_comunatrabajo", leer.GetString(5));
                    try
                    {
                        Utils.addIfNotExistParameter(ref cargos, "new_tarifa2", (decimal)leer.GetInt32(4));
                    }
                    catch(Exception e)
                    {
                        throw new Exception("FALLA tarifa(U): " +leer.GetValue(4) + e.Message);
                    }
                        //try
                    //{
                    //    int tiposueldo = Int32.Parse(leer.GetValue(7).ToString());
                    //    if (tiposueldo == 0)
                    //    {
                    //        Utils.addOrUpdateParameter(ref cargos, "new_divisatarifa", new OptionSetValue(100000000));
                    //    }
                    //    else if (tiposueldo == 1)
                    //    {
                    //        Utils.addOrUpdateParameter(ref cargos, "new_divisatarifa", new OptionSetValue(100000001));
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    throw new Exception("CONVERSION DE NUMERIC(U):" + leer.GetValue(7).ToString());
                    //}
                    try
                    {
                        if (leer.GetInt32(0) == 0)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000000));
                        }
                        
                        else if (leer.GetInt32(0) == 1)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000001));
                        }
                    
                        else if (leer.GetInt32(0) == 2)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000002));
                        }
                        else if (leer.GetInt32(0) == 4)
                        {
                            Utils.addOrUpdateParameter(ref cargos, "new_tipodecontrato", new OptionSetValue(100000004));
                        }
                    }
                        catch(Exception e)
                    {
                        throw new Exception("FALLA contrato(U): " + leer.GetValue(0) + e.Message);
                        }
                   

                    Utils.addOrUpdateParameter(ref cargos, "new_descripcionhorario", leer.GetString(11));
                    //actualizar cargo
                    // resp += "CARGO ACTUALIZADO: " + cargos["new_idintranet"] + " " + cargos["new_name"] + "\n";
                    service.Update(cargos);

                }


            }
            //  Utils.addOrUpdateParameter(ref juego, "new_lineas", resp);
            // service.Create(juego);
            leer.Dispose();
            leer.Close();
            conn.Close();




        }
    }
}
