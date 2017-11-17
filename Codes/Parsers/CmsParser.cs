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
    public class CmsParser : BaseFileParser
    {
        public CmsParser(string agency)
            : base(agency, "dmsListDeviceStatus", SourceDataType.Cms.ToString())
        {
            expectedFieldNum = 11;
            del = FetchData;
        }


        public override List<string> ReadARecord()
        {
            var result = new List<string>();
   
                bool endofRecord = false;

                while (!endofRecord)
                {
                    textReader.Read();

                    if (textReader.NodeType == XmlNodeType.Element)
                    {
                        switch(textReader.Name)
                        {

                            case "id":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "dms-device-status":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "dmsState":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "date":
                                textReader.Read();
                                
                                result.Add(textReader.Value);
                                break;
                            case "time":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "phase1Line1":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "phase1Line2":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "phase1Line3":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "phase2Line1":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "phase2Line2":
                                textReader.Read();
                                result.Add(textReader.Value);
                                break;
                            case "phase2Line3":
                                textReader.Read();
                                result.Add(textReader.Value);
                                endofRecord = true;
                                break;

                        } // switch
                    } // if
                } // while

           textReader.Read(); //
           textReader.Read(); //
           for (int i = 0; i < result.Count; i++ )
           {
               if(result[i].Contains("\'"))
               {
                   result[i] = result[i].Replace('\'', '\"');
               }
           }
           return result;
        }

        public string FetchData()
        {
            // return FreewayDataReplayer(); //todo: comment later
            return WSDLConnector("cms", agency, "real-time");
            
        }
    }
}
