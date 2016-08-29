using INTRANET_CRM.IntranetNumeraciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INTRANET_CRM.Model;
using Microsoft.Xrm.Sdk;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Query;

namespace INTRANET_CRM.Recursos
{
    class Recurso
    {
        public Guid crmID;
        public string rut;
        public string crmRut;
        public string nombre;
        public string apellidoPaterno;
        public string apellidoMaterno;
        public SEXO sexo;
        public string nacionalidad;
        public EstadoCivil estadoCivil;
        public REGION region;
        public CIUDAD ciudad;
        public COMUNA comuna;
        public string direccion;
        public long telefono;
        public string celular;
        public string email;
        public AFP afp;
        public ISAPRE isapre;
        public bool cuentaBancaria;
        public BANCO banco;
        public TipoCuenta tipoCuenta;
        public string nroCuenta;
        public string rutUsuario;
        public DateTime fechaNacimiento;

        public Recurso(string rut, string nombre, string apellidoPaterno, string apellidoMaterno, SEXO sexo,
            string nacionalidad, EstadoCivil ec, REGION region, CIUDAD ciudad, COMUNA comuna,
            string direccion, int telefono, string celular, string email, AFP afp,
            ISAPRE isapre, bool cuentaBancaria, BANCO banco, TipoCuenta tipoCuenta, string nroCuenta,
            string rutUsuario, DateTime fechaNacimiento)
        {
            this.rut = rut;
            this.crmRut = Utils.crmRut(rut);
            this.nombre = nombre;
            this.apellidoPaterno = apellidoPaterno;
            this.apellidoMaterno = apellidoMaterno;
            this.sexo = sexo;
            this.nacionalidad = nacionalidad;
            this.estadoCivil = ec;
            this.region = region;
            this.ciudad = ciudad;
            this.comuna = comuna;
            this.direccion = direccion;
            this.telefono = telefono;
            this.celular = celular;
            this.email = email;
            this.afp = afp;
            this.isapre = isapre;
            this.cuentaBancaria = cuentaBancaria;
            this.banco = banco;
            this.tipoCuenta = tipoCuenta;
            this.nroCuenta = nroCuenta;
            this.rutUsuario = rutUsuario;
            this.fechaNacimiento = fechaNacimiento;
        }

        public Recurso(Entity colaborador, string rutUsuario)
        {
            //OBLIGATORIOS
            this.crmID = colaborador.Id;
            this.crmRut = (string)colaborador["new_rut"];
            this.rut = Utils.intranetRut(this.crmRut);
            this.nombre = (string)colaborador["new_name"];
            this.apellidoPaterno = (string)colaborador["new_apellidopaterno"];
            this.apellidoMaterno = (string)colaborador["new_apellidomaterno"];
            this.sexo = new SEXO((bool)colaborador["new_sexo"]);
            
            if(colaborador.Contains("new_nacionalidad2"))
                this.nacionalidad = (string) colaborador["new_nacionalidad2"];
            if(colaborador.Contains("new_estadocivil"))
                this.estadoCivil = new EstadoCivil(((OptionSetValue)colaborador["new_estadocivil"]).Value);
            if(colaborador.Contains("new_region"))
                this.region = new REGION(((OptionSetValue)colaborador["new_region"]).Value, true);
            if(colaborador.Contains("new_ciudad"))
                this.ciudad = new CIUDAD(((OptionSetValue)colaborador["new_ciudad"]).Value);
            if(colaborador.Contains("new_comuna"))
                this.comuna = new COMUNA(((OptionSetValue)colaborador["new_comuna"]).Value);
            if(colaborador.Contains("new_direccion"))
                this.direccion = (string)colaborador["new_direccion"];
            if(colaborador.Contains("new_telefono2"))
                this.telefono = (int)colaborador["new_telefono2"];
            this.celular = null;
            if(colaborador.Contains("new_email"))
                this.email = (string)colaborador["new_email"];
            if(colaborador.Contains("new_afp"))
                this.afp = new AFP(((OptionSetValue)colaborador["new_afp"]).Value);
            if(colaborador.Contains("new_salud"))
                this.isapre = new ISAPRE(((OptionSetValue)colaborador["new_salud"]).Value);
            if (colaborador.Contains("new_cuentaabancaria2"))
                this.cuentaBancaria = (bool)colaborador["new_cuentabancaria2"];
            if(this.cuentaBancaria)
            {
                this.banco = new BANCO(((OptionSetValue)colaborador["new_cuentabancaria"]).Value, true);
                this.tipoCuenta = new TipoCuenta(((OptionSetValue)colaborador["new_cuentabancaria"]).Value, true);
                this.nroCuenta = (string)colaborador["new_nmerodecuenta"];
            }
            if (colaborador.Contains("new_fechadenacimiento"))
                this.fechaNacimiento = (DateTime)colaborador["new_fechadenacimiento"];
            this.rutUsuario = rutUsuario;
        }
        /*
         * 0 OK
         * -1 ERROR
         * -3 YA EXISTE
        */

        public Recurso(string rutIntranet, SqlConnection conn)
        {
            SqlCommand recIntranet = new SqlCommand(
                "SELECT RUT_RECURSO, NOMBRE_RECURSO, APELLIDO_PATERNO, APELLIDO_MATERNO, SEXO, " +
                "NACIONALIDAD_RECURSO, ESTADO_CIVIL, REGION, CIUDAD_SOFTLAND, COMUNA_SOFTLAND, " +
                "DIRECCION_RECURSO, TELEFONO_RECURSO, CELULAR_RECURSO, EMAIL_RECURSO, id_afpt, " +
                "id_saludt, ID_BANCO, ID_TIPO_CTABANCARIA, NCTA, RUT_USUARIO, " +
                "FECHA_NACIMIENTO " +
                "FROM RECURSO " +
                "WHERE RUT_RECURSO = '" + rutIntranet + "'", conn);

            SqlDataReader data = recIntranet.ExecuteReader();
            if (data.Read())
            {

                this.rut = data.GetString(0);
                this.crmRut = Utils.crmRut(this.rut);
                this.nombre = data.GetString(1);
                this.apellidoPaterno = data.GetString(2);
                this.apellidoMaterno = data.GetString(3);
                this.sexo = new SEXO(data.GetString(4));
                this.nacionalidad = data.GetString(5);
                this.estadoCivil = new EstadoCivil(data.GetString(6));
                try
                {
                    string regionString = data.GetValue(7).ToString();
                    if (regionString != null && regionString != "" && regionString != "0")
                        this.region = new REGION(data.GetInt32(7), false);
                }
                catch (Exception e)
                {
                    throw new Exception(rutIntranet + "-ERROR EN REGION:" + e.Message);
                }
                this.ciudad = new CIUDAD(data.GetString(8));
                this.comuna = new COMUNA(data.GetString(9));
                this.direccion = data.GetString(10);
                this.telefono = (int)data.GetInt64(11);
                this.celular = data.GetString(12);
                this.email = data.GetString(13);
                this.afp = new AFP(data.GetString(14));
                if(data.GetString(15) != "007")
                    this.isapre = new ISAPRE(data.GetString(15));
                string cuentaBancariaString = data.GetValue(16).ToString();
                try
                {
                    
                    this.cuentaBancaria = cuentaBancariaString != null && cuentaBancariaString != "" && cuentaBancariaString != "0" && cuentaBancariaString != "9999";
                    if (this.cuentaBancaria)
                    {

                        this.banco = new BANCO(data.GetInt32(16), false);
                        this.tipoCuenta = new TipoCuenta(data.GetInt32(17), false);
                        this.nroCuenta = data.GetString(18); 

                    }
                }
                catch (Exception e)
                {
                    throw new Exception(rutIntranet + "-ERROR EN BANCO (" + cuentaBancariaString + "):" + e.Message);
                }
                this.rutUsuario = data.GetString(19);
                string fechaNacimientoString = data.GetValue(20).ToString();
                if(fechaNacimientoString != null && fechaNacimientoString != "" && data.GetDateTime(20) >= new DateTime(1900, 1, 1))
                    this.fechaNacimiento = data.GetDateTime(20);
                
                data.Dispose();

            }
            else
                throw new Exception("NO SE PUDO HACER CONSULTA");
        
        }

        public bool intranetDataOK()
        {
            bool oblig = this.rut != null &&
                this.rutUsuario != null &&
                this.nombre != null &&
                this.apellidoPaterno != null &&
                this.apellidoMaterno != null &&
                this.fechaNacimiento != null &&
                this.sexo.intranetSEXO != null &&
                this.nacionalidad != null &&
                this.estadoCivil.intranetEstadoCivil != null &&
                this.ciudad.intranetCiudad != null &&
                this.comuna.intranetComuna != null &&
                this.comuna.nombreComuna != null &&
                this.direccion != null &&
                this.email != null &&
                this.afp.intranetAFP != null &&
                this.isapre.intranetISAPRE != null;

            if (this.cuentaBancaria)
            {
                return oblig &&
                    this.nroCuenta != null;
            }

            return oblig;
        }


        /*
         * 0 OK,
         * -3 Usuario ya existe
         * -1 rut nulo
         * -2 rut invalido
         * -100 faltan datos
         */
        public int createInIntranet()
        {
            String connectionZeus;
            SqlConnection connZeus;
            SqlCommand createRecurso;
            connectionZeus = "Data Source=ZEUS.atcom.cl; Initial Catalog=IntranetV5;User ID=zeus;Password=Atcom2012";
            connZeus = new SqlConnection(connectionZeus);
            connZeus.Open();

            createRecurso = new SqlCommand("CRM_RECURSO_Agregar", connZeus);
            createRecurso.CommandType = CommandType.StoredProcedure;
            if (!intranetDataOK())
                return -100;

            //OBLIGATORIO
            
            camposObligatoriosIntranet(createRecurso);
                
            //OPTATIVO
                
            //NULL
            camposNullIntranet(createRecurso);
                        
            SqlParameter ret = createRecurso.Parameters.Add("@", SqlDbType.Int);
            ret.Direction = ParameterDirection.ReturnValue;
            createRecurso.ExecuteNonQuery();
            int retValue = (int)ret.Value;
            connZeus.Close();
            return retValue;
    
        }

        private void camposNullIntranet(SqlCommand createRecurso)
        {
            createRecurso.Parameters.AddWithValue("@i_id_ocupacion", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@s500_detalle_recurso", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@s500_url_curriculum", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@bi_telefono_recurso_2", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@s15_celular_recurso", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@i_cuenta_vista_generada", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@i_registro_victoria", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@i_estado_recurso", 0);
            createRecurso.Parameters.AddWithValue("@s1_verificador", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@s100_observaciones", DBNull.Value);
            createRecurso.Parameters.AddWithValue("@s100_motivos_no_contrato", DBNull.Value);
        }

        private void camposObligatoriosIntranet(SqlCommand createRecurso)
        {
            createRecurso.Parameters.AddWithValue("@s10_rut_recurso", this.rut);
            createRecurso.Parameters.AddWithValue("@s10_rut_usuario", this.rutUsuario);
            createRecurso.Parameters.AddWithValue("@dt_fecha_ingreso_recurso", DateTime.Now);
            createRecurso.Parameters.AddWithValue("@s100_nombre_recurso", intranetString(this.nombre));
            createRecurso.Parameters.AddWithValue("@s100_apellido_paterno", intranetString(this.apellidoPaterno));
            createRecurso.Parameters.AddWithValue("@s100_apellido_materno", intranetString(this.apellidoMaterno));
            createRecurso.Parameters.AddWithValue("@s100_apellido_recurso", intranetString(this.apellidoPaterno + " " + apellidoMaterno));
            createRecurso.Parameters.AddWithValue("@dt_fecha_nacimiento", this.fechaNacimiento);
            createRecurso.Parameters.AddWithValue("@s1_sexo", this.sexo.intranetSEXO);
            createRecurso.Parameters.AddWithValue("@s50_nacionalidad_recurso", intranetString(this.nacionalidad));
            createRecurso.Parameters.AddWithValue("@s10_estado_civil", this.estadoCivil.intranetEstadoCivil);
            createRecurso.Parameters.AddWithValue("@i_region", this.region.intranetREGION);
            createRecurso.Parameters.AddWithValue("@s7_ciudad_softland", this.ciudad.intranetCiudad);
            createRecurso.Parameters.AddWithValue("@s7_comuna_softland", this.comuna.intranetComuna);
            createRecurso.Parameters.AddWithValue("@s50_comuna_recurso", intranetString(this.comuna.nombreComuna));
            createRecurso.Parameters.AddWithValue("@s100_direccion_recurso", intranetString(this.direccion));
            createRecurso.Parameters.AddWithValue("@bi_telefono_recurso", this.telefono);
            createRecurso.Parameters.AddWithValue("@s75_email_recurso", this.email);
            createRecurso.Parameters.AddWithValue("@s3_id_afp", this.afp.intranetAFP);
            createRecurso.Parameters.AddWithValue("@s3_id_salud", this.isapre.intranetISAPRE);
            if (this.cuentaBancaria)
            {
                createRecurso.Parameters.AddWithValue("@i_id_banco", this.banco.intranetBanco);
                createRecurso.Parameters.AddWithValue("@i_id_tipo_cuenta", this.tipoCuenta.intranetTipoCuenta);
                createRecurso.Parameters.AddWithValue("@s50_numero_cuenta", this.nroCuenta);
            }
            else
            {
                createRecurso.Parameters.AddWithValue("@i_id_banco", 0);
                createRecurso.Parameters.AddWithValue("@i_id_tipo_cuenta", 0);
                createRecurso.Parameters.AddWithValue("@s50_numero_cuenta", "");
            }
        }

        private static string intranetString(string str)
        {
            return str.Replace('Ñ', 'N').Replace('ñ', 'n');
        }


        //OBSOLETO
        public Guid createInCRM(IOrganizationService service)
        {
            Entity colaborador = new Entity("new_colaborador");
            colaborador.Attributes.Add("new_rut", this.crmRut);
            colaborador.Attributes.Add("new_name", this.nombre);
            colaborador.Attributes.Add("new_apellidopaterno", this.apellidoPaterno);
            colaborador.Attributes.Add("new_apellidomaterno", this.apellidoMaterno);
            colaborador.Attributes.Add("new_fechadenacimiento", this.fechaNacimiento);
            //colaborador.Attributes.Add("new_telefono2", this.telefono);
            colaborador.Attributes.Add("new_email", this.email);
            colaborador.Attributes.Add("new_nacionalidad2", this.nacionalidad);
            colaborador.Attributes.Add("new_estadocivil", new OptionSetValue(this.estadoCivil.crmEstadoCivil));
            colaborador.Attributes.Add("new_ciudad", new OptionSetValue(this.ciudad.crmCiudad));
            colaborador.Attributes.Add("new_sexo", this.sexo.crmSEXO);
            colaborador.Attributes.Add("new_comuna", new OptionSetValue(this.comuna.crmComuna));
            colaborador.Attributes.Add("new_direccion", this.direccion);
            colaborador.Attributes.Add("new_afp", new OptionSetValue(this.afp.crmAFP));
            colaborador.Attributes.Add("new_salud", new OptionSetValue(this.isapre.crmISAPRE));
            colaborador.Attributes.Add("new_cuentabancaria2", this.cuentaBancaria);
            if (this.cuentaBancaria)
            {
                colaborador.Attributes.Add("new_cuentabancaria", new OptionSetValue(this.banco.crmBanco));
                colaborador.Attributes.Add("new_tipocuenta", new OptionSetValue(this.tipoCuenta.crmTipoCuenta));
                colaborador.Attributes.Add("new_nmerodecuenta", this.nroCuenta);
            }
            /*
            Entity usuario = Utils.getUserByRut(this.rutUsuario, service);
            colaborador.Attributes.Add("new_reclutadora", usuario.ToEntityReference());
             */
            return service.Create(colaborador);
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


        public void updateColaborador(Entity colaborador, bool overwrite, IOrganizationService service)
        {
            bool update = false;
            if (overwrite)
            {
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_name", this.nombre);
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_apellidopaterno", this.apellidoPaterno);
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_apellidomaterno", this.apellidoMaterno);
                if(this.fechaNacimiento != null && this.fechaNacimiento >= new DateTime(1900,1,1))
                    update = update | Utils.addOrUpdateParameter(ref colaborador, "new_fechadenacimiento", this.fechaNacimiento);
                //Utils.addOrUpdateParameter(ref colaborador, "new_telefono2", this.telefono);
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_email", this.email);
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_nacionalidad2", this.nacionalidad);
                if (this.region != null)
                    update = update | Utils.addOrUpdateParameter(ref colaborador, "new_region", new OptionSetValue(this.region.crmREGION));
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_estadocivil", new OptionSetValue(this.estadoCivil.crmEstadoCivil));
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_ciudad", new OptionSetValue(this.ciudad.crmCiudad));
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_sexo", this.sexo.crmSEXO);
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_comuna", new OptionSetValue(this.comuna.crmComuna));
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_direccion", this.direccion);
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_afp", new OptionSetValue(this.afp.crmAFP));
                if(this.isapre != null)
                    update = update | Utils.addOrUpdateParameter(ref colaborador, "new_salud", new OptionSetValue(this.isapre.crmISAPRE));
                update = update | Utils.addOrUpdateParameter(ref colaborador, "new_cuentabancaria2", this.cuentaBancaria);
                if (this.cuentaBancaria)
                {
                    update = update | Utils.addOrUpdateParameter(ref colaborador, "new_cuentabancaria", new OptionSetValue(this.banco.crmBanco));
                    if (this.tipoCuenta != null && this.tipoCuenta.intranetTipoCuenta != 0)
                        update = update | Utils.addOrUpdateParameter(ref colaborador, "new_tipocuenta", new OptionSetValue(this.tipoCuenta.crmTipoCuenta));
                    update = update | Utils.addOrUpdateParameter(ref colaborador, "new_nmerodecuenta", this.nroCuenta);
                }
                Entity usuario = Utils.getUserByRut(this.rutUsuario, service);
                if (usuario != null)
                    update = update | Utils.addOrUpdateParameter(ref colaborador, "new_reclutadora", usuario.ToEntityReference());
            }
            else
            {
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_name", this.nombre);
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_apellidopaterno", this.apellidoPaterno);
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_apellidomaterno", this.apellidoMaterno);
                if (this.fechaNacimiento != null && this.fechaNacimiento >= new DateTime(1900, 1, 1))
                    update = update | Utils.addIfNotExistParameter(ref colaborador, "new_fechadenacimiento", this.fechaNacimiento);
                //Utils.addIfNotExistParameter(ref colaborador, "new_telefono2", this.telefono);
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_email", this.email);
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_nacionalidad2", this.nacionalidad);
                if (this.region != null)
                    update = update | Utils.addIfNotExistParameter(ref colaborador, "new_region", new OptionSetValue(this.region.crmREGION));
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_estadocivil", new OptionSetValue(this.estadoCivil.crmEstadoCivil));
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_ciudad", new OptionSetValue(this.ciudad.crmCiudad));
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_sexo", this.sexo.crmSEXO);
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_comuna", new OptionSetValue(this.comuna.crmComuna));
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_direccion", this.direccion);
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_afp", new OptionSetValue(this.afp.crmAFP));
                if(this.isapre != null)
                    update = update | Utils.addIfNotExistParameter(ref colaborador, "new_salud", new OptionSetValue(this.isapre.crmISAPRE));
                update = update | Utils.addIfNotExistParameter(ref colaborador, "new_cuentabancaria2", this.cuentaBancaria);
                if (this.cuentaBancaria)
                {
                    update = update | Utils.addIfNotExistParameter(ref colaborador, "new_cuentabancaria", new OptionSetValue(this.banco.crmBanco));
                    if (this.tipoCuenta != null && this.tipoCuenta.intranetTipoCuenta != 0)
                        update = update | Utils.addIfNotExistParameter(ref colaborador, "new_tipocuenta", new OptionSetValue(this.tipoCuenta.crmTipoCuenta));
                    update = update | Utils.addIfNotExistParameter(ref colaborador, "new_nmerodecuenta", this.nroCuenta);
                }
                Entity usuario = Utils.getUserByRut(this.rutUsuario, service);
                if (usuario != null)
                    update = update | Utils.addIfNotExistParameter(ref colaborador, "new_reclutadora", usuario.ToEntityReference());
            }

            update = update | Utils.addOrUpdateParameter(ref colaborador, "new_enintranet", new OptionSetValue(100000002));
            if(update)
                service.Update(colaborador);
        }

        public Entity makeColaborador(IOrganizationService service)
        {
            Entity colaborador = new Entity("new_colaborador");
           
            Utils.addOrUpdateParameter(ref colaborador, "new_rut", this.crmRut);
            Utils.addOrUpdateParameter(ref colaborador, "new_name", this.nombre);
            Utils.addOrUpdateParameter(ref colaborador, "new_apellidopaterno", this.apellidoPaterno);
            Utils.addOrUpdateParameter(ref colaborador, "new_apellidomaterno", this.apellidoMaterno);
            if(this.fechaNacimiento != null && this.fechaNacimiento >= new DateTime(1900, 1, 1))
                Utils.addOrUpdateParameter(ref colaborador, "new_fechadenacimiento", this.fechaNacimiento);
            //Utils.addOrUpdateParameter(ref colaborador, "new_telefono2", this.telefono);
            Utils.addOrUpdateParameter(ref colaborador, "new_email", this.email);
            Utils.addOrUpdateParameter(ref colaborador, "new_nacionalidad2", this.nacionalidad);
            if (this.region != null)
                Utils.addOrUpdateParameter(ref colaborador, "new_region", new OptionSetValue(this.region.crmREGION));
            Utils.addOrUpdateParameter(ref colaborador, "new_estadocivil", new OptionSetValue(this.estadoCivil.crmEstadoCivil));
            Utils.addOrUpdateParameter(ref colaborador, "new_ciudad", new OptionSetValue(this.ciudad.crmCiudad));
            Utils.addOrUpdateParameter(ref colaborador, "new_sexo", this.sexo.crmSEXO);
            Utils.addOrUpdateParameter(ref colaborador, "new_comuna", new OptionSetValue(this.comuna.crmComuna));
            Utils.addOrUpdateParameter(ref colaborador, "new_direccion", this.direccion);
            Utils.addOrUpdateParameter(ref colaborador, "new_afp", new OptionSetValue(this.afp.crmAFP));
            if(this.isapre != null)
                Utils.addOrUpdateParameter(ref colaborador, "new_salud", new OptionSetValue(this.isapre.crmISAPRE));
            Utils.addOrUpdateParameter(ref colaborador, "new_cuentabancaria2", this.cuentaBancaria);
            if (this.cuentaBancaria)
            {
                Utils.addOrUpdateParameter(ref colaborador, "new_cuentabancaria", new OptionSetValue(this.banco.crmBanco));
                if(this.tipoCuenta != null && this.tipoCuenta.intranetTipoCuenta != 0)
                    Utils.addOrUpdateParameter(ref colaborador, "new_tipocuenta", new OptionSetValue(this.tipoCuenta.crmTipoCuenta));
                Utils.addOrUpdateParameter(ref colaborador, "new_nmerodecuenta", this.nroCuenta);
            }
            Entity usuario = Utils.getUserByRut(this.rutUsuario, service);
            if (usuario != null)
                Utils.addOrUpdateParameter(ref colaborador, "new_reclutadora", usuario.ToEntityReference());
            Utils.addOrUpdateParameter(ref colaborador, "new_enintranet", new OptionSetValue(100000002));
            return colaborador;
        }
    }
}
