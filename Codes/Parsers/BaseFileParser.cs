/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: 
 * 1. Delete WSDL(Congestion/Event)Connector, integrate WSDLConnector into one function
 * 2. Update all the references of WSDL(Congestion/Event)Connector
 * 2. Add Web services
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Parser;
using Parsers.CongestionService1;
using Parsers.IAI_EventService;
using Parsers.BusService1;
using Parsers.RailService1;
using Parsers.RampService1;
using Parsers.TravelTimeService1;
using Parsers.CmsServiceService1;
using ExceptionReporter;

namespace Parsers
{
    public abstract class BaseFileParser
    {
        #region Delegates

        public delegate string DataFetcher();

        #endregion

        public string StartTag;
        public string DataType;
        protected string all;
        protected DataFetcher del;
        protected int expectedFieldNum;
        protected bool fileLoaded;
        protected XmlTextReader textReader;
        public string agency;
        protected long last_updated_time;
        protected long last_updated_day;

        public BaseFileParser(string agency, string startTag, string dataType)
        {
            this.agency = agency;
            StartTag = startTag;
            last_updated_time = 0;
            DataType = dataType;
        }
        protected bool NoData(string all)
        {
            return all == null || all == "";
        }

        private int tempJalal = 0;
        public void Init()
        {
            all = del();

            //TextWriter tempJJTextWriter = new StreamWriter(@"C:\just for test\orig\" + tempJalal + ".xml");
            //tempJJTextWriter.Write(all);
            //tempJalal++;
            //tempJJTextWriter.Close();
            //var temp = new StreamWriter(@"c:\jj.xml");
            //temp.Write(all);
            //temp.Close();
            if (NoData(all))
            {
                fileLoaded = false;

                return;
            }
            textReader = new XmlTextReader(new StringReader(all));
            fileLoaded = true;
        }

        public abstract List<string> ReadARecord();

        public DateTime SpringOverHeaders(CultureInfo _culture)
        {
            DateTime packetDataTime = DateTime.MinValue;
            DateTime date = DateTime.MinValue, time;

            bool isValidDate = true;

            bool IsFirstTime = true;

            bool goOn = true;
            try
            {
                while (goOn && textReader.Read())
                {
                    if (textReader.Name == StartTag)
                        goOn = false;
                    else if (textReader.Name == "date")
                    {
                        textReader.Read();
                        //if (IsFirstDate)
                        {
                            try
                            {
                                date = DateTime.ParseExact(textReader.Value, "yyyyMMdd",
                                                   _culture);
                                isValidDate = true;
                            }
                            catch (Exception)
                            {
                                //Invalid date information

                                return default(DateTime);
                                //isValidDate = false;
                                
                            }
                            
                            //IsFirstDate = false;
                        }
                        textReader.Read(); //closing tag
                    }
                    else if (textReader.Name == "time")
                    {
                        textReader.Read();

                        //if (IsUpdateTime)
                        {
                            try
                            {
                                time = DateTime.ParseExact(textReader.Value, "HHmmssff", _culture);
                            }
                            catch (Exception)
                            {
                                return default(DateTime);
                                //packetDataTime = default(DateTime);
                                //isValidDate = false;
                                //continue;
                            }

                            try
                            {
                                packetDataTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                            }
                            catch
                            {
                                return default(DateTime);
                            }

                            //Console.WriteLine("Query Time for " + StartTag + " :" + time);

                            if (!IsFirstTime)
                            {
                                Console.WriteLine("Updated Time for "+ DataType + " :" + time);

                                long cur_day = date.Month*100 + date.Day;
                                long cur_time = time.Hour * 60 * 60 + time.Minute * 60 + time.Second;

                                if (cur_day < last_updated_day)
                                {
                                    Console.WriteLine("Duplicates in BaseFileParser: " + DataType);

                                    FileExpender(DataType + "ErrorLog.txt", cur_time.ToString() + "," + last_updated_time.ToString());

                                    ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter("", 0, "BaseFileParser.cs", DataType, "SI");
                                    reporter2.SendDatabaseDuplicates();

                                    return default(DateTime);
                                }
                                else if (time.Hour < 3 && last_updated_time > 12 * 60 * 60)
                                {
                                    if(cur_day>=last_updated_day)
                                    {
                                        last_updated_time = cur_time;
                                        last_updated_day = cur_day;
                                    }
                                
                                }
                                else if (cur_time <= last_updated_time && cur_day<= last_updated_day)     // < or <=
                                {
                                    Console.WriteLine("Duplicates in BaseFileParser: " + DataType);

                                    FileExpender(DataType + "ErrorLog.txt", cur_time.ToString() + "," + last_updated_time.ToString());

                                    ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter("", 0, "BaseFileParser.cs", DataType, "SI");
                                    reporter2.SendDatabaseDuplicates();

                                    return default(DateTime);
                                    //continue;
                                }
                                else
                                {
                                    last_updated_time = cur_time;
                                    last_updated_day = cur_day;
                                }
                                
                            }
                            IsFirstTime = false;
                            
                        }
                        textReader.Read(); //closing tag
                    }
                }
            }
            catch (XmlException e)
            {
                // root element not found (because the file is empty)
                return default(DateTime);
            }
            //if(DataType == "Bus")
            //    Console.WriteLine("BUS" + packetDataTime);
            return packetDataTime;
        }

        public void FileExpender(string fileName, string content)
        {
            //Added by Penny
            try
            {
                StreamWriter tempWriter = new StreamWriter(fileName, true);
                tempWriter.WriteLine(DateTime.Now.ToString() + ":" + content);
                tempWriter.Close();
                tempWriter.Dispose();
            }
            catch (Exception)
            {
            }

        }

        public void CloseFile()
        {
            if (textReader != null)
                textReader.Close();
            fileLoaded = false;
        }

        public void ReadNode()
        {
            textReader.Read();
        }

        public bool IsNotEndElement()
        {
            return textReader.NodeType != XmlNodeType.EndElement;
        }

        // this function has two uses: 1) when we are loading xml for the first time (used in adapter); 2) when the RIITS server has not generated an xml file and as a result, we get a SOAP Exception because the 
        // document is not found. In such a case, the adapter should not generate any events.
        public bool XMLLoaded()
        {
            return fileLoaded;
        }

        private static int arterialReplyaCounter = -1;
        protected static string ArterialDataReplayer()
        {
            TextReader reader = new StreamReader(@"C:\SavedData\ArterialRealTimeLADOT\" + arterialReplyaCounter + ".xml");
            arterialReplyaCounter++;
            return reader.ReadToEnd();
        }

        private static int freewayReplayCounter = 36;//3050;// 2741;//1055;
        protected static string FreewayDataReplayer()
        {
            //string path= @"C:\SavedData\FreewayRealTime\" + freewayReplayCounter + ".xml";
            string path = @"C:\just for test\orig\" + freewayReplayCounter + ".xml";
            TextReader reader = new StreamReader(path);
            freewayReplayCounter++;
            return reader.ReadToEnd();


        }

        private static void FileWriter(string fileName, string content)
        {
            StreamWriter wr = new StreamWriter(fileName);
            wr.WriteLine(content);
            wr.Close();
            wr.Dispose();
        }

        private static void FileExpendedWriter(string fileName, string content)
        {
            StreamWriter wr = new StreamWriter(fileName, true);
            wr.WriteLine(content);
            wr.Close();
            wr.Dispose();
        }

        protected static string WSDLConnector(String requestType, String issueAgency, String verbosity)
        {
            String result = null;


            Console.Out.WriteLine(DateTime.Now + " : getting " + requestType + " " + verbosity + " from " +
                                  issueAgency);
            try
            {
                switch (requestType)
                {
                    case "congestionArterial":
                    case "congestionFreeway":
                        var vdsSer2 = new CongestionServiceService();
                        result = vdsSer2.getTrafficInfo(XmlUtil.getUser(), XmlUtil.getPass(),
                                               XmlUtil.getMsgReq(requestType, issueAgency, verbosity));
                        break;
                    case "bus":
                        var vdsSer1 = new BusServiceService();
                        result = vdsSer1.getBusInfo(XmlUtil.getUser(), XmlUtil.getPass(),
                                               XmlUtil.getMsgReq(requestType, issueAgency, verbosity));
                        break;
                    case "rail":
                        var vdsSer3 = new RailServiceService();
                        result = vdsSer3.getRailInfo(XmlUtil.getUser(), XmlUtil.getPass(),
                                               XmlUtil.getMsgReq(requestType, issueAgency, verbosity));
                        break;
                    case "rms":
                        var vdsSer4 = new RmsServiceService();
                        result = vdsSer4.getRmsInfo(XmlUtil.getUser(), XmlUtil.getPass(),
                                               XmlUtil.getMsgReq(requestType, issueAgency, verbosity));
                        break;
                    case "travelTimes":
                        var vdsSer5 = new TravelTimesServiceService();
                        result = vdsSer5.getTravelTimeInfo(XmlUtil.getUser(), XmlUtil.getPass(),
                                               XmlUtil.getMsgReq(requestType, issueAgency, verbosity));
                        break;
                    case "event":
                        var vdsSer6 = new EventServiceService();
                        result = vdsSer6.getEventInfo(XmlUtil.getUser(), XmlUtil.getPass(),
                                               XmlUtil.getMsgReq(requestType, issueAgency, verbosity));
                        /*
                        StreamReader rd = new StreamReader("event_Caltrans-D7.xml");
                        result = rd.ReadToEnd();
                        rd.Close();
                        */
                        break;
                    case "cms":
                        var vdsSer7 = new CmsServiceService();
                        result = vdsSer7.getCmsInfo(XmlUtil.getUser(), XmlUtil.getPass(),
                                               XmlUtil.getMsgReq(requestType, issueAgency, verbosity));
                        break;

                }

                FileWriter(requestType + "_" + issueAgency + ".xml", result);

                Console.Out.WriteLine(DateTime.Now + " : got " + requestType + " " + verbosity + " from " +
                                      issueAgency);


            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                //FileExpendedWriter("WSDL" + requestType.ToString() + "_" + verbosity.ToString() + ".txt", DateTime.Now.ToString() + " error:" + e.Message);

                //ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 307, "BaseFileParser.cs");
                //reporter.SendEmailThread();

                string datatype = ExceptionDatabaseReporter.DataTypeConverter(requestType);
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(e.Message, 307, "BaseFileParser.cs", datatype, "RIITS");
                reporter2.SendDatabaseExceptionThread();

                result = null;
            }

            return result;
        }
    }


}
