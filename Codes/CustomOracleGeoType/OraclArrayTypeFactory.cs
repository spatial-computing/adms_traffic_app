/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Customize Oracle Sdo_Geometry Type 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace Oracle.MyGeoSpace
{
    public abstract class OracleArrayTypeFactoryBase<T> : IOracleArrayTypeFactory
    {
        public Array CreateArray(int numElems)
        {
            return new T[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}
