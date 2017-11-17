/**
 * Created by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Create Oracle Output Messages
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using MyMemory;
using OutputTypes;
using Utils;
using Parsers;
using System.IO;

namespace XMLOutputAdapter
{
    public class OracleOutputMessage<T> : IOutputMessage<T> where T : IOutputType
    {

        public SQLMessageConfig config;
        public LookupTable<T> lookupTable;
        public List<char> tags;

        public OracleOutputMessage(SQLMessageConfig configInfo, LookupTable<T> table)
        {
            config = configInfo;
            lookupTable = table;
            tags = CreateTags(config);
        }

        private List<char> CreateTags(SQLMessageConfig config)
        {
            List<char> result = new List<char>(config.OutputFieldOrders.Count);
            for (int i = 0; i < config.OutputFieldOrders.Count; i++)
            {
                result.Add(default(char));
                string type = config.OutputFieldOrders[i].GetType().ToString();
                if (type == typeof(string).ToString() || type == typeof(String).ToString())
                    result[i] = '\'';

            }
            return result;
        }


        public Object CreateMessage(List<T> buffer)
        {
            string agency = config.Agency;
            if (config.TableName == DBTableStrings.FreewayTableName)
            {
                //agency = RIITSFreewayAgency.Caltrans_D7;
                return createFreewayMessage(buffer, agency);
            }
            else if (config.TableName == DBTableStrings.ArterialDataTableName)
            {
                //agency = RIITSArterialAgency.LADOT;
                return createArterialMessage(buffer, agency);
            }
            else if (config.TableName == DBTableStrings.MetroBusTableName)
            {
                //agency = RIITSBusAgency.MTA_Metro;
                return createBusMessage(buffer, agency);
            }
            else if (config.TableName == DBTableStrings.MetroRailTableName)
            {
                //agency = RIITSBusAgency.MTA_Metro;
                return createRailMessage(buffer, agency);
            }
            else if (config.TableName == DBTableStrings.RampMeterTableName)
            {
                //agency = RIITSFreewayAgency.Caltrans_D7;
                return createRampMessage(buffer, agency);
            }
            else if (config.TableName == DBTableStrings.TravelTimeTableName)
            {
                //agency = RIITSFreewayAgency.Caltrans_D7;
                return createTravelMessage(buffer, agency);
            }
            else if (config.TableName == DBTableStrings.EventTableName)
            {
                // to be implemented
                return createEventMessage(buffer);
            }
            else if (config.TableName == DBTableStrings.CmsTableName)
            {
                // to be implemented
                //agency = RIITSFreewayAgency.Caltrans_D7;
                return createCmsMessage(buffer, agency);
            }
            else
                return "";

        }

        public void FileWriterAppend(string fileName, string Msg)
        {
            StreamWriter tempWriter = new StreamWriter(fileName, true);
            tempWriter.WriteLine(DateTime.Now.ToString() + Msg);
            tempWriter.Close();
        }

        public List<Object> createFreewayMessage(List<T> buffer, string agency)
        {
            string SqlStr = "";
            char tag = '\'';


            //DateTime mydate = DateTime.Now;
            List<Object> result = new List<Object>();

            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e,agency);  //get from LookUpFreewayDB

                if (values == null) // sensor information not found in our databases.
                    continue;


                SqlStr = "insert /*+ append */ into " + DBTableStrings.FreewayTableName + " values (" +
                            tag + values[0].ToString() + tag + "," +
                            tag + agency + tag + "," +
                            "to_date(" + tag + values[3].ToString() + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                            tag + values[1].ToString() + tag + "," +    //sensor id
                            tag + values[5].ToString() + tag + "," +    //occupancy                  
                            tag + values[2].ToString() + tag + "," +    //speed         
                            tag + values[6].ToString() + tag + "," +    //volume
                            tag + values[4].ToString() + tag + "," +    //HOV
                            tag + values[7].ToString() + tag + ")";    //link status

                // According the setting in LookUpFreewayDB.Getfields
                //(10, 'Caltrans', to_date('01/15/2000 14:24:32', 'mm/dd/yyyy HH24:MI:SS'), 717145,23, 100,19,103);";
                result.Add(SqlStr);
            }


            return result;
        }

        public List<Object> createArterialMessage(List<T> buffer, string agency)
        {
            string SqlStr = "";
            char tag = '\'';

            List<Object> result = new List<Object>();

            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e, agency);
                if (values == null) // sensor information not found in our databases.
                    continue;


                SqlStr = "insert /*+ append */ into " + DBTableStrings.ArterialDataTableName + " values (" +
                            tag + values[0].ToString() + tag + "," +
                            tag + agency + tag + "," +
                            "to_date(" + tag + values[3] + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                            tag + values[1].ToString() + tag + "," +    //sensor id
                            tag + values[5].ToString() + tag + "," +    //occupancy                  
                            tag + values[2].ToString() + tag + "," +    //speed         
                            tag + values[6].ToString() + tag + "," +    //volume
                            tag + values[4].ToString() + tag + "," +    //HOV
                            tag + values[7].ToString() + tag + ")";    //link status


                //(10, 'Caltrans', to_date('01/15/2000 14:24:32', 'mm/dd/yyyy HH24:MI:SS'), 717145,23, 100,19,103);";
                result.Add(SqlStr);

            }
            return result;
        }

        public List<Object> createBusMessage(List<T> buffer, string agency)
        {
            string SqlStr = "";
            char tag = '\'';
            char[] delimeter = { ',', '/', ' ', ':' };
            string[] date;
            string temp_date;
            int hour;

            List<Object> result = new List<Object>();

            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e,agency);
                if (values == null) // sensor information not found in our databases.
                    continue;

                SqlStr = "insert /*+ append */ into " + DBTableStrings.MetroBusTableName + " values (" +
                            tag + values[0].ToString() + tag + "," +
                            tag + agency + tag + "," +
                            "to_date(" + tag + values[1].ToString() + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "),";

                for (int i = 2; i < 8; i++)
                    SqlStr += tag + values[i].ToString() + tag + ",";

                int decimalPoints = 6;

                double latitude = Double.Parse((String)values[8]) / (Math.Pow(10, decimalPoints));
                double longitude = Double.Parse((String)values[9]) / (Math.Pow(10, decimalPoints));

                //lat,long
                SqlStr += "MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(" +
                          longitude.ToString() + "," + latitude.ToString() + ",null),null,null),";

                //parsing date
                date = values[10].ToString().Split(delimeter);
                temp_date = "20" + date[2] + date[0] + date[1];

                if (date[date.Length - 1] == "PM")
                {
                    hour = Convert.ToInt16(date[3]);
                    if (hour != 12)
                        hour += 12;
                    temp_date += " " + hour.ToString() + ":" + date[4] + ":00";
                }
                else
                {
                    temp_date += " " + date[3] + ":" + date[4] + ":00";
                }

                SqlStr += "to_date(" + tag + temp_date + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "),";

                //others
                for (int i = 11; i < 15; i++)
                    SqlStr += tag + values[i].ToString() + tag + ",";

                SqlStr += tag + values[15].ToString() + tag + ")";
                result.Add(SqlStr);

            }
            return result;
        }

        public List<Object> createRailMessage(List<T> buffer, string agency)
        {
            string SqlStr = "";
            char tag = '\'';
            char[] delimeter = { ',', '/', ' ', ':' };
            string[] date;
            string temp_date;
            int hour;

            List<Object> result = new List<Object>();
            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e,agency);
                if (values == null) // sensor information not found in our databases.
                    continue;

                SqlStr = "insert /*+ append */ into " + DBTableStrings.MetroRailTableName + " values (" +
                            tag + values[0].ToString() + tag + "," +
                            tag + agency + tag + "," +
                            "to_date(" + tag + values[1].ToString() + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "),";

                for (int i = 2; i < 9; i++)
                    SqlStr += tag + values[i].ToString() + tag + ",";

                int decimalPoints = 6;

                double latitude = Double.Parse((String)values[9]) / (Math.Pow(10, decimalPoints));
                double longitude = Double.Parse((String)values[10]) / (Math.Pow(10, decimalPoints));

                //lat,long
                SqlStr += "MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(" +
                          longitude.ToString() + "," + latitude.ToString() + ",null),null,null),";

                //parsing date
                date = values[1].ToString().Split(' ');
                temp_date = date[0];

                //rail message only have time data, no date
                date = values[11].ToString().Split(delimeter);

                if (date[date.Length - 1] == "PM")
                {
                    hour = Convert.ToInt16(date[0]);
                    if (hour != 12)
                        hour += 12;
                    temp_date += " " + hour.ToString() + ":" + date[1] + ":00";
                }
                else
                {
                    temp_date += " " + date[0] + ":" + date[1] + ":00";
                }

                SqlStr += "to_date(" + tag + temp_date + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "),";

                //others
                for (int i = 12; i < 15; i++)
                    SqlStr += tag + values[i].ToString() + tag + ",";

                SqlStr += tag + values[15].ToString() + tag + ")";
                result.Add(SqlStr);
            }
            return result;
        }

        public List<Object> createRampMessage(List<T> buffer, string agency)
        {
            string SqlStr = "";
            char tag = '\'';

            List<Object> result = new List<Object>();
            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e,agency);
                if (values == null) // sensor information not found in our databases.
                    continue;

                SqlStr = "insert /*+ append */ into " + DBTableStrings.RampMeterTableName + " values (" +
                            tag + values[0].ToString() + tag + "," +
                            tag + agency + tag + "," +
                            "to_date(" + tag + values[1].ToString() + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "),";

                for (int i = 2; i < 16; i++)
                    SqlStr += tag + values[i].ToString() + tag + ",";

                SqlStr += tag + values[16].ToString() + tag + ")";
                result.Add(SqlStr);
            }
            return result;
        }

        public List<Object> createTravelMessage(List<T> buffer, string agency)
        {
            string SqlStr = "";
            char tag = '\'';

            List<Object> result = new List<Object>();
            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e,agency);
                if (values == null) // sensor information not found in our databases.
                    continue;

                SqlStr = "insert /*+ append */ into " + DBTableStrings.TravelTimeTableName + " values (" +
                            tag + values[0].ToString() + tag + "," +
                            tag + agency + tag + "," +
                            "to_date(" + tag + values[1].ToString() + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "),";

                for (int i = 2; i < 4; i++)
                    SqlStr += tag + values[i].ToString() + tag + ",";

                SqlStr += tag + values[4].ToString() + tag + ")";
                result.Add(SqlStr);
            }
            return result;
        }

        public List<Object> createEventMessage(List<T> buffer)
        {
            string SqlStr = "";
            char tag = '\'';
//            Console.WriteLine(buffer);
            List<Object> result = new List<Object>();
            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e,"");
                if (values == null) // sensor information not found in our databases.
                    continue;

                if (ProcessSingleQuote(values[0].ToString()) == "13370" &&  //added by keivan  
                    ProcessSingleQuote(values[1].ToString()) == "POLB")     //added by keivan
                    continue;                                               //added by keivan

                SqlStr = "insert /*+ append */ into " + DBTableStrings.EventTableName + " values (" +
                          tag + ProcessSingleQuote(values[0].ToString()) + tag + "," +
                          tag + ProcessSingleQuote(values[1].ToString()) + tag + "," +
                          tag + ProcessSingleQuote(values[2].ToString()) + tag + "," +
                          tag + ProcessSingleQuote(values[3].ToString()) + tag + "," +
                          tag + ProcessSingleQuote(values[4].ToString()) + tag + "," +
                          "MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(" +
                          values[6].ToString() + "," + values[5].ToString() + //long, lat
                          ",null),null,null),";

                for (int i = 7; i < 47; i++)
                {
                    if (i == 21 || i == 22 || i == 44 || i == 45)
                    {
                        SqlStr += "to_date(" + tag + values[i].ToString() + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "),";
                    }
                    else
                    {
                        string content = values[i].ToString();

                        content = ProcessSingleQuote(content);

                        // trim event 
                        if (content.Length > 4000)
                           content = content.Substring(0, 4000);

                        SqlStr += tag + content + tag + ",";
                    }

                }
                SqlStr += "to_date(" + tag + values[47].ToString() + tag + "," +
                            tag + "yyyymmdd HH24:MI:SS" + tag + "))";

                result.Add(SqlStr);
            }

//            Console.WriteLine(SqlStr);

            return result;
        }


        public string ProcessSingleQuote(string target)
        {
            if (!target.Contains("\'"))
                return target;

            string processedString = "";
            string[] item = target.Split('\'');
            for (int i = 0; i < item.Length - 1; i++)
            {
                processedString += item[i] + "\'\'";
            }
            processedString += item[item.Length - 1];
            return processedString;
        }

        public List<Object> createCmsMessage(List<T> buffer, String agency)
        {
            string SqlStr = "";
            char tag = '\'';


            //DateTime mydate = DateTime.Now;
            List<Object> result = new List<Object>();

            foreach (T e in buffer)
            {
                var values = (List<object>)lookupTable.GetRecord(e,agency);  //get from LookUpFreewayDB

                if (values == null) // sensor information not found in our databases.
                    continue;

                SqlStr = "insert /*+ append */ into " + DBTableStrings.CmsTableName + " values (" +
                         values[0].ToString() + "," +
                         tag + values[1].ToString() + tag + "," +
                         tag + values[2].ToString() + tag + "," +
                         "to_date(" + tag + values[3].ToString() + " " + values[4].ToString().Substring(0,6) + tag +
                            "," + "'yyyymmdd HH24MISS'" + ")" + "," +
                          tag + values[5].ToString() + tag + "," +
                          tag + values[6].ToString() + tag + "," +
                          tag + values[7].ToString() + tag + "," +
                          tag + values[8].ToString() + tag + "," +
                          tag + values[9].ToString() + tag + "," +
                          tag + values[10].ToString() + tag + "," +
                          tag + values[11].ToString() + tag + "," +
                          tag + agency + tag + "," +
                          "to_date(" + tag + values[12].ToString() + tag + "," + "'yyyymmdd HH24MISS'" + ")" +
                           ")"; 

                result.Add(SqlStr);
            }


            return result;
        }


    }
}
