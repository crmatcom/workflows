using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INTRANET_CRM.IntranetNumeraciones;

namespace INTRANET_CRM
{
    class AFP
    {
        public static string SUMMA_BANSANDER = "001";      //   
        public static string Aporte_Fomenta = "002";       //   
        public static string CUPRUM = "003";               //   
        public static string HABITAT = "004";              //   
        public static string PROVIDA = "006";              //
        public static string PLANVITAL = "007";            //
        public static string Magister = "008";             //
        public static string ING_AFP_CAPITAL = "009";      //
        public static string AFP_MODELO = "010";           //  
        public static string PENSIONADOS_S_AFP = "9";

        public static int CRM_SUMMA_BANSANDER = 100000008;      //   
        public static int CRM_Aporte_Fomenta = 100000009;       //   
        public static int CRM_CUPRUM = 100000005;               //   
        public static int CRM_HABITAT = 100000001;              //   
        public static int CRM_PROVIDA = 100000003;              //
        public static int CRM_PLANVITAL = 100000004;            //
        public static int CRM_Magister = 100000007;             //
        public static int CRM_ING_AFP_CAPITAL = 100000002;      //
        public static int CRM_AFP_MODELO = 100000000;           //  
        public static int CRM_PENSIONADOS_S_AFP = 100000006;    //

        public string intranetAFP { get; private set; }
        public int crmAFP { get; private set; }
        
        public static string getIntranetAFP(int crmAFP)
        {
            switch (crmAFP)
            {
                case 100000008: return SUMMA_BANSANDER;
                case 100000009: return Aporte_Fomenta;
                case 100000005: return CUPRUM;
                case 100000001: return HABITAT;
                case 100000003: return PROVIDA;
                case 100000004: return PLANVITAL;
                case 100000007: return Magister;
                case 100000002: return ING_AFP_CAPITAL;
                case 100000000: return AFP_MODELO;
                case 100000006: return PENSIONADOS_S_AFP;

            }
            
            throw new AFPException("AFP CRM invalida");
        }

        public static int getCrmAFP(string intranetAFP)
        {
            switch (intranetAFP)
            {
                case "001": return CRM_SUMMA_BANSANDER;      
                case "002": return CRM_Aporte_Fomenta;
                case "003": return CRM_CUPRUM;
                case "004": return CRM_HABITAT;
                case "006": return CRM_PROVIDA;
                case "007": return CRM_PLANVITAL;
                case "008": return CRM_Magister;
                case "009": return CRM_ING_AFP_CAPITAL;
                case "010": return CRM_AFP_MODELO;
                case "9": return CRM_PENSIONADOS_S_AFP;

            }

            throw new AFPException("AFP intranet invalida");
        }

        public AFP(string intranetAFP)
        {
            this.intranetAFP = intranetAFP;
            this.crmAFP = getCrmAFP(intranetAFP);
        }

        public AFP(int crmAFP)
        {
            this.crmAFP = crmAFP;
            this.intranetAFP = getIntranetAFP(crmAFP);
        }

    }
}
