using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTRANET_CRM.IntranetNumeraciones
{
    class REGION
    {
        /*
            CL	1	PRIMERA REGION (DE TARAPACA)	I	I de Tarapacá	1000000
            CL	2	SEGUNDA REGION (DE ANTOFAGASTA)	II	II de Antofagasta	1000001
            CL	3	TERCERA REGION (DE ATACAMA)	III	III de Atacama	1000002
            CL	4	CUARTA REGION (DE COQUIMBO)	IV	IV de Coquimbo	1000003
            CL	5	QUINTA REGION (DE VALPARAISO)	V	V de Valparaíso	1000004
            CL	6	SEXTA REGION (DEL LIBERTADOR B.O HIGGINS)	VI	VI del Libertador General Bernardo O'Higgins	1000005
            CL	7	SEPTIMA REGION (DEL MAULE)	VII	VII del Maule	1000006
            CL	8	OCTAVA REGION (DEL BIO-BIO)	VIII	VIII del Bío Bío	1000007
            CL	9	NOVENA REGION (DE LA ARAUCANIA)	IX	IX de la Araucanía	1000008
            CL	10	DECIMA REGION (DE LOS LAGOS)	X	X de los Lagos	1000009
            CL	11	UNDECIMA REGION (DE AISEN DEL GENERAL CARLOS IBANEZ DEL CAM)	XI	XI Aisén del General Carlos Ibáñez del Campo	1000010
            CL	12	DUODECIMA REGION (DE MAGALLANES Y DE LA ANTARTICA CHILENA)	XII	XII de Magallanes y Antártica Chilena	1000011
            CL	13	REGION METROPOLITANA (DE SANTIAGO)	RM	Metropolitana de Santiago	1000012
            CL	14	DECIMO CUARTA REGION(REGION DE LOS RIOS)	XIV	XIV de los Ríos	1000013
            CL	15	DECIMO QUINTA REGION(ARICA  PARINACOTA)	XV	XV de Arica y Parinacota	1000014
         */

        public int intranetREGION { get; private set; }
        public int crmREGION { get; private set; }

        public static int getIntranetRegion(int crmRegion)
        {
            return crmRegion - 99999999;
        }

        public static int getCrmRegion(int intranetRegion)
        {
            return intranetRegion + 99999999;
        }

        public REGION(int region, bool regionCRM)
        {
            if (regionCRM)
            {
                this.crmREGION = region;
                this.intranetREGION = getIntranetRegion(region);
            }
            else
            {
                this.intranetREGION = region;
                this.crmREGION = getCrmRegion(region);

            }
        }

    }
}
