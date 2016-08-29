using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INTRANET_CRM.IntranetNumeraciones;

namespace INTRANET_CRM
{
    class SEXO
    {
        public static string MASCULINO = "M";
        public static string FEMENINO = "F";

        public static bool CRM_MASCULINO = false;
        public static bool CRM_FEMENINO = true;

        public string intranetSEXO { get; private set; }
        public bool crmSEXO { get; private set; }

        public SEXO(string intranetSEXO){
            this.intranetSEXO = intranetSEXO;
            this.crmSEXO = getCrmSexo(intranetSEXO);
        }

        public SEXO(bool crmSEXO)
        {
            this.crmSEXO = crmSEXO;
            this.intranetSEXO = getIntranetSexo(crmSEXO);
        }

        static public string getIntranetSexo(bool crmSexo)
        {
            if (crmSexo)
                return "F";
            else if (!crmSexo)
                return "M";
            else
                throw new SEXOException("SEXO NO EXISTE");
        }

        static public bool getCrmSexo(string intranetSexo)
        {
            if (intranetSexo == "F")
                return true;
            else if (intranetSexo == "M")
                return false;
            else
                throw new SEXOException("SEXO NO EXISTE");
        }

    }
}
