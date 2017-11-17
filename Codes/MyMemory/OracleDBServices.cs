/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using Parsers;
using ExceptionReporter;

namespace MyMemory
{
    public class OracleDBServices
    {
        //static String connString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=adms.usc.edu)));User Id=jalal;Password=InfoLab;";
        static String connString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=geodb.usc.edu)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=geodbs)));User Id=METRO;Password=metro323;";
        
        static OracleConnection connection = new OracleConnection(connString);
        
        public static int ReadMaxConfigID(string tableName)
        {
            int result = -1;

            string query = "select max( config_id) from " + tableName;

            try
            {
                connection.Open();

                OracleCommand command = new OracleCommand(query, connection);
                OracleNumber jj = (OracleNumber) command.ExecuteOracleScalar();
                if (jj.IsNull)
                    // no data in table. set the result to -1.
                    result = -1;
                else
                {
                    result = (int) jj.Value;

                }
            
                connection.Close();
            }
            catch (Exception e)
            {
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 46, "OracleDBService.cs");
                reporter.SendEmailThread();
                if (connection != null)
                    connection.Close();
            }
            return result;
        }

        public static bool RunCommand(string comm)
        {
            return RunCommand(connection, comm);

        }

        public static bool RunCommand(OracleConnection tempConnection, string comm)
        {

            String msg = comm;
            try
            {
                connection.Open();

                OracleCommand command = new OracleCommand(msg, tempConnection);
                int result = command.ExecuteNonQuery();
                tempConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 75, "OracleDBService.cs");
                reporter.SendEmailThread();
                if (tempConnection!= null)
                    tempConnection.Close();
                return false;
            }
        }

        public static void UpdateDatabase(string configTableName, String link_type, List<object> fieldOrders, DateTime date, int configID, Dictionary<int, Sensor> difList)
        {
            string message = null;
            char tag = '\'';
            string tableSchema = "";
            
            foreach (string field in fieldOrders)
            {
                tableSchema+= field + ",";
            }
            tableSchema= tableSchema.Remove(tableSchema.Length - 1, 1); // last ','
            
            
            //"( configID, agency, readDate, linkID, linkType, onstreet, fromstreet, tostreet, startLatLong, direction, postmile ) ";)
            
            message += "insert all ";
            foreach (KeyValuePair<int, Sensor> keyValuePair in difList)
            {
                
                message += "into " + configTableName + "(" + tableSchema + ") VALUES ";
                Sensor tempSensor = keyValuePair.Value;
                

                message += "(" +  configID +  "," +
                           tag + RIITSFreewayAgency.Caltrans_D7 + tag + "," +
                           "to_date(" + tag + date.ToString("yyyyMMdd HHmmss") + tag + ", 'YYYYMMDD HH24MISS')" + "," +
                           tag + keyValuePair.Value.ID + tag + "," +
                           tag + link_type + tag + "," +
                           tag + tempSensor.OnStreet.Replace("\'","\'\'") + tag + "," +
                           tag + tempSensor.FromStreet.Replace("\'", "\'\'") + tag + "," +
                           tag + tempSensor.ToStreet.Replace("\'", "\'\'") + tag + "," +
                           "SDO_GEOMETRY(2001,NULL,SDO_POINT_TYPE(" +
                           tempSensor.GeogLocation.Lat + "," +
                           tempSensor.GeogLocation.Lat + ",NULL),NULL,NULL)"+ "," +
                           tag + tempSensor.Direction + tag + "," +
                            tempSensor.PostMile + ") ";
                
            }
            message += "SELECT * FROM dual";
            //message = message.Remove(message.Length - 1, 1);
            

            // use sql output message to write to DB.
            // do everything you did for freeway, for arterial.
            RunCommand(message);
        }


    }
}
