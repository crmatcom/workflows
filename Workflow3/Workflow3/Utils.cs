using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCOM_jtoro
{
    public class Utils{

        public static int COLABORADOR_TYPE_CODE = 10005;
        public static int EMAIL_TYPE_CODE = 4202;
        public static int APROBADO_JEFA_VENTAS = 100000000;

        public static EntityCollection areAsociated(IOrganizationService service, String relationshipName,
            String ref1, String refName1,
            String ref2, String refName2)
        {

            QueryExpression query = new QueryExpression(relationshipName)
    {
        NoLock = true,
        ColumnSet = new ColumnSet(false),//only get the row ID, since we don't need any actual values
        Criteria =
        {
            Filters =
             {
                 new FilterExpression
                 {
                     FilterOperator = LogicalOperator.And,
                     Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression(refName1, ConditionOperator.Equal, ref1),
                         new ConditionExpression(refName2, ConditionOperator.Equal, ref2),
                     },
                 },
             }
        }
    };

            var result = service.RetrieveMultiple(query);
            //Check if the relationship was not found
            return result;

        }

        public static EntityCollection guidAsociated(IOrganizationService service, String relationshipName, String guidColumn,
           String entRef, String refName)
        {
            ColumnSet columnSet = new ColumnSet(new String[]{ guidColumn });
            return guidAsociated(service,relationshipName,columnSet,entRef,refName);

        }

        public static EntityCollection guidAsociated(IOrganizationService service, String relationshipName, bool guidColumn,
           String entRef, String refName)
        {

            ColumnSet columnSet = new ColumnSet(guidColumn);
            return guidAsociated(service,relationshipName,columnSet,entRef,refName);

        }

        private static EntityCollection guidAsociated(IOrganizationService service, String relationshipName, ColumnSet columnSet,
            String entRef, String refName){

            QueryExpression query = new QueryExpression(relationshipName)
            {
                NoLock = true,
                ColumnSet = columnSet,
                Criteria =
                {
                    Filters =
             {
                 new FilterExpression
                 {
                     Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression(refName, ConditionOperator.Equal, entRef)
                     },
                 },
             }
                }
            };

            var result = service.RetrieveMultiple(query);
            //Check if the relationship was not found
            return result;


        }

        public static EntityCollection getPorIngresarMañana(IOrganizationService service, EntityReference unit)
        {

            QueryExpression query = new QueryExpression("new_colaboradorenoportunidad")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.LinkEntities.Add(new LinkEntity("new_colaboradorenoportunidad", "systemuser", "createdby", "systemuserid", JoinOperator.Inner));
            query.LinkEntities[0].EntityAlias = "USER";
            query.Criteria.AddFilter(new FilterExpression
                {
                     Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("new_fechaingreso", ConditionOperator.Tomorrow),
                         new ConditionExpression("new_ingreso", ConditionOperator.Equal, 100000002),
                         new ConditionExpression("USER", "businessunitid", ConditionOperator.Equal, unit.Id)
                     }
                 });

            var result = service.RetrieveMultiple(query);
            //Check if the relationship was not found
            return result;
        }

        public static EntityCollection getResumenIngresosHoy(IOrganizationService service, EntityReference unit)
        {

            QueryExpression query = new QueryExpression("new_colaboradorenoportunidad")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.LinkEntities.Add(new LinkEntity("new_colaboradorenoportunidad", "systemuser", "createdby", "systemuserid", JoinOperator.Inner));
            query.LinkEntities[0].EntityAlias = "USER";
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("new_fechaingreso", ConditionOperator.Today),
                         new ConditionExpression("USER", "businessunitid", ConditionOperator.Equal, unit.Id)
                     }
            });

            var result = service.RetrieveMultiple(query);
            //Check if the relationship was not found
            return result;
        }

        internal static EntityCollection getPorIngresarMañanaSupervisor(IOrganizationService service, EntityReference userRef)
        {
             QueryExpression query = new QueryExpression("new_colaboradorenoportunidad")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.LinkEntities.Add(new LinkEntity("new_colaboradorenoportunidad", "account", "new_cuenta", "accountid", JoinOperator.Inner));
            query.LinkEntities[0].EntityAlias = "CUENTA";
            query.Criteria.AddFilter(new FilterExpression
                {
                     Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("new_fechaingreso", ConditionOperator.Tomorrow),
                         new ConditionExpression("new_ingreso", ConditionOperator.Equal, 100000002),
                         new ConditionExpression("CUENTA", "new_supervisor", ConditionOperator.Equal, userRef.Id)
                     }
                 });

            var result = service.RetrieveMultiple(query);
            //Check if the relationship was not found
            return result;
        }

        public static EntityCollection getEgresosProximaSemana(IOrganizationService service, EntityReference unidadRef)
        {

            QueryExpression query = new QueryExpression("new_fichadetrabajo")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.LinkEntities.Add(new LinkEntity("new_fichadetrabajo", "account", "new_cuenta", "accountid", JoinOperator.Inner));
            query.LinkEntities[0].EntityAlias = "CUENTA";
            query.LinkEntities[0].LinkEntities.Add(new LinkEntity("account", "systemuser", "new_supervisor", "systemuserid", JoinOperator.Inner));
            query.LinkEntities[0].LinkEntities[0].EntityAlias = "USUARIO";
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                         new ConditionExpression("USUARIO", "businessunitid", ConditionOperator.Equal, unidadRef.Id)
                     }
            });
            query.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Conditions =
                     {
                         //Get the row for the relationship where the account and contact are the account and contact passed in
                         new ConditionExpression("new_fechatermino", ConditionOperator.NextWeek),
                         new ConditionExpression("new_fechaegreso", ConditionOperator.NextWeek),
                     }
            });
            try
            {
                var result = service.RetrieveMultiple(query);
                return result;
            }
            catch(Exception e)
            {
                throw new Exception("ERROR UTILS 206:" + e.Message);
            }
            //Check if the relationship was not found
            
        }

        public static EntityCollection getCEOByOpportunity(EntityReference op, IOrganizationService service)
        {

            QueryExpression query = new QueryExpression("new_colaboradorenoportunidad")
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
                         new ConditionExpression("new_oportunidad", ConditionOperator.Equal, op.Id)
                     }
            });
            return service.RetrieveMultiple(query);

        }
        public static EntityCollection FacturaDuplicar(IOrganizationService service, Guid id)
        {
            QueryExpression query = new QueryExpression("new_ordendecompra")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
             };
            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                    new ConditionExpression("new_factura", ConditionOperator.Equal, id),
                }
                
            });
            return service.RetrieveMultiple(query);

        }
       
            public static string crmRut(string Rut)
            {
                return Rut.Substring(0, Rut.Length - 1) + "-" + Rut.Substring(Rut.Length - 1);
            }
            public static string rut(string crmRut)
            {
                int guion = crmRut.IndexOf('-');
                return crmRut.Substring(0, guion) + crmRut.Substring(guion, 1);
            }


            public static Entity getRut(string rutCrm, IOrganizationService service)
            {
                QueryExpression query = new QueryExpression("account")
                {
                    NoLock = true,
                    ColumnSet = new ColumnSet(true)
                };
                query.Criteria.AddCondition(new ConditionExpression("new_rut", ConditionOperator.Equal, rutCrm));
                EntityCollection cuenta = service.RetrieveMultiple(query);
                if (cuenta.Entities.Count > 0)
                    return cuenta.Entities[0];
                else
                    return null;
            }
            private Entity getCRMRecursoRUT(string rut, IOrganizationService service)
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
            }


        


        public static bool addOrUpdateParameter(ref Entity e, string parameterName, Object o)
        {
            if (e.Contains(parameterName))
            {
                bool resp = e[parameterName] != o;
                e[parameterName] = o;
                return resp;
            }
            else
            {
                e.Attributes.Add(parameterName, o);
                return true;
            }
        }

        public static bool setEmptyParameter(ref Entity e, string parameterName)
        {
            if (e.Contains(parameterName))
            {
                e.Attributes.Remove(parameterName);
                return true;
            }

            return false;
        }

        internal static bool addIfNotExistParameter(ref Entity e, string parameterName, Object o)
        {
            if (!e.Contains(parameterName))
            {
                e.Attributes.Add(parameterName, o);
                return true;
            }
            else
                return false;
        }
       


    }
}

