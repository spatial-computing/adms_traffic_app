using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using System.Threading;
using System.IO;

namespace ExceptionReporter
{
    

    public class ExceptionDatabaseReporter
    {

        //public static string OutputFilePath = "";
        public string msg = "";
        public int lineNo = -1;
        public string FileName = "";
        public string dataType = "";
        public string platform = "";
        public ExceptionDatabaseReporter(string m, int l, string fileName, string dt, string pl)
        {
            msg = m;
            lineNo = l;
            FileName = fileName;
            dataType = dt;
            platform = pl;
        }

        public void SendDatabaseExceptionThread()
        {
            try
            {

                //var reportThread = new Thread(SendDatabaseException);
                //reportThread.Start();
            }
            catch (Exception e)
            {
                try
                {
                    StreamWriter tempWriter = new StreamWriter("Network Failure_DBExp.txt", true);
                    tempWriter.WriteLine(DateTime.Now.ToString() + ": " + e.Message);
                    tempWriter.Close();
                    tempWriter.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public void SendDatabaseDuplicatesThread()
        {
            try
            {
                //var reportThread = new Thread(SendDatabaseDuplicates);
                //reportThread.Start();
            }
            catch (Exception e)
            {
                try
                {
                    StreamWriter tempWriter = new StreamWriter("Network Failure_DBDup.txt", true);
                    tempWriter.WriteLine(DateTime.Now.ToString() + ": " + e.Message);
                    tempWriter.Close();
                    tempWriter.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public void SendDatabaseException()
        {
            char tag = '\'';

            msg = msg.Replace('\'', ' ');
            msg = msg.Replace('\n', ' ');
            msg = msg.Replace(',', ' ');

            if (msg.Length > 1000)
                msg = msg.Remove(1000, msg.Length - 1001);

            string description = msg + " in " + FileName+ " with line: " + lineNo.ToString();
            List<Object> result = new List<object>();
            string insertMsg = "insert /*+ append */ into Exceptions values (" +
                               "to_date(" + tag + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                               tag + "yyyymmdd HH24:MI:SS" + tag + ")," + tag + dataType + tag + ",";
            insertMsg += tag + platform + tag + "," + tag + description + tag + ")";

            result.Add(insertMsg);
            ExceptionOracleWriter writer = new ExceptionOracleWriter("logger", "rl323");
            writer.Write(result);
        }

        public void SendDatabaseDuplicates()
        {
            char tag = '\'';

            List<Object> result = new List<object>();
            string insertMsg = "insert /*+ append */ into Duplicates values (" +
                               "to_date(" + tag + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + tag + "," +
                               tag + "yyyymmdd HH24:MI:SS" + tag + ")," + tag + dataType + tag + ")";

            result.Add(insertMsg);
            ExceptionOracleWriter writer = new ExceptionOracleWriter("logger", "rl323");
            writer.Write(result);
        }

        public static string DataTypeConverter(string msg)
        {
            try
            {
                if (msg.Contains("highway") || msg.Contains("Highway") || msg.Contains("HIGHWAY"))
                {
                    return SourceDataType.Freeway.ToString();
                }
                else if (msg.Contains("arterial") || msg.Contains("Arterial") || msg.Contains("ARTERIAL"))
                {
                    return SourceDataType.Arterial.ToString();
                }
                else if (msg.Contains("bus") || msg.Contains("Bus") || msg.Contains("BUS"))
                {
                    return SourceDataType.Bus.ToString();
                }
                else if (msg.Contains("rail") || msg.Contains("Rail") || msg.Contains("RAIL"))
                {
                    return SourceDataType.Rail.ToString();
                }
                else if ((msg.Contains("travel") || msg.Contains("Travel") || msg.Contains("TRAVEL")))
                {
                    return SourceDataType.TravelTime.ToString();
                }
                else if (msg.Contains("ramp") || msg.Contains("Ramp") || msg.Contains("RAMP"))
                {
                    return SourceDataType.Ramp.ToString();
                }
                else if (msg.Contains("event") || msg.Contains("Event") || msg.Contains("EVENT"))
                {
                    return SourceDataType.Event.ToString();
                }
                else if (msg.Contains("cms") || msg.Contains("Cms") || msg.Contains("CMS"))
                {
                    return SourceDataType.Cms.ToString();
                }
                else
                {
                    return "unknown";
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
