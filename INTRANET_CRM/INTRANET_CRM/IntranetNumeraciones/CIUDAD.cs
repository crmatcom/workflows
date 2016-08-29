using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM.IntranetNumeraciones
{
    class CIUDAD
    {
        static public string SANTIAGO = "001";
        static public string VALPARAISO = "002";
        static public string LA_SERENA = "003";
        static public string CONCEPCION = "004";
        static public string ARICA = "005";
        static public string ANTOFAGASTA = "006";
        static public string PUERTO_MONTT = "007";
        static public string LOS_ANDES = "008";
        static public string IQUIQUE = "009";
        static public string VINA_DEL_MAR = "010";
        static public string LA_CALERA = "011";
        static public string CHILLAN = "012";
        static public string CALAMA = "031";
        static public string TARAPACA = "035";
        static public string COPIAPO = "036";
        static public string NUBLE = "038";
        static public string PUNTA_ARENAS = "040";
        static public string LINARES = "055";
        static public string COYHAIQUE = "120";
        static public string COQUIMBO = "130";
        static public string TALCA = "132";
        static public string TEMUCO = "134";
        static public string VALDIVIA = "140";
        static public string RANCAGUA = "458";
        static public string PENCO = "578";
        static public string MELIPILLA = "745";
        //static public string SANTIAGO = "SAN";

        static public int CRM_SANTIAGO = 100000000;
        static public int CRM_VALPARAISO = 100000001;
        static public int CRM_LA_SERENA = 100000002;
        static public int CRM_CONCEPCION = 100000003;
        static public int CRM_ARICA = 100000004;
        static public int CRM_ANTOFAGASTA = 100000005;
        static public int CRM_PUERTO_MONTT = 100000006;
        static public int CRM_LOS_ANDES = 100000007;
        static public int CRM_IQUIQUE = 100000008;
        static public int CRM_VINA_DEL_MAR = 100000009;
        static public int CRM_LA_CALERA = 100000010;
        static public int CRM_CHILLAN = 100000011;
        static public int CRM_CALAMA = 100000012;
        static public int CRM_TARAPACA = 100000013;
        static public int CRM_COPIAPO = 100000014;
        static public int CRM_NUBLE = 100000015;
        static public int CRM_PUNTA_ARENAS = 100000016;
        static public int CRM_LINARES = 100000017;
        static public int CRM_COYHAIQUE = 100000018;
        static public int CRM_COQUIMBO = 100000019;
        static public int CRM_TALCA = 100000020;
        static public int CRM_TEMUCO = 100000021;
        static public int CRM_VALDIVIA = 100000022;
        static public int CRM_RANCAGUA = 100000023;
        static public int CRM_PENCO = 100000024;
        static public int CRM_MELIPILLA = 100000025;
        //static public int CRM_SANTIAGO = 100000026;

        public string intranetCiudad { get; private set; }
        public int crmCiudad { get; private set; }

        static private Dictionary<string, int> intranetToCRM = new Dictionary<string, int>
        {
            { "001", 100000000 },
            { "002", 100000001 },
            { "003", 100000002 },
            { "004", 100000003 },
            { "005", 100000004 },
            { "006", 100000005 },
            { "007", 100000006 },
            { "008", 100000007 },
            { "009", 100000008 },
            { "010", 100000009 },
            { "011", 100000010 },
            { "012", 100000011 },
            { "031", 100000012 },
            { "035", 100000013 },
            { "036", 100000014 },
            { "038", 100000015 },
            { "040", 100000016 },
            { "055", 100000017 },
            { "120", 100000018 },
            { "130", 100000019 },
            { "132", 100000020 },
            { "134", 100000021 },
            { "140", 100000022 },
            { "458", 100000023 },
            { "578", 100000024 },
            { "745", 100000025 },
            { "SAN", 100000026 }

        };

        private static Dictionary<int, string> crmToIntranet = new Dictionary<int, string>
        {
            { 100000000, "001" },
            { 100000001, "002" },
            { 100000002, "003" },
            { 100000003, "004" },
            { 100000004, "005" },
            { 100000005, "006" },
            { 100000006, "007" },
            { 100000007, "008" },
            { 100000008, "009" },
            { 100000009, "010" },
            { 100000010, "011" },
            { 100000011, "012" },
            { 100000012, "031" },
            { 100000013, "035" },
            { 100000014, "036" },
            { 100000015, "038" },
            { 100000016, "040" },
            { 100000017, "055" },
            { 100000018, "120" },
            { 100000019, "130" },
            { 100000020, "132" },
            { 100000021, "134" },
            { 100000022, "140" },
            { 100000023, "458" },
            { 100000024, "578" },
            { 100000025, "745" },
            { 100000026, "SAN" }

        };



        public static int getCRMCiudad(string intranetCiudad)
        {
            return intranetToCRM[intranetCiudad];
        }

        public static string getIntranetCiudad(int crmCiudad)
        {
            return crmToIntranet[crmCiudad];
        }

        public CIUDAD(string intranetCIUDAD)
        {
            this.intranetCiudad = intranetCIUDAD;
            this.crmCiudad = getCRMCiudad(intranetCIUDAD);
        }

        public CIUDAD(int crmCIUDAD)
        {
            this.crmCiudad = crmCIUDAD;
            this.intranetCiudad = getIntranetCiudad(crmCIUDAD);
        }

    }
}
