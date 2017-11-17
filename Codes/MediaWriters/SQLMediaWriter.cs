/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Merge all SQLDBServices into this file
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

using System;
//using MyMemory;
using System.Data.SqlClient;
using Parsers;
using System.Collections;
using System.Collections.Generic;
using ExceptionReporter;

namespace MediaWriters
{
    public class SQLMediaWriter : MediaWriter
    {
        SqlConnection connection = null;
        String connString = "Server=tcp:fwv9kgwloo.database.windows.net;Database=jalal;User ID=Jalal@fwv9kgwloo;Password=InfoLab%#!;Trusted_Connection=False;Encrypt=True;";
        public SQLMediaWriter()
        {
            connection = new SqlConnection(connString);

        }

        public bool Write(Object message)
        {
            return RunCommand(connection, (string)message);
        }

        public int ReadMaxConfigID(string tableName)
        {
            int result = -1;

            string query = "select max( configid) from " + tableName + ")";

            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                result = (int)command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception e)
            {
                if (connection != null)
                    connection.Close();

                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 53, "SQLMediaWriter.cs");
                reporter.SendEmailThread();
            }
            return result;
        }

        public bool RunCommand(string comm)
        {
            return RunCommand(connection, comm);

        }

        public bool RunCommand(SqlConnection tempConnection, string comm)
        {

            String msg = comm;
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand(msg, tempConnection);
                int result = command.ExecuteNonQuery();
                tempConnection.Close();
                return true;
            }
            catch (Exception e)
            {
                if (tempConnection != null)
                    tempConnection.Close();

                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 79, "SQLMediaWriter.cs");
                reporter.SendEmailThread();

                return false;
            }
        }

        public void UpdateDatabase(string configTableName, String link_type, List<object> fieldOrders, DateTime date, int configID, Dictionary<int, Sensor> difList)
        {
            string message = null;
            char tag = '\'';
            string UNION_ALL = " UNION ALL ";
            //todo: avval ye version ro tooye db benvis.
            message = "insert into sys." + configTableName + "(";
            foreach (string field in fieldOrders)
            {
                message += field + ",";
            }
            message = message.Remove(message.Length - 1, 1); // last ','
            message += ")";
            //"( configID, agency, readDate, linkID, linkType, onstreet, fromstreet, tostreet, startLatLong, direction, postmile ) ";)

            foreach (KeyValuePair<int, Sensor> keyValuePair in difList)
            {
                Sensor tempSensor = keyValuePair.Value;
                //todo: should correct the agency and read date, and type

                message += "SELECT " +
                           tag + configID + tag + "," +
                           tag + RIITSFreewayAgency.Caltrans_D7 + tag + "," +
                           tag + date.ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                           tag + keyValuePair.Value.ID + tag + "," +
                           tag + link_type + tag + "," +
                           tag + tempSensor.OnStreet + tag + "," +
                           tag + tempSensor.FromStreet + tag + "," +
                           tag + tempSensor.ToStreet + tag + "," +
                           tag + tempSensor.GeogLocation + tag + "," +
                           tag + tempSensor.Direction + tag + "," +
                           tag + tempSensor.PostMile + tag;
                message += UNION_ALL;
            }
            message = message.Remove(message.Length - UNION_ALL.Length, UNION_ALL.Length);

            // use sql output message to write to DB.
            // do everything you did for freeway, for arterial.
            RunCommand(message);
        }

    }
}
