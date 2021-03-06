﻿/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using EventTypes;

namespace Parsers
{
    public class RailInventory : BaseFileParser
    {
        public RailInventory(string agency)
            : base(agency, "trains", SourceDataType.Rail.ToString())
        {
            expectedFieldNum = 2;
            del = FetchData;
        }

        public RailRoute ReadARailRouteInfo()
        {
            return new RailRoute(agency, ReadARecord());
        }

        
        public override List<string> ReadARecord()
        {
            var result = new List<string>(3);

            bool goOn = true;
            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "train":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case "routeId":
                    case "routeDescription":
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
            return WSDLConnector("rail", agency, "inventory");
        }
    }
}
