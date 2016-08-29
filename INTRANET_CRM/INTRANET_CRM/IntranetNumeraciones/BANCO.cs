using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INTRANET_CRM.Model
{
    class BANCO
    {
        public static int BANCO_DE_CHILE = 7;
        public static int BBVA = 3;
        public static int BCI = 11;
        public static int BICE = 4;
        public static int CONSORCIO = 8;
        public static int CORPBANCA = 10;
        public static int DESARROLLO = 12;
        public static int EDWARDS_CITI = 9;
        public static int ESTADO = 13;
        public static int FALABELLA = 14;
        public static int HSBC_BANK = 19;
        public static int INTERNACIONAL = 5;
        public static int ITAU = 6;
        public static int PARIS = 21;
        public static int PENTA = 22;
        public static int RABOBANK = 18;
        public static int RIPLEY = 20;
        public static int SANTANDER = 1;
        public static int SANTANDER_BANEFE = 2;
        public static int SCOTIABANK = 17;
        public static int SECURITY = 15;
        public static int TBANC = 16;

        public static int CRM_BANCO_DE_CHILE = 100000006;
        public static int CRM_BBVA = 100000002;
        public static int CRM_BCI = 100000010;
        public static int CRM_BICE = 100000003;
        public static int CRM_CONSORCIO = 100000007;
        public static int CRM_CORPBANCA = 100000009;
        public static int CRM_DESARROLLO = 100000011;
        public static int CRM_EDWARDS_CITI = 100000008;
        public static int CRM_ESTADO = 100000012;
        public static int CRM_FALABELLA = 100000013;
        public static int CRM_HSBC_BANK = 100000018;
        public static int CRM_INTERNACIONAL = 100000004;
        public static int CRM_ITAU = 100000005;
        public static int CRM_PARIS = 100000020;
        public static int CRM_PENTA = 100000021;
        public static int CRM_RABOBANK = 100000017;
        public static int CRM_RIPLEY = 100000019;
        public static int CRM_SANTANDER = 100000000;
        public static int CRM_SANTANDER_BANEFE = 100000001;
        public static int CRM_SCOTIABANK = 100000016;
        public static int CRM_SECURITY = 100000014;
        public static int CRM_TBANC = 100000015;

        public int crmBanco { get; private set; }
        public int intranetBanco { get; private set; }

        public static int getIntranetBanco(int crmBanco)
        {
            return crmBanco - 99999999;
        }

        public static int getCrmBanco(int intranetBanco)
        {
            return intranetBanco + 99999999;
        }

        public BANCO(int banco, bool crmBanco)
        {
            if (crmBanco)
            {
                this.crmBanco = banco;
                this.intranetBanco = getIntranetBanco(banco);
            }
            else
            {
                this.intranetBanco = banco;
                this.crmBanco = getCrmBanco(banco);
            }
        }

    }
}
