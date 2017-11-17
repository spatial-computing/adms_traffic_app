/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei (Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Add the UpdateRate for Bus, Rail, RMS, TravelTime
 * Date: 04/18/2011
 */

using System;
using System.IO;

namespace Utils
{
    public class Constants
    {
        public static String BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static String InputDataPath = Path.Combine(BaseDirectory, @"..\IOFiles\");
        public static String PcaCoefPath = Path.Combine(InputDataPath, @"ADMS\PCA_Coefs\");
        public static String PcaIndecesPath = Path.Combine(InputDataPath, @"ADMS\ind\");
        public static String RealTrafficDataPath = Path.Combine(InputDataPath, @"TrafficData\");
        public static readonly int FreewayUpdateRate =  30; 
        public static readonly int ArterialUpdateRate = 60;         //changed by Penny
        public static readonly int BusUpdateRate = 60;
        public static readonly int RailUpdateRate = 60;
        public static readonly int RampUpdateRate = 60;
        public static readonly int TravelTimeUpdateRate = 60;
        public static readonly int EventUpdateRate = 60;
        public static readonly int FreewayInventoryUpdateRate = 86400;
        public static readonly int ArterialInventoryUpdateRate = 86400;
        public static readonly int CmsInventoryUpdateRate = 86400;
        //public static int DataSendPeriod= 20;

        public static String GetPCFileName(String preprocessedTableName)
        {
            return "PCs" + preprocessedTableName + ".txt";
        }

        public static String GetTransFormedDataString(String preprocessedTableName)
        {
            return "TransData" + preprocessedTableName + ".txt";
        }

        public static String GetMuString(String preprocessedTableName)
        {
            return "mu" + preprocessedTableName + ".txt";
        }

        public static String GetPreprocessedTableString(String unprocessedTableName)
        {
            return "Fresh_" + unprocessedTableName;
        }

        public static String GetUnprocessedTableString(String highwayName, int direction)
        {
            return "jj_" + highwayName + "_" + direction;
        }

        public static String GetAllDaysString(String preprocessedTableName)
        {
            return preprocessedTableName + "days" + ".txt";
        }

        public static String GetAllSensorsString(String preprocessedTableName)
        {
            return preprocessedTableName + "sensors" + ".txt";
        }

        public static String GetSensorDaysString(String preprocessedTableName)
        {
            return preprocessedTableName + "sensorDay" + ".txt";
        }

        public static String GetSensorTableString(String preprocessedTableName)
        {
            return preprocessedTableName + "sensors" + ".txt";
        }

        public static String GetConfigurationFileCulture()
        {
            return "en-US";
        }

        //public static String GetAllPostMilestring(String preprocessedTableName)
        //{
        //    return preprocessedTableName + "postmiles" + ".txt";
        //}
    }
}