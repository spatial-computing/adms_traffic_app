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
    public class RampParser : BaseFileParser
    {
        public RampParser(string agency)
            : base(agency, "rampMeterListStatus", SourceDataType.Ramp.ToString())
        {
            expectedFieldNum = 9;
            del = FetchData;
        }

        
        public override List<string> ReadARecord()
        {
            var result = new List<string>();
            
            for (int i = 0; i < expectedFieldNum; i++)
            {
                do
                {
                    textReader.Read();
                } while (textReader.NodeType == XmlNodeType.Element || textReader.NodeType == XmlNodeType.EndElement);

                if (textReader.NodeType == XmlNodeType.Text)
                    result.Add(textReader.Value);
            }

            bool goOn = true;
            string link_ids = "";
            string dectorTypes = "";
            string os = "";
            string ss = "";
            string vs = "";
            string statuses = "";
            while (goOn && textReader.Read())
            {
                switch (textReader.Name)
                {
                    case "rampMeterStatus":
                        if (textReader.NodeType == XmlNodeType.EndElement)
                            goOn = false;
                        break;
                    case ("id"):
                        textReader.Read();
                        link_ids += textReader.Value + ",";
                        if (textReader.Value.Length > 0)
                            textReader.Read();   
                        break;
                    case ("detectorType"):
                        textReader.Read();
                        dectorTypes += textReader.Value + ",";
                        if (textReader.Value.Length > 0)
                            textReader.Read();   
                        break;
                    case ("occupancy"):
                        textReader.Read();
                        os += textReader.Value + ",";
                        if (textReader.Value.Length > 0)
                            textReader.Read();   
                            break;
                    case ("speed"):
                        textReader.Read();
                        if (textReader.Value.Length > 0)
                        {
                            int sp = Convert.ToInt16(textReader.Value);
                            sp = (int)(sp / 1.609344);
                            ss +=sp.ToString() + ",";
                            textReader.Read();
                        }
                            break;
                    case ("volume"):
                        textReader.Read();
                        vs += textReader.Value + ",";
                        if (textReader.Value.Length > 0)
                            textReader.Read();   
                            break;
                    case ("linkDataStatus"):
                        textReader.Read();
                        statuses += textReader.Value + ",";
                        if (textReader.Value.Length > 0)
                            textReader.Read();   
                            break;
                }
            }
            result.Add(link_ids);
            result.Add(dectorTypes);
            result.Add(os);
            result.Add(ss);
            result.Add(vs);
            result.Add(statuses);
            textReader.Read();

            return result;
        }

        public string FetchData()
        {
            // return FreewayDataReplayer(); //todo: comment later
            return WSDLConnector("rms", agency, "real-time");
        }
    }
}
