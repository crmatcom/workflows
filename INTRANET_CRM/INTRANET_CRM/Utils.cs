using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM
{
    class Utils
    {
        public static string crmRut(string intranetRut)
        {
            return intranetRut.Substring(0, intranetRut.Length - 1) + "-" + intranetRut.Substring(intranetRut.Length - 1);
        }

        public static string intranetRut(string crmRut)
        {
            int guion = crmRut.IndexOf('-');
            return crmRut.Substring(0, guion) + crmRut.Substring(guion + 1);
        }

        public static Entity getUserByRut(string rutCrm, IOrganizationService service)
        {
            QueryExpression query = new QueryExpression("systemuser")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };
            query.Criteria.AddCondition(new ConditionExpression("new_rut", ConditionOperator.Equal, rutCrm));
            EntityCollection usuarios = service.RetrieveMultiple(query);
            if (usuarios.Entities.Count > 0)
                return usuarios.Entities[0];
            else
                return null;
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

        public static Entity getExisteCargos(EntityReference cuenta, IOrganizationService service, string name, decimal id)
        {
            QueryExpression query = new QueryExpression("new_cargo")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };

            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions = 
                  {
                      new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                  }

            });
                
            query.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Conditions = 
                  {
                      new ConditionExpression("new_idintranet", ConditionOperator.Equal, id),
                      new ConditionExpression("new_cuenta", ConditionOperator.Equal, cuenta.Id)
                  }

            });
            query.Criteria.AddFilter(new FilterExpression
            {
                FilterOperator = LogicalOperator.Or,
                Conditions = 
                  {
                      new ConditionExpression("new_idintranet", ConditionOperator.Equal, id),
                      new ConditionExpression("new_name", ConditionOperator.Equal, name)
                  }

            });
            
                EntityCollection cargos = service.RetrieveMultiple(query);
                
            if(cargos.Entities.Count > 0)
                return cargos.Entities[0];
            return null;
  
            }
      

        public static Entity Getcuentas(EntityReference cuenta, IOrganizationService service, string rut)
           {
               QueryExpression query = new QueryExpression("account")
               {
                   NoLock = true,
                   ColumnSet = new ColumnSet(true)
               };

               query.Criteria.AddFilter(new FilterExpression
               {
                   Conditions = 
                     {
                       new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                       new ConditionExpression("new_rut", ConditionOperator.Equal, rut)
                     }

               });
              
               EntityCollection cuentas = service.RetrieveMultiple(query);
               if (cuentas.Entities.Count > 0)
                   return cuentas.Entities[0];
               return null;

            }

        public static Entity getExisteRecurso(EntityReference colaborador, IOrganizationService service, string name)
        {
            QueryExpression query = new QueryExpression("resource")
            {
                NoLock = true,
                ColumnSet = new ColumnSet(true)
            };

            query.Criteria.AddFilter(new FilterExpression
            {
                Conditions = 
                  {
                      new ConditionExpression("statecode", ConditionOperator.Equal, 0),
                      new ConditionExpression("name", ConditionOperator.Equal, name)
                  }

            }); 

            EntityCollection recurso = service.RetrieveMultiple(query);

            if (recurso.Entities.Count > 0)
                return recurso.Entities[0];
            return null;

        }

        static public Entity getColaboradorFromRutCRM(string rutCrm, IOrganizationService service)
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
                         new ConditionExpression("new_rut", ConditionOperator.Equal, rutCrm),
                         new ConditionExpression("statecode", ConditionOperator.Equal, 0)
                     }
            });

            EntityCollection colaboradores = service.RetrieveMultiple(query);
            if (colaboradores.Entities.Count > 0)
                return colaboradores.Entities[0];
            //Check if the relationship was not found
            return null;
        }
       
         







    }
    }

