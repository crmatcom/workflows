using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM.IntranetNumeraciones
{
    class EstadoCivil
    {

        public static string SOLTERO = "S";
        public static string CASADO = "C";
        public static string VIUDO = "V";

        public static int CRM_SOLTERO = 100000000;
        public static int CRM_CASADO = 100000001;
        public static int CRM_VIUDO = 100000003;

        public string intranetEstadoCivil { get; private set; }
        public int crmEstadoCivil { get; private set; }

        public static string getIntranetEstadoCivil(int crmEstadoCivil)
        {
            switch (crmEstadoCivil)
            {
                case 100000000: return SOLTERO;
                case 100000001: return CASADO;
                case 100000003: return VIUDO;
            }
            throw new EstadoCivilException("No Existe estado civil en CRM:" + crmEstadoCivil);
        }

        public static int getCrmEstadoCivil(string intranetEstadoCivil)
        {
            switch (intranetEstadoCivil)
            {
                case "S": return CRM_SOLTERO;
                case "C": return CRM_CASADO;
                case "V": return CRM_VIUDO;
            }
            throw new EstadoCivilException("No existe estado civil en intranet");
        }

        public EstadoCivil(string intranetEstadoCivil)
        {
            this.intranetEstadoCivil = intranetEstadoCivil;
            this.crmEstadoCivil = getCrmEstadoCivil(intranetEstadoCivil);
        }

        public EstadoCivil(int crmEstadoCivil)
        {
            this.crmEstadoCivil = crmEstadoCivil;
            this.intranetEstadoCivil = getIntranetEstadoCivil(crmEstadoCivil);
        }

    }
}
