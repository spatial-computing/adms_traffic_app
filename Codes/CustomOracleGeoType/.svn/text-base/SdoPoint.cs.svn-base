/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Customize Oracle Sdo_Geometry Type 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Types;
using Oracle.MyGeoSpace;


namespace CustomOracleGeoType
{

        [OracleCustomTypeMappingAttribute("MDSYS.SDO_POINT_TYPE")]
        public class SdoPoint : OracleCustomTypeBase<SdoPoint>
        {
            private decimal? x;

            [OracleObjectMappingAttribute("X")]
            public decimal? X
            {
                get { return x; }
                set { x = value; }
            }

            private decimal? y;

            [OracleObjectMappingAttribute("Y")]
            public decimal? Y
            {
                get { return y; }
                set { y = value; }
            }

            private decimal? z;

            [OracleObjectMappingAttribute("Z")]
            public decimal? Z
            {
                get { return z; }
                set { z = value; }
            }

            public override void MapFromCustomObject()
            {
                SetValue("X", x);
                SetValue("Y", y);
                SetValue("Z", z);
            }

            public override void MapToCustomObject()
            {
                X = GetValue<decimal?>("X");
                Y = GetValue<decimal?>("Y");
                Z = GetValue<decimal?>("Z");
            }
        }
    
}
