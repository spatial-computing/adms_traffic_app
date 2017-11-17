/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Merge all OracleDBServices into this file
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CustomOracleGeoType;
using Microsoft.SqlServer.Types;
using Parsers;
using Utils;
using ExceptionReporter;
using System.Threading;

namespace MediaWriters
{
    public class OracleMediaWriter : MediaWriter
    {
        public StreamReader SR;
        // Create the connection object
        public OracleConnection con;
        public string user;
        public string pwd;
        public string QueryResult;
        public string TableName;


        //public OracleMediaWriter()
        //{
        //    con = new OracleConnection();   // for transit data
        //    //user = "TRANSIT";
        //    //pwd = "tr323";
        //}

        public OracleMediaWriter(string userName, string password)
        {
            con = new OracleConnection();
            user = userName;
            pwd = password;
            //OpenConnection();
        }

        ~OracleMediaWriter()
        {
            //CloseConnection();
        }

        public void OpenConnection()
        {
            con.ConnectionString = "User Id=" + user + ";Password=" + pwd + ";Data Source=ADMS;";
            //con.ConnectionString = "User Id=" + user + ";Password=" + pwd + ";Data Source=geodbs;";
            // Open the connection
            con.Open();
        }

        public void CloseConnection()
        {
            con.Close();
            //con.Dispose();
        }


        //insert /*+ append */ into congestion_config values
        //( 10,'Caltrans', to_date('01/15/2000 14:24:32', 'mm/dd/yyyy HH24:MI:SS'), 717145, '717145', 
        //'I-10','dfsgf',null, MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(34.069036,-117.971134,null),null,null), '3',2.5
        //);

        //insert /*+ append */ into congestion_data values
        //(10, 'Caltrans', to_date('01/15/2000 14:24:32', 'mm/dd/yyyy HH24:MI:SS'), 717145,23, 100,19,103);


        public void FileWriterAppend(string fileName, string Msg)
        {
            try
            {
                StreamWriter tempWriter = new StreamWriter(fileName, true);
                tempWriter.WriteLine(DateTime.Now.ToString() + Msg);
                tempWriter.Close();
            }
            catch
            {
            }
        }

        public bool Write(Object message)
        {
            char[] delimiter = { ';' };
            //separate the message by ';'

            List<Object> list = (List<Object>)message;
            bool result = true;
            int i=0;
            int duplicates = 0;
            try
            {
                OpenConnection();
                IDbTransaction trans = con.BeginTransaction(); // Turn off AUTOCOMMIT

                for (; i < list.Count; i++)
                {

                    try
                    {
                        string commends = (string)list[i];
                        commends = commends.Replace('&', ' ');
                        OracleCommand cmd = (OracleCommand)con.CreateCommand();
                        cmd.CommandText = commends;
                        cmd.ExecuteNonQuery();
                    }
                    catch(Exception e)
                    {

                        try
                        {
                            string datatype = ExceptionDatabaseReporter.DataTypeConverter((string)list[i]);
                            string msg = e.Message + " happen when single sentence " + (string)list[i] + " executed";

                            //FileWriterAppend("OracleConnection_write_single.txt", msg);

                            ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(msg, 127, "OracleMediaWriter.cs",
                                                                                                datatype, "Oracle");
                            reporter2.SendDatabaseExceptionThread();

                            if (!e.Message.Contains("ORA-00001"))        //connection not there
                            {
                                //msg = e.Message + " starting when single sentence No. " + i.ToString() + " for datatype: " + datatype + " executed";
                                ExceptionEmailReporter reporter = new ExceptionEmailReporter(msg, 127, "OracleMediaWriter.cs");
                                reporter.SendEmailThread();
                                //break;
                            }
                            
                        }
                        catch(Exception ex)
                        {
                        }

                    }
                    //    cmd.Dispose();

                }

                trans.Commit(); // AutoCommit is 'ON'
                trans.Dispose();
                //con.Dispose();
                CloseConnection();
                
                return true;
            }
            catch (Exception e)
            {
                try
                {
                    if (con != null)
                       con.Close();


                
                    string datatype = ExceptionDatabaseReporter.DataTypeConverter((string) list[0]);
                    string msg = e.Message + " caused when " + list.Count.ToString() + " rows " + datatype +
                                 " inserted ";
                    FileWriterAppend("OracleConnection_write.txt", msg);

                    ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(msg, 127, "OracleMediaWriter.cs",
                                                                                        datatype, "Oracle");
                    reporter2.SendDatabaseExceptionThread();

                    ExceptionEmailReporter reporter = new ExceptionEmailReporter(msg, 127, "OracleMediaWriter.cs");
                    reporter.SendEmailThread();

                }
                catch (Exception ex)
                {
                }

               


                return false;
            }
        }

        public int ReadMaxConfigID(string tableName)
        {
            int result = -1;

            string query = "select max(config_id) as m_id from " + tableName;

            try
            {
                OpenConnection();
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;


                // Execute command, create OracleDataReader object
                OracleDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return -1;

                result = Convert.ToInt16(reader["m_id"]);
                CloseConnection();
            }
            catch (Exception e)
            {
                if (con != null)
                    con.Close();
                Console.WriteLine(e.Message);
                FileWriterAppend("OracleConnection_readMaxConfig.txt", e.Message);

                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 164, "OracleMediaWriter.cs");
                reporter.SendEmailThread();
                result = -1;
            }
            return result;
        }

        public string ReadConfig(string tableName, int config_id)
        {
            string result = "";
            string tag = "?";
            string query = "";

            //fields are separated by ",", records are separated by ";"
            if (tableName==DBTableStrings.ArterialConfigTableName)
                query = "select CONFIG_ID,AGENCY,CITY,DATE_AND_TIME,LINK_ID,LINK_TYPE,ONSTREET,FROMSTREET,TOSTREET,START_LAT_LONG,DIRECTION,POSTMILE,AFFECTED_NUMBEROF_LANES from " + tableName + " where config_id = " + config_id.ToString();
            else
                query = "select * from " + tableName + " where config_id = " + config_id.ToString();
            Console.WriteLine("Query===++++++++++++++++++++++++++++++++++++++++++++===>" + query);
            try
            {
                OpenConnection();
                OracleCommand cmd = new OracleCommand(query);
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;


                // Execute command, create OracleDataReader object
                OracleDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {

                        //if the type is SdoGeometry, store lat/long separately
                        if (reader.GetFieldType(i).ToString() == "CustomOracleGeoType.SdoGeometry")
                        {
                            SdoGeometry info = (SdoGeometry)reader[i];

                            result += info.Point.Y.ToString() + tag;    //lat
                            result += info.Point.X.ToString() + tag;    //long
                        }
                        //Console.WriteLine(reader[i].GetType());
                        //Console.WriteLine(reader[i].ToString());
                        else
                        {
                            result += Convert.ToString(reader[i]);
                            result += tag;
                        }

                    }
                    //result.TrimEnd('?');
                    result += ";";
                }


                CloseConnection();
                return result;
            }
            catch (Exception e)
            {
                if (con != null)
                    con.Close();
                Console.WriteLine(e.Message);
                FileWriterAppend("OracleConnection_readConfig.txt", e.Message);

                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 226, "OracleMediaWriter.cs");
                reporter.SendEmailThread();

                return "";
            }
        }


        public static Dictionary<string, Sensor> OracleParsing2SensorList(string result)
        {
            Dictionary<string, Sensor> temp_list = new Dictionary<string, Sensor>();

            double lat, lon;
            if(result.Contains("50349")	&& result.Contains("389686"))
                Console.WriteLine("result=======>  has 50349 and 389686"); 
            string[] items = result.Split(';');
            List<string> record = new List<string>(11);
            for (int i = 0; i < items.Length - 1; i++)
            {
                if(items[i].Contains("50349"))
                    Console.WriteLine("50349 record =======> "+items[i]); 
                string[] content = items[i].Split('?');
                record.Add(content[4]);     //ID
                record.Add(content[6]);
                record.Add(content[7]);
                record.Add(content[8]);

                lat = Double.Parse(content[9]);
                lat = lat * (Math.Pow(10, 6));
                record.Add(lat.ToString());        //lat

                lon = Double.Parse(content[10]);
                lon = lon * (Math.Pow(10, 6));
                record.Add(lon.ToString());        //lon

                record.Add(content[11]);         //dir
                record.Add(content[2]);         //city
                record.Add(content[12]);        //post mile
                record.Add(content[13]);        //count
                record.Add(content[5]);         //Link Type

                Sensor temp = new Sensor(content[1], record);

                if (!temp_list.ContainsKey(temp.ID))
                    temp_list.Add(temp.ID, temp);
                //temp_list.Add(temp.ID, temp);
                record.Clear();
            }
            return temp_list;
        }

        public static Dictionary<int, BusRoute> OracleParsing2BusList(string result)
        {
            Dictionary<int, BusRoute> temp_list = new Dictionary<int, BusRoute>();
            string[] items = result.Split(';');
            List<string> record = new List<string>(11);
            for (int i = 0; i < items.Length - 1; i++)
            {
                string[] content = items[i].Split('?');
                record.Add(content[3]);
                record.Add(content[4]);
                record.Add(content[5]);
                BusRoute temp = new BusRoute(content[1], record);
                if( !temp_list.ContainsKey(temp.routeId))
                    temp_list.Add(temp.routeId, temp);
                record.Clear();
            }
            return temp_list;
        }

        public static Dictionary<int, RailRoute> OracleParsing2RailList(string result)
        {
            Dictionary<int, RailRoute> temp_list = new Dictionary<int, RailRoute>();
            string[] items = result.Split(';');
            List<string> record = new List<string>(11);
            for (int i = 0; i < items.Length - 1; i++)
            {
                string[] content = items[i].Split('?');
                record.Add(content[3]);
                record.Add(content[4]);
                RailRoute temp = new RailRoute(content[1], record);
                //temp_list.Add(temp.routeId, temp);
                if (!temp_list.ContainsKey(temp.routeId))
                    temp_list.Add(temp.routeId, temp);
                record.Clear();
            }
            return temp_list;
        }

        public static Dictionary<int, RampMeter> OracleParsing2RampList(string result)
        {
            Dictionary<int, RampMeter> temp_list = new Dictionary<int, RampMeter>();
            string[] items = result.Split(';');
            double temp_loc;
            List<string> record = new List<string>(11);
            for (int i = 0; i < items.Length - 1; i++)
            {
                string[] content = items[i].Split('?');

                for (int j = 3; j < content.Length - 1; j++)
                {
                    if (j != 9 && j != 10)
                    {
                        record.Add(content[j]);
                    }
                    else
                    {
                        temp_loc = Double.Parse(content[j]);
                        temp_loc = temp_loc * (Math.Pow(10, 6));
                        record.Add(temp_loc.ToString());        //lat,long
                    }

                }

                RampMeter temp = new RampMeter(content[1], record);
                if (!temp_list.ContainsKey(temp.ID))
                    temp_list.Add(temp.ID, temp);
                //temp_list.Add(temp.ID, temp);
                record.Clear();
            }
            return temp_list;
        }

        public static Dictionary<int, travelLinks> OracleParsing2TravelList(string result)
        {
            Dictionary<int, travelLinks> temp_list = new Dictionary<int, travelLinks>();
            string[] items = result.Split(';');
            double temp_loc;
            List<string> record = new List<string>(11);
            for (int i = 0; i < items.Length - 1; i++)
            {
                string[] content = items[i].Split('?');

                for (int j = 3; j < content.Length - 1; j++)
                {
                    if (j != 11 && j != 12 && j != 14 && j != 15)       //begin, end
                    {
                        record.Add(content[j]);
                    }
                    else
                    {
                        temp_loc = Double.Parse(content[j]);
                        temp_loc = temp_loc * (Math.Pow(10, 6));
                        record.Add(temp_loc.ToString());        //lat,long
                    }

                }

                travelLinks temp = new travelLinks(content[1], record);
                if (!temp_list.ContainsKey(temp.ID))
                    temp_list.Add(temp.ID, temp);
                //temp_list.Add(temp.ID, temp);
                record.Clear();
            }
            return temp_list;
        }
        public static Dictionary<int, CmsDevice> OracleParsing2CmsDevice(string result)
        {
            Dictionary<int, CmsDevice> temp_list = new Dictionary<int, CmsDevice>();

            double lat, lon;
            string[] items = result.Split(';');
            List<string> record = new List<string>(11);
            for (int i = 0; i < items.Length - 1; i++)
            {
                string[] content = items[i].Split('?');
                record.Add(content[3]);     //ID
                record.Add(content[4]);
                record.Add(content[5]);
                record.Add(content[6]);

                lat = Double.Parse(content[7]);
                lat = lat * (Math.Pow(10, 6));
                record.Add(lat.ToString());        //lat

                lon = Double.Parse(content[8]);
                lon = lon * (Math.Pow(10, 6));
                record.Add(lon.ToString());        //lon

                record.Add(content[9]);         //dir
                record.Add(content[10]);         //city
                record.Add(content[11]);        //post mile
                

                CmsDevice temp = new CmsDevice(content[1], record);

                if (!temp_list.ContainsKey(temp.ID))
                    temp_list.Add(temp.ID, temp);
                //temp_list.Add(temp.ID, temp);
                record.Clear();
            }
            return temp_list;
        }

        public void UpdateCongestionDatabase(string configTableName, int configID, Dictionary<string, Sensor> difList)
        {
            string temp_message = null;

            char tag = '\'';
            //OracleMediaWriter writer = new OracleMediaWriter(user, pwd);

            List<Object> result = new List<object>();

            foreach (KeyValuePair<string, Sensor> keyValuePair in difList)
            {
                Sensor tempSensor = keyValuePair.Value;

                tempSensor.OnStreet = ProcessSingleQuote(tempSensor.OnStreet);
                tempSensor.FromStreet = ProcessSingleQuote(tempSensor.FromStreet);
                tempSensor.ToStreet = ProcessSingleQuote(tempSensor.ToStreet);

                // following line corrected by keivan
                //temp_message = "insert /*+ append */ into " + configTableName + " values(";
                temp_message = "insert /*+ append */ into " + configTableName + 
                "(CONFIG_ID,AGENCY,CITY,DATE_AND_TIME,LINK_ID,LINK_TYPE,ONSTREET,FROMSTREET,TOSTREET,START_LAT_LONG,DIRECTION,POSTMILE,AFFECTED_NUMBEROF_LANES) values(";

                temp_message += tag + configID.ToString() + tag + "," +
                                tag + keyValuePair.Value.Agency + tag + "," +
                                tag + tempSensor.City + tag + "," +
                                "to_date(" + tag + DateTime.Now.ToLocalTime().ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                                tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                                tag + keyValuePair.Value.ID.ToString() + tag + "," +
                                tag + tempSensor.AffectedLanes[0].Type + tag + "," +
                                tag + tempSensor.OnStreet + tag + "," +
                                tag + tempSensor.FromStreet + tag + "," +
                                tag + tempSensor.ToStreet + tag + "," +
                                "MDSYS.SDO_GEOMETRY(2001,8307,MDSYS.SDO_POINT_TYPE(" +
                                tempSensor.GeogLocation.Long.ToString() + "," + tempSensor.GeogLocation.Lat.ToString() +
                                ",null),null,null)," +
                                tag + tempSensor.Direction.ToString() + tag + "," +
                                tag + tempSensor.PostMile.ToString() + tag + "," +
                                tag + tempSensor.AffectedLanes[0].LaneCnt.ToString() + tag + ")";

                //temp_message += ";";
                result.Add(temp_message);
            }
            while (!Write(result))
            {
                Thread.Sleep(1000 * 60);
            };
            //writer.Write(temp_message);
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

        public void UpdateTransitDatabase(string configTableName,  int configID, Dictionary<int, BusRoute> difList)
        {
            string temp_message = null;

            char tag = '\'';
            //OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            List<object> result = new List<object>();

            foreach (KeyValuePair<int, BusRoute> keyValuePair in difList)
            {
                BusRoute tempSensor = keyValuePair.Value;

                temp_message = "insert /*+ append */ into " + configTableName + " values(";

                temp_message += tag + configID.ToString() + tag + "," +
                                tag + keyValuePair.Value.Agency + tag + "," +
                                "to_date(" + tag + DateTime.Now.ToLocalTime().ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                                tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                                tag + keyValuePair.Value.routeId.ToString() + tag + "," +
                                tag + tempSensor.routeDes + tag + "," + tag;

                for (int i = 0; i < tempSensor.zones.Length - 1; i++)
                {
                    temp_message += tempSensor.zones[i].ToString() + ",";
                }

                temp_message += tempSensor.zones[tempSensor.zones.Length - 1].ToString() + tag + ")";

                //temp_message += ";";

                result.Add(temp_message);
                //break;
            }
            while (!Write(result))
            {
                Thread.Sleep(1000 * 60);
            };
        }

        public void UpdateTransitDatabase(string configTableName, int configID, Dictionary<int, RailRoute> difList)
        {
            string temp_message = null;

            char tag = '\'';
            //OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            List<object> result = new List<object>();

            foreach (KeyValuePair<int, RailRoute> keyValuePair in difList)
            {
                RailRoute tempSensor = keyValuePair.Value;

                temp_message = "insert /*+ append */ into " + configTableName + " values(";

                temp_message += tag + configID.ToString() + tag + "," +
                                tag + keyValuePair.Value.Agency
                                + tag + "," +
                                "to_date(" + tag + DateTime.Now.ToLocalTime().ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                                tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                                tag + keyValuePair.Value.routeId.ToString() + tag + "," +
                                tag + tempSensor.routeDes + tag + ")";
                //temp_message += ";";
                result.Add(temp_message);
                //break;
            }
            while (!Write(result))
            {
                Thread.Sleep(1000 * 60);
            };
        }

        public void UpdateTransitDatabase(string configTableName, int configID, Dictionary<int, RampMeter> difList)
        {
            string temp_message = null;

            char tag = '\'';
            //OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            List<Object> result = new List<object>();

            foreach (KeyValuePair<int, RampMeter> keyValuePair in difList)
            {
                RampMeter tempSensor = keyValuePair.Value;

                temp_message = "insert /*+ append */ into " + configTableName + " values(";

                temp_message += tag + configID.ToString() + tag + "," +
                                tag + keyValuePair.Value.Agency + tag + "," +
                                "to_date(" + tag + DateTime.Now.ToLocalTime().ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                                tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                                tag + keyValuePair.Value.ID.ToString() + tag + "," +
                                tag + keyValuePair.Value.msID.ToString() + tag + "," +
                                tag + keyValuePair.Value.rampType.ToString() + tag + "," +
                                tag + keyValuePair.Value.OnStreet + tag + "," +
                                tag + keyValuePair.Value.FromStreet + tag + "," +
                                tag + keyValuePair.Value.ToStreet + tag + "," +
                                "MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(" +
                                tempSensor.longitude.ToString() + "," + tempSensor.latitude.ToString() +
                                ",null),null,null)," +
                                tag + keyValuePair.Value.Direction + tag + "," +
                                tag + keyValuePair.Value.City + tag + "," +
                                tag + keyValuePair.Value.PostMile.ToString() + tag + ")";
                //temp_message += ";";
                result.Add(temp_message);
                //break;
            }
            while (!Write(result))
            {
                Thread.Sleep(1000 * 60);
            };
        }

        public void UpdateTransitDatabase(string configTableName,  int configID, Dictionary<int, travelLinks> difList)
        {
            string temp_message = null;

            char tag = '\'';
            //OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            List<Object> result = new List<object>();

            foreach (KeyValuePair<int, travelLinks> keyValuePair in difList)
            {
                travelLinks tempSensor = keyValuePair.Value;

                temp_message = "insert /*+ append */ into " + configTableName + " values(";

                temp_message += tag + configID.ToString() + tag + "," +
                                tag + keyValuePair.Value.Agency + tag + "," +
                                "to_date(" + tag + DateTime.Now.ToLocalTime().ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                                tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                                tag + keyValuePair.Value.ID.ToString() + tag + "," +
                                tag + keyValuePair.Value.route.ToString() + tag + "," +
                                tag + keyValuePair.Value.Direction + tag + "," +
                                tag + keyValuePair.Value.linkType + tag + "," +
                                tag + keyValuePair.Value.beginID.ToString() + tag + "," +
                                tag + keyValuePair.Value.endID.ToString() + tag + "," +
                                tag + keyValuePair.Value.length.ToString() + tag + "," +
                                tag + keyValuePair.Value.beginStreet.ToString() + tag + "," +
                                "MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(" +
                                tempSensor.beginLon.ToString() + "," + tempSensor.beginLat.ToString() +
                                ",null),null,null)," +
                                tag + keyValuePair.Value.endStreet.ToString() + tag + "," +
                                "MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(" +
                                tempSensor.endLon.ToString() + "," + tempSensor.endLat.ToString() +
                                ",null),null,null))";

                //temp_message += ";";
                result.Add(temp_message);
                //break;
            }
            while (!Write(result))
            {
                Thread.Sleep(1000 * 60);
            };
        }

        public void UpdateEventDatabase(string configTableName, int configID, Dictionary<int, CmsDevice> difList)
        {
            string temp_message = null;

            char tag = '\'';
            //OracleMediaWriter writer = new OracleMediaWriter(user, pwd);

            List<Object> result = new List<object>();

            foreach (KeyValuePair<int, CmsDevice> keyValuePair in difList)
            {
                CmsDevice tempSensor = keyValuePair.Value;

                temp_message = "insert /*+ append */ into " + configTableName + " values(";

                temp_message += tag + configID.ToString() + tag + "," +
                                tag + keyValuePair.Value.Agency + tag + "," +
                                "to_date(" + tag + DateTime.Now.ToLocalTime().ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                                tag + "yyyymmdd HH24:MI:SS" + tag + ")," +
                                tag + keyValuePair.Value.ID.ToString() + tag + "," +
                                tag + tempSensor.OnStreet + tag + "," +
                                tag + tempSensor.FromStreet + tag + "," +
                                tag + tempSensor.ToStreet + tag + "," +
                                "MDSYS.SDO_GEOMETRY(2001,null,MDSYS.SDO_POINT_TYPE(" +
                                tempSensor.GeogLocation.Long.ToString() + "," + tempSensor.GeogLocation.Lat.ToString() +
                                ",null),null,null)," +
                                tag + tempSensor.Direction.ToString() + tag + "," +
                                tag + tempSensor.City + tag + "," +
                                tag + tempSensor.PostMile.ToString() +  tag + ")";

                //temp_message += ";";
                result.Add(temp_message);
            }
            while (!Write(result))
            {
                Thread.Sleep(1000 * 60);
            };
            //writer.Write(temp_message);
        }

        
    }
}
