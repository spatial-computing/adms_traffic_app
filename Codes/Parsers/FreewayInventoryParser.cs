/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;

namespace Parsers
{
    public class FreewayInventoryParser: InventoryParser
    {
        public FreewayInventoryParser(string agency) : base(agency, SourceDataType.Freeway.ToString())
        {

        }
        public override string FetchData()
        {
            return WSDLConnector("congestionFreeway", agency, "inventory");
        }
    }
}
