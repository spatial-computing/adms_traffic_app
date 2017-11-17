/**
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
    public class BusParser : BaseFileParser
    {
        public BusParser(string agency)
            : base(agency, "buses", SourceDataType.Bus.ToString())
        {
            expectedFieldNum = 13;
            del = FetchData;
        }


        public override List<string> ReadARecord()
        {
            var result = new List<string>();
            /*for (int i = 0; i < expectedFieldNum; i++)
            {
                do
                {
                    textReader.Read();
                } while (textReader.NodeType == XmlNodeType.Element || textReader.NodeType == XmlNodeType.EndElement);

                if (textReader.NodeType == XmlNodeType.Text)
                    result.Add(textReader.Value);
            }*/

            bool endofRecord = false;

            while (!endofRecord)
            {
                textReader.Read();

                if (textReader.NodeType == XmlNodeType.Element)
                {
                    switch (textReader.Name)
                    {

                        case "busId":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "lineId":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "runId":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "routeId":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "routeDescription":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "direction":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "atLongitude":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "atLatitude":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "busLocationTime":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "scheduleDeviation":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "arrivalNextTP":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "nextTimepointLocation":
                            textReader.Read();
                            result.Add(textReader.Value);
                            break;
                        case "timepointTime":
                            textReader.Read();
                            result.Add(textReader.Value);
                            //endofRecord = true;
                            break;
                        case "brtFlag":
                            textReader.Read();
                            result.Add(textReader.Value);
                            endofRecord = true;
                            break;
                    } // switch
                } // if
            } // while
            textReader.Read(); // /timepointTime
            textReader.Read(); // /bus

            return result;
        }

        public string FetchData()
        {
            // return FreewayDataReplayer(); //todo: comment later
            return WSDLConnector("bus", agency, "real-time");
        }
    }
}
