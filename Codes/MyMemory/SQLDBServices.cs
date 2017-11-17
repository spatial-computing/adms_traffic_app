﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Parsers;
using ExceptionReporter;

namespace MyMemory
{
    public class SQLDBServices
    {
        //private static SQLDBServices singleton;
        
        //public SQLDBServices GetInstance()
        //{
        //    if (singleton== null)
        //    {
        //        singleton = new SQLDBServices();
        //    }

        //    return singleton;

        //}

        static String connString = "Server=tcp:fwv9kgwloo.database.windows.net;Database=jalal;User ID=Jalal@fwv9kgwloo;Password=InfoLab%#!;Trusted_Connection=False;Encrypt=True;";
        
        static SqlConnection connection = new SqlConnection(connString);

        public static int ReadMaxConfigID(string tableName)
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
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 49, "SQLDBService.cs");
                reporter.SendEmailThread();
            }
            return result;
        }

        public static bool RunCommand(string comm)
        {
            return RunCommand(connection, comm);

        }

        public static bool RunCommand(SqlConnection tempConnection, string comm)
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
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 78, "SQLDBService.cs");
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
