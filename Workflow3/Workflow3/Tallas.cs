using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATCOM_jtoro
{
    public class Tallas
    {

        public static int getTallaZapato(int talla)
        {
            return talla - 100000001 + 35;
        }

        public static int getTallaPantalon(int talla)
        {
            switch (talla)
            {
                case 100000011: return 58;
                case 100000010: return 60;
                default: return (talla - 100000000 + 19) * 2;
            }
        }

        public static String getTallaPolera(int talla)
        {
            switch (talla)
            {
                case 100000000: return "S";
                case 100000001: return "M";
                case 100000002: return "L";
                case 100000003: return "XL";
                case 100000004: return "XXL";
                case 100000005: return "XXXL";
                default: return null;
            }
        }

    }
}
