using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM
{
    public class RecursosActivos : CodeActivity
    {

        [Output("RESPUESTA")]
        public OutArgument<int> outResp { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            try
            {
                IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

                //Create an Organization Service
                IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

                String connectionZeus = "Data Source=ZEUS.atcom.cl; Initial Catalog=IntranetV5;User ID=test;Password=Aa123456";
                SqlConnection connZeus = new SqlConnection(connectionZeus);

                connZeus.Open();

                SqlCommand recursos2016 = new SqlCommand(
                    "SELECT R.RUT_RECURSO,  R.NOMBRE_RECURSO, R.APELLIDO_RECURSO, R.TELEFONO_RECURSO, R.EMAIL_RECURSO," +
                    "R.COMUNA_RECURSO, R.FECHA_NACIMIENTO, R.NACIONALIDAD_RECURSO, R.CELULAR_RECURSO, R.SEXO, " +
                    "R.RUT_USUARIO " +
                    "FROM RECURSO AS R " +
                    "JOIN FICHA AS F ON R.RUT_RECURSO = F.RUT_RECURSO " +
                    "WHERE F.FECHA_INICIO_FICHA >= '2016-01-01'", connZeus);

                SqlDataReader dataReader;
                dataReader = recursos2016.ExecuteReader();
                int i = 0;
                Dictionary<String, Entity> usuarios = new Dictionary<String, Entity>();
                while (dataReader.Read())
                {

                    string rut = dataReader.GetValue(0).ToString();
                    string rutCRM = rut.Substring(0, rut.Length - 2) + "-" + rut.Substring(rut.Length - 1);
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
                                new ConditionExpression("new_rut", ConditionOperator.Equal, rutCRM)
                            }
                    });

                    EntityCollection result = service.RetrieveMultiple(query);
                    if (result != null && result.Entities != null && result.Entities.Count <= 1)
                    {
                        if (result.Entities.Count > 0)
                        {
                            Entity r = result.Entities[0];
                            if (!r.Contains("new_fechadenacimiento"))
                                r.Attributes.Add("new_fechadenacimiento", dataReader.GetDateTime(6));
                            /*
                                if (!r.Contains("new_telefono"))
                                {
                                    int tel = dataReader.GetInt32(3);
                                    if (tel != null && tel != 0)
                                        r.Attributes.Add("new_telefono", tel);
                                    else
                                    {
                                        int cel = dataReader.GetInt32(8);
                                        if (cel != null && cel != 0)
                                            r.Attributes.Add("new_telefono", cel);
                                    }
                                }
                                */
                            if (!r.Contains("new_comunaderesidencia"))
                                r.Attributes.Add("new_comunaderesidencia", dataReader.GetString(5));
                            if (!r.Contains("new_email") && dataReader.GetString(4) != null)
                                r.Attributes.Add("new_email", dataReader.GetString(4));
                            if (dataReader.GetValue(9).ToString() == "F")
                                r["new_sexo"] = true;
                            else
                                r["new_sexo"] = false;
                            Entity usuario;
                            String rutUsuario = dataReader.GetString(10);
                            if (!usuarios.TryGetValue(rutUsuario, out usuario))
                            {
                                EntityCollection result2 = getUserByRut(rutUsuario, service);
                                if (result2 != null && result2.Entities.Count == 1)
                                {
                                    usuario = result2.Entities[0];
                                    usuarios.Add(rutUsuario, usuario);
                                }


                            }
                            if (usuario == null)
                                throw new Exception("USUARIO NULL AL EDITAR: " +  rutUsuario);
                            r["new_reclutadora"] = usuario.ToEntityReference();

                            service.Update(r);
                            i++;
                        }
                        else
                        {
                            Entity r = new Entity("new_colaborador");
                            r.Attributes.Add("new_rut", rutCRM);
                            r.Attributes.Add("new_name", dataReader.GetString(1));
                            r.Attributes.Add("new_apellidos", dataReader.GetString(2));
                            r.Attributes.Add("new_fechadenacimiento", dataReader.GetDateTime(6));

                            /*
                                int tel = dataReader.GetInt32(3);
                                if (tel != null && tel != 0)
                                r.Attributes.Add("new_telefono", tel);
                            else
                            {
                                int cel = dataReader.GetInt32(8);
                                if (cel != null && cel != 0)
                                    r.Attributes.Add("new_telefono", cel);
                            }
                                */
                            r.Attributes.Add("new_comunaderesidencia", dataReader.GetString(5));
                            r.Attributes.Add("new_email", dataReader.GetString(4));
                            if (dataReader.GetValue(9).ToString() == "F")
                                r.Attributes.Add("new_sexo", true);
                            else
                                r.Attributes.Add("new_sexo", false);
                            Entity usuario;
                            String rutUsuario = dataReader.GetString(10);
                            if (!usuarios.TryGetValue(rutUsuario, out usuario))
                            {

                                EntityCollection result2 = getUserByRut(rutUsuario, service);
                                if (result2 != null && result2.Entities.Count == 1)
                                {
                                    usuario = result2.Entities[0];
                                    usuarios.Add(rutUsuario, usuario);
                                }
                            }
                            if (usuario == null)
                                throw new Exception("USUARIO NULL AL CREAR: " + rutUsuario);
                            r.Attributes.Add("new_reclutadora", usuario.ToEntityReference());
                            service.Create(r);
                            i++;
                        }


                    }

                }
                dataReader.Close();
                connZeus.Close();
                outResp.Set(executionContext, i);

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw FullStackTraceException.Create(ex);


            }
        }

        static EntityCollection getUserByRut(String rutUsuario, IOrganizationService service)
        {
            QueryExpression userQuery = new QueryExpression("systemuser")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            userQuery.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                                        {
                                            //Get the row for the relationship where the account and contact are the account and contact passed in
                                            new ConditionExpression("isdisabled", ConditionOperator.Equal, false),
                                            new ConditionExpression("new_rut", ConditionOperator.Equal, rutUsuario)
                                        }
            });
            return service.RetrieveMultiple(userQuery);
        }


    }
}