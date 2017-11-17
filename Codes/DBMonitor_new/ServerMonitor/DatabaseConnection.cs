
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using System.Convert;
using System.IO;
using System.Data;
using Oracle.DataAccess.Client;


namespace ServerMonitor
{
    class DatabaseConnection
    {
        public StreamReader SR;
        // Create the connection object
        public OracleConnection con;
        public string QueryResult;
        public string TableName;

        public DatabaseConnection()
        {
            con = new OracleConnection();
        }
        public void OpenConnection()
        {
            con.ConnectionString = "User Id=sppd;Password=sr323;Data Source=adms;";
            // Open the connection
            con.Open();
            //Console.WriteLine("Connection to Oracle database established successfully !");
            //Console.WriteLine(" ");
        }

        public void CloseConnection()
        {
            con.Close();
            con.Dispose();
        }

        public ArrayList Query(string cmdQuery, string columnName)
        {

            string image_Path;
            ArrayList image_Set = new ArrayList();

            // Create the OracleCommand object
            OracleCommand cmd = new OracleCommand(cmdQuery);
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;

            try
            {
                // Execute command, create OracleDataReader object
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //s++;
                    //ArrayList temp;
                    image_Path = Convert.ToString(reader[columnName]);
                    image_Set.Add(image_Path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Dispose OracleCommand object
            return image_Set;
            // Close and Dispose OracleConnection object
        }
        public bool Write(String message)
        {

            int i = 0;

            try
            {
                string commends = message;
                OracleCommand cmd = (OracleCommand)con.CreateCommand();
                cmd.CommandText = commends;
                cmd.ExecuteNonQuery();
                //    cmd.Dispose();


                return true;
            }
            catch (Exception e)
            {
                try
                {
                    if (con != null)
                        con.Close();
                }
                catch (Exception)
                {

                }

                return false;
            }
        }

    }
}
