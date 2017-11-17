/**
 * Create by Bei(Penny) Pan (beipan@usc.edu) 
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
using ExceptionReporter;


namespace ExceptionReporter
{
    public class ExceptionOracleWriter 
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

        public ExceptionOracleWriter(string userName, string password)
        {
            con = new OracleConnection();
            user = userName;
            pwd = password;
            //OpenConnection();
        }

        ~ExceptionOracleWriter()
        {
            //CloseConnection();
        }

        public void OpenConnection()
        {
            con.ConnectionString = "User Id=" + user + ";Password=" + pwd + ";Data Source=ADMS;";
            // Open the connection
            con.Open();
        }

        public void CloseConnection()
        {
            con.Close();
            con.Dispose();
        }


        public bool Write(Object message)
        {
            char[] delimiter = { ';' };
            //separate the message by ';'

            List<Object> list = (List<Object>)message;
            bool result = true;
            int i = 0;

            try
            {
                OpenConnection();
                IDbTransaction trans = con.BeginTransaction(); // Turn off AUTOCOMMIT

                for (; i < list.Count; i++)
                {
                    string commends = (string)list[i];
                    OracleCommand cmd = (OracleCommand)con.CreateCommand();
                    cmd.CommandText = commends;
                    cmd.ExecuteNonQuery();
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

                    string msg = e.Message + "caused when " + (string)list[0] + "\n executed \n.";

                    FileWriterAppend("OracleConnection_ExceptionWrite.txt", msg);

                }
                catch (Exception)
                {
                    
                }
                
                return false;
            }
        }

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

    }
}
