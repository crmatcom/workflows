using INTRANET_CRM.IntranetNumeraciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM
{
    class ISAPRE
    {
        public static string Banmedica_SA = "001";
        public static string BANMEDICA = "002";
        public static string CRUZ_BLANCA = "003";
        public static string CONSALUD = "004";
        public static string VIDA_TRES = "006";
        public static string COLMENA_GOLDEN_CROSS = "008";
        public static string SFERA = "009";
        public static string ISAPRE_LA_ARAUCANA = "011";
        public static string MAS_VIDA = "012";
        public static string ISAPRE_MASTER_SALUD = "013";
        public static string ISAPRE_LINK_SALUD = "014";
        public static string ISAPRE_FUSAT = "015";
        public static string FERROSALUD_SA = "019";
        public static string ISAPRE_VIDA_PLENA = "17";
        public static string ISAPRE_ISTEL = "18";
        public static string FONASA_IPS = "500";

        public static int CRM_Banmedica_SA = 100000001;
        public static int CRM_BANMEDICA = 100000001;
        public static int CRM_CRUZ_BLANCA = 100000002;
        public static int CRM_CONSALUD = 100000006;
        public static int CRM_VIDA_TRES = 100000004;
        public static int CRM_COLMENA_GOLDEN_CROSS = 100000005;
        public static int CRM_SFERA = 100000007;
        public static int CRM_ISAPRE_LA_ARAUCANA = 100000008;
        public static int CRM_MAS_VIDA = 100000003;
        public static int CRM_ISAPRE_MASTER_SALUD = 100000009;
        public static int CRM_ISAPRE_LINK_SALUD = 100000010;
        public static int CRM_ISAPRE_FUSAT = 100000011;
        public static int CRM_FERROSALUD_SA = 100000012;
        public static int CRM_ISAPRE_VIDA_PLENA = 100000013;
        public static int CRM_ISAPRE_ISTEL = 100000014;
        public static int CRM_FONASA_IPS = 100000000;

        public string intranetISAPRE { get; private set; }
        public int crmISAPRE { get; private set; }

        public static int getCRMISAPRE(string intranetISAPRE)
        {
            switch (intranetISAPRE)
            {
                case "001": return 100000001;
                case "002": return 100000001;
                case "003": return 100000002;
                case "004": return 100000006;
                case "006": return 100000004;
                case "008": return 100000005;
                case "009": return 100000007;
                case "011": return 100000008;
                case "012": return 100000003;
                case "013": return 100000009;
                case "014": return 100000010;
                case "015": return 100000011;
                case "019": return 100000012;
                case "17": return 100000013;
                case "18": return 100000014;
                case "500": return 100000000;
            }
            throw new ISAPREException("NO EXISTE ISAPRE EN CRM:" + intranetISAPRE);
        }

        public static string getIntranetISAPRE(int crmISAPRE)
        {
            switch (crmISAPRE)
            {
                case 100000001: return "002";
                case 100000002: return "003";
                case 100000006: return "004";
                case 100000004: return "006";
                case 100000005: return "008";
                case 100000007: return "009";
                case 100000008: return "011";
                case 100000003: return "012";
                case 100000009: return "013";
                case 100000010: return "014";
                case 100000011: return "015";
                case 100000012: return "019";
                case 100000013: return "17";
                case 100000014: return "18";
                case 100000000: return "500";

            }
            throw new ISAPREException("NO EXISTE ISAPRE EN INTRANET:" + crmISAPRE);
        }

        public ISAPRE(string intranetISAPRE)
        {
            this.intranetISAPRE = intranetISAPRE;
            this.crmISAPRE = getCRMISAPRE(intranetISAPRE);
        }

        public ISAPRE(int crmISAPRE)
        {
            this.intranetISAPRE = getIntranetISAPRE(crmISAPRE);
            this.crmISAPRE = crmISAPRE;
        }

    }
}
