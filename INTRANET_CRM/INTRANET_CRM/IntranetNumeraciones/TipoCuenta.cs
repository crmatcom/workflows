using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INTRANET_CRM.Model
{
    class TipoCuenta
    {
        static public int CUENTA_CORRIENTE = 1;
        static public int CUENTA_VISTA = 2;
        static public int CUENTA_DE_AHORRO = 3;
        static public int CHEQUERA_ELECTRONICA = 4;
        static public int CUENTA_RUT = 5;

        static public int CRM_CUENTA_CORRIENTE = 100000000;
        static public int CRM_CUENTA_VISTA = 100000001;
        static public int CRM_CUENTA_DE_AHORRO = 100000002;
        static public int CRM_CHEQUERA_ELECTRONICA = 100000003;
        static public int CRM_CUENTA_RUT = 100000004;

        public int crmTipoCuenta { get; private set; }
        public int intranetTipoCuenta { get; private set; }

        public static int getCrmTipoCuenta(int intranetTipoCuenta)
        {
            return intranetTipoCuenta + 99999999;
        }

        public static int getIntranetTipoCuenta(int crmTipoCuenta)
        {
            return crmTipoCuenta - 99999999;
        }

        public TipoCuenta(int tipoCuenta, bool crmTipoCuenta)
        {
            if (crmTipoCuenta)
            {
                this.crmTipoCuenta = tipoCuenta;
                this.intranetTipoCuenta = getIntranetTipoCuenta(tipoCuenta);
            }
            else
            {
                this.intranetTipoCuenta = tipoCuenta;
                this.crmTipoCuenta = getCrmTipoCuenta(tipoCuenta);
            }
        }



    }
}
