﻿/**
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
    public class ArterialInventoryParser : InventoryParser
    {
        public ArterialInventoryParser(string agency)
            : base(agency, SourceDataType.Arterial.ToString())
        {

        }
        public override string FetchData()
        {
            return WSDLConnector("congestionArterial", agency, "inventory");
        }
    }
}
