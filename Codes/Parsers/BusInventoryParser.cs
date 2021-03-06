﻿/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Parser;
using EventTypes;

namespace Parsers
{
    public class BusInventoryParser : BaseFileParser
    {
        public BusInventoryParser(string agency)
            : base(agency, "routes", SourceDataType.Bus.ToString())
        {
            expectedFieldNum = 3;
            del = FetchData;
        }

        public BusRoute ReadABusRouteInfo()
        {
            return new BusRoute(agency, ReadARecord());
        }

        public override List<string> ReadARecord()
        {
            var result = new List<string>(3);

            bool goOn = true;
            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "route":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case "routeId":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                    case "routeDescription":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag                       
                        break;
                    case "routeZones":
                        textReader.Read();
                        result.Add(textReader.Value);
                        if (textReader.Value.Length > 0)
                            textReader.Read(); //closing tag
                        break;
                }
            }
            return result;
        }

        public string FetchData()
        {
            return WSDLConnector("bus", agency, "inventory");
        }
    }
}
