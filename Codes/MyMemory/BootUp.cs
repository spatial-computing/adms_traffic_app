/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Load Bus, Ramp, Rail, TravelTime Inventory data
 * 1. Add max_config ID, dictionary varible members
 * 2. Add Load from XML functions, Add Load from Database functions
 * 3. Add Get functions
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Added writing configurations to Azure tables.
 * Date: 06/19/2011
 */

using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
//using DBServices;
using Microsoft.SqlServer.Types;
using MyMemory;
using Parsers;
using Utils;
using MediaWriters;

namespace MyMemory
{
    public class BootUp
    {
        public static int ServerPort = 11112;

        public static readonly string InputDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\IOFiles\");
        private static BootUp instance;
        public static int maxFreewayConfigID;
        public static int maxArterialConfigID;
        public static int maxBusConfigID;
        public static int maxTravelTimeConfigID;
        public static int maxRailConfigID;
        public static int maxRampMeterConfigID;
        public static int maxCmsConfigID;

        private static Dictionary<string, Sensor> arterialLinkLocDic = new Dictionary<string, Sensor>();
        private static Dictionary<string, Sensor> freewayLinkLocDic = new Dictionary<string, Sensor>();

        private static Dictionary<int, BusRoute> busRouteLocDic = new Dictionary<int, BusRoute>();
        private static Dictionary<int, travelLinks> travelLinkLocDic = new Dictionary<int, travelLinks>();
        private static Dictionary<int, RampMeter> rampMeterLocDic = new Dictionary<int, RampMeter>();
        private static Dictionary<int, RailRoute> railRouteLocDic = new Dictionary<int, RailRoute>();
        private static Dictionary<int, CmsDevice> cmsLinkLocDic = new Dictionary<int, CmsDevice>();
        

        private BootUp(string DBAddress)
        {
            
            //string cwd = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            try
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"HighwayTraffic.xml");
            }
            catch (FileNotFoundException e)
            {
            }
            try
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"AverageSpeed.xml");
            }
            catch (FileNotFoundException e)
            {
            }
            try
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"PredictedSpeeds.xml");
            }
            catch (FileNotFoundException e)
            {
            }
            //todo : exception handling for here.

            
            /*
            //todo : exception handling for here.
            maxFreewayConfigID = new OracleMediaWriter(DBTableStrings.Freeway_user, DBTableStrings.Freeway_pwd)
                                    .ReadMaxConfigID(DBTableStrings.FreewayConfigTableName);
            if (maxFreewayConfigID < 0)
                maxFreewayConfigID = 0;

            maxArterialConfigID = new OracleMediaWriter(DBTableStrings.Arterial_user, DBTableStrings.Arterial_pwd)
                                    .ReadMaxConfigID(DBTableStrings.ArterialConfigTableName);
            if (maxArterialConfigID < 0)
                maxArterialConfigID = 0;

            maxBusConfigID = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd)
                        .ReadMaxConfigID(DBTableStrings.MetroBusConfigTableName);
            if (maxBusConfigID < 0)
                maxBusConfigID = 0;

            maxRailConfigID = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd)
                        .ReadMaxConfigID(DBTableStrings.MetroRailConfigTableName);
            if (maxRailConfigID < 0)
                maxRailConfigID = 0;

            maxTravelTimeConfigID = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd)
                        .ReadMaxConfigID(DBTableStrings.TravelTimeConfigTableName);
            if (maxTravelTimeConfigID < 0)
                maxTravelTimeConfigID = 0;

            maxRampMeterConfigID = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd)
                        .ReadMaxConfigID(DBTableStrings.RampMeterConfigTableName);
            if (maxRampMeterConfigID < 0)
                maxRampMeterConfigID = 0;


            maxCmsConfigID = new OracleMediaWriter(DBTableStrings.Event_user, DBTableStrings.Event_pwd)
            .ReadMaxConfigID(DBTableStrings.CmsConfigTableName);
            if (maxCmsConfigID  < 0)
                maxCmsConfigID = 0;
            */

            maxFreewayConfigID = GetMaxConfigId(DBTableStrings.Freeway_user, DBTableStrings.Freeway_pwd, DBTableStrings.FreewayConfigTableName);
            maxArterialConfigID = GetMaxConfigId(DBTableStrings.Arterial_user, DBTableStrings.Arterial_pwd, DBTableStrings.ArterialConfigTableName);
            maxBusConfigID = GetMaxConfigId(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd, DBTableStrings.MetroBusConfigTableName);
            maxRailConfigID = GetMaxConfigId(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd, DBTableStrings.MetroRailConfigTableName);
            maxTravelTimeConfigID = GetMaxConfigId(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd, DBTableStrings.TravelTimeConfigTableName);
            maxRampMeterConfigID = GetMaxConfigId(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd, DBTableStrings.RampMeterConfigTableName);
            maxCmsConfigID = GetMaxConfigId(DBTableStrings.Event_user, DBTableStrings.Event_pwd, DBTableStrings.CmsConfigTableName);


            //test part, Penny
            DateTime myTime;
            //LoadArterialSensorInfo(out myTime);
            //LoadRampInfo(out myTime);
            LoadBusRouteInfo(out myTime);

            var periodicFreewayInventoryLoader = new Thread(PeriodicFreewayInventoryUpdate);
            periodicFreewayInventoryLoader.Start();

            var periodicArterialInventoryLoader = new Thread(PeriodicArterialInventoryUpdate);
            periodicArterialInventoryLoader.Start();


            var periodicBusInventoryLoader = new Thread(PeriodicBusInventoryUpdate);
            periodicBusInventoryLoader.Start();

            var periodicRailInventoryLoader = new Thread(PeriodicRailInventoryUpdate);
            periodicRailInventoryLoader.Start();

            var periodicRampInventoryLoader = new Thread(PeriodicRampInventoryUpdate);
            periodicRampInventoryLoader.Start();

            var periodicTravelTimeInventoryLoader = new Thread(PeriodicTravelTimeInventoryUpdate);
            periodicTravelTimeInventoryLoader.Start();

            var periodicCmsInventoryLoader = new Thread(PeriodicCmsInventoryUpdate);
            periodicCmsInventoryLoader.Start();
            
        }

        private static Object freewayWriteLock = new object();
        private static Object arterialWriteLock = new object();

        public int GetMaxConfigId(string user, string password, string tableName)
        {
            OracleMediaWriter oracleReader = new OracleMediaWriter(user, password);
            int configID = -1;
            while (configID == -1)
            {
                configID = oracleReader.ReadMaxConfigID(tableName);

                if (configID != -1)
                    break;

                Thread.Sleep(60 * 1000);
            }
            return configID;

        }

        //todo: what if somebody wants to read while you want to write? is it safe?

        public static Dictionary<string, Sensor> ArterialLinkLocDic
        {
            get { return arterialLinkLocDic; }
            set
            {
                lock (arterialWriteLock)
                {
                    arterialLinkLocDic = value;
                }
            }
        }


  
        static bool firstTimeFreeway = true;
        static bool firstTimeArterial = true;
        static bool firstTimeBus = true;
        static bool firstTimeRail = true;
        static bool firstTimeRamp = true;
        static bool firstTimeTravel = true;
        static bool firstTimeCms = true;


        public static Dictionary<string, Sensor> FreewayLinkLocDic
        {
            get { return freewayLinkLocDic; }
            set
            {
                lock( freewayWriteLock)
                {
                    freewayLinkLocDic = value;
                }
            }
        }



        private static Dictionary<string, Sensor> FindDiff(Dictionary<string, Sensor> current, Dictionary<string, Sensor> recent)
        {
            Dictionary<string, Sensor> changedValues = new Dictionary<string, Sensor>();
            if (current.Count == 0) // first call to this function
                return recent;

            foreach (KeyValuePair<string, Sensor> pair in recent)
            {
                Sensor newValue = pair.Value;
                Sensor previousValue = null;
                
                current.TryGetValue(pair.Key, out previousValue);
                if (previousValue == null)
                    changedValues.Add(pair.Key, newValue);
                else
                {
                    if (!newValue.AllFieldsEqual(previousValue))
                        changedValues.Add(pair.Key, newValue);
                }
            }

            return changedValues;

        }



        private static void PeriodicArterialInventoryUpdate()
        {

            OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Arterial_user, DBTableStrings.Arterial_pwd);

            if (!firstTimeArterial)
            {
                Thread.Sleep(1000 * Constants.ArterialInventoryUpdateRate);
            }
            else
            {
                
                string result = writer.ReadConfig(DBTableStrings.ArterialConfigTableName, maxArterialConfigID);

                if (result.Length > 0)
                {
                    arterialLinkLocDic = OracleMediaWriter.OracleParsing2SensorList(result);
                    firstTimeArterial = false;
                    //return;
                }
            }
            DateTime packetDateTime;
            Dictionary<string, Sensor> recentArterial = LoadArterialSensorInfo(out packetDateTime);

            Dictionary<string, Sensor> difList = null;
            difList = BootUpFunctions.FindDiff(ArterialLinkLocDic, recentArterial);

            if (difList.Count == 0)
                return;
            UpdateSensorList<Sensor>.UpdateCurrentList(ArterialLinkLocDic, difList);
            maxArterialConfigID++;
            writer.UpdateCongestionDatabase(DBTableStrings.ArterialConfigTableName,
                maxArterialConfigID, recentArterial);

            List<Object> pairsList = new List<Object>();

            foreach (Sensor sensor in recentArterial.Values)
            {
                pairsList.Add(new SensorConfigEntityTuple(sensor, maxArterialConfigID, packetDateTime));
            }
            new AzureTableStorageMediaWriter<SensorConfigEntityTuple>(AzureTableStrings.ArterialConfigTableName).Write(pairsList);


            firstTimeArterial = false;
        }


        private static void PeriodicCmsInventoryUpdate()
        {

            OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Event_user, DBTableStrings.Event_pwd);

            if (!firstTimeCms)
            {
                Thread.Sleep(1000 * Constants.CmsInventoryUpdateRate);
            }
            else
            {

                string result = writer.ReadConfig(DBTableStrings.CmsConfigTableName, maxCmsConfigID);

                if (result.Length > 0)
                {
                    cmsLinkLocDic = OracleMediaWriter.OracleParsing2CmsDevice(result);
                    firstTimeCms = false;
                    //return;
                }
            }
            DateTime packetDateTime;
            Dictionary<int, CmsDevice> recentCms = LoadCmsDeviceInfo(out packetDateTime);

            Dictionary<int, CmsDevice> difList = null;
            difList = BootUpFunctions.FindDiff(cmsLinkLocDic, recentCms);

            if (difList.Count == 0)
                return;
            UpdateList<CmsDevice>.UpdateCurrentList(cmsLinkLocDic, difList);
            maxCmsConfigID++;
            writer.UpdateEventDatabase(DBTableStrings.CmsConfigTableName,
                maxCmsConfigID, recentCms);

            /**/

            List<Object> pairsList = new List<Object>();

            foreach (CmsDevice device in recentCms.Values)
            {
                pairsList.Add(new CmsDeviceEntityTuple(device, maxCmsConfigID, packetDateTime));
            }
            new AzureTableStorageMediaWriter<CmsDeviceEntityTuple>(AzureTableStrings.CMSConfigTableName).Write(pairsList);

            
            firstTimeCms = false;
        }


        private static void PeriodicFreewayInventoryUpdate()
        {
            OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Freeway_user, DBTableStrings.Freeway_pwd);
            if (!firstTimeFreeway)
            {
                Thread.Sleep(1000 * Constants.FreewayInventoryUpdateRate);
            }
            else
            {
                //first try to load from database
                
                string result = writer.ReadConfig(DBTableStrings.FreewayConfigTableName, maxFreewayConfigID);

                if (result.Length > 0)
                {
                    freewayLinkLocDic = OracleMediaWriter.OracleParsing2SensorList(result);
                    firstTimeFreeway = false;
                    //return;
                }
            }

            DateTime packetDateTime;
            Dictionary<string, Sensor> recentFreeway = LoadFreewaySensorInfo(out packetDateTime);

            Dictionary<string, Sensor> difList = null;
            difList = BootUpFunctions.FindDiff(freewayLinkLocDic, recentFreeway);

            if (difList.Count == 0)
                return;
            UpdateSensorList<Sensor>.UpdateCurrentList(freewayLinkLocDic, difList);
            maxFreewayConfigID++;

            //Penny delete the two services, merge them to OracleMediaWriter and SqlMediaWriter
            //SQLDBServices.UpdateDatabase(DBTableStrings.FreewayConfigTableName,"Freeway", FieldOrders.GetFreewayConfigOutputSQLFieldOrders(),packetDateTime, maxFreewayConfigID, difList);
            //OracleDBServices.UpdateDatabase(DBTableStrings.FreewayConfigTableName, "Freeway", FieldOrders.GetFreewayConfigOutputSQLFieldOrders(), packetDateTime, maxFreewayConfigID, difList);


            writer.UpdateCongestionDatabase(DBTableStrings.FreewayConfigTableName,
                maxFreewayConfigID, recentFreeway);

            List<Object> pairsList = new List<Object>();
            
            foreach (Sensor sensor in recentFreeway.Values)
            {
                pairsList.Add(new SensorConfigEntityTuple(sensor, maxFreewayConfigID, packetDateTime));//, "Freeway"));
            }
            new AzureTableStorageMediaWriter<SensorConfigEntityTuple>(AzureTableStrings.FreewayConfigTableName).Write(
                pairsList);


            firstTimeFreeway = false;
        }

        private static void PeriodicBusInventoryUpdate()
        {
            OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            if (!firstTimeBus)
            {
                Thread.Sleep(1000 * Constants.FreewayInventoryUpdateRate);
            }
            else
            {
                //first try to load from database
                
                string result = writer.ReadConfig(DBTableStrings.MetroBusConfigTableName, maxBusConfigID);

                if (result.Length > 0)
                {
                    //freewayLinkLocDic = OracleParsing2SensorList(result);
                    busRouteLocDic = OracleMediaWriter.OracleParsing2BusList(result);
                    firstTimeBus = false;
                    //return;
                }
            }
            DateTime packetDateTime;
            Dictionary<int, BusRoute> recent = LoadBusRouteInfo(out packetDateTime);

            Dictionary<int, BusRoute> difList = null;
            difList = BootUpFunctions.FindDiff(busRouteLocDic, recent);

            if (difList.Count == 0)
                return;
            UpdateList<BusRoute>.UpdateCurrentList(busRouteLocDic, difList);

            maxBusConfigID++;
            //UpdateDatabase(DBTableStrings.FreewayConfigTableName, maxFreewayConfigID, difList);
            writer.UpdateTransitDatabase(DBTableStrings.MetroBusConfigTableName,
                maxBusConfigID, recent);

            List<Object> pairsList = new List<Object>();

            foreach (BusRoute sensor in recent.Values)
            {
                pairsList.Add(new BusRouteEntityTuple(sensor, maxBusConfigID, packetDateTime));
            }
            new AzureTableStorageMediaWriter<BusRouteEntityTuple>(AzureTableStrings.BusRoutesTableName).Write(pairsList);


            firstTimeBus = false;
        }

        private static void PeriodicRailInventoryUpdate()
        {
            OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            if (!firstTimeRail)
            {
                Thread.Sleep(1000 * Constants.FreewayInventoryUpdateRate);
            }
            else
            {
                //first try to load from database
                
                string result = writer.ReadConfig(DBTableStrings.MetroRailConfigTableName, maxRailConfigID);
                if (result.Length > 0)
                {
                    railRouteLocDic = OracleMediaWriter.OracleParsing2RailList(result);
                    firstTimeRail = false;
                    //return;
                }
            }
            DateTime packetDateTime;
            Dictionary<int, RailRoute> recent = LoadRailRouteInfo( out packetDateTime);

            Dictionary<int, RailRoute> difList = null;
            difList = BootUpFunctions.FindDiff(railRouteLocDic, recent);

            if (difList.Count == 0)
                return;
            UpdateList<RailRoute>.UpdateCurrentList(railRouteLocDic, difList);

            maxRailConfigID++;
            //UpdateDatabase(DBTableStrings.FreewayConfigTableName, maxFreewayConfigID, difList);
            writer.UpdateTransitDatabase(DBTableStrings.MetroRailConfigTableName,
                maxRailConfigID, recent);

            List<Object> pairsList = new List<Object>();

            foreach (RailRoute sensor in recent.Values)
            {
                pairsList.Add(new RailRouteEntityTuple(sensor, maxRailConfigID, packetDateTime));
            }
            new AzureTableStorageMediaWriter<RailRouteEntityTuple>(AzureTableStrings.RailRoutesTableName).Write(pairsList);


            firstTimeRail = false;
        }

        private static void PeriodicRampInventoryUpdate()
        {
            OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            if (!firstTimeRamp)
            {
                Thread.Sleep(1000 * Constants.FreewayInventoryUpdateRate);
            }
            else
            {
                //first try to load from database
                
                string result = writer.ReadConfig(DBTableStrings.RampMeterConfigTableName, maxRampMeterConfigID);
                if (result.Length > 0)
                {
                    rampMeterLocDic = OracleMediaWriter.OracleParsing2RampList(result);
                    firstTimeRamp = false;
                    //return;
                }
            }
            DateTime packetDateTime;
            Dictionary<int, RampMeter> recent = LoadRampInfo(out packetDateTime);
            Dictionary<int, RampMeter> difList = null;
            difList = BootUpFunctions.FindDiff(rampMeterLocDic, recent);

            if (difList.Count == 0)
                return;
            UpdateList<RampMeter>.UpdateCurrentList(rampMeterLocDic, difList);

            maxRampMeterConfigID++;
            //UpdateDatabase(DBTableStrings.FreewayConfigTableName, maxFreewayConfigID, difList);
            writer.UpdateTransitDatabase(DBTableStrings.RampMeterConfigTableName,
                maxRampMeterConfigID, recent);

            List<Object> pairsList = new List<Object>();

            foreach (RampMeter sensor in recent.Values)
            {
                pairsList.Add(new RampMeterEntityTuple(sensor, maxRampMeterConfigID, packetDateTime));
            }
            new AzureTableStorageMediaWriter<RampMeterEntityTuple>(AzureTableStrings.RampMeterTableName).Write(pairsList);


            firstTimeRamp = false;
        }

        private static void PeriodicTravelTimeInventoryUpdate()
        {
            OracleMediaWriter writer = new OracleMediaWriter(DBTableStrings.Transit_user, DBTableStrings.Transit_pwd);
            if (!firstTimeTravel)
            {
                Thread.Sleep(1000 * Constants.FreewayInventoryUpdateRate);
            }
            else
            {
                //first try to load from database               
                string result = writer.ReadConfig(DBTableStrings.TravelTimeConfigTableName, maxTravelTimeConfigID);
                if (result.Length > 0)
                {
                    travelLinkLocDic = OracleMediaWriter.OracleParsing2TravelList(result);
                    firstTimeTravel = false;
                    //return;
                }
            }
            DateTime packetDateTime;
            Dictionary<int, travelLinks> recent = LoadTravelLinkInfo(out packetDateTime);

            Dictionary<int, travelLinks> difList = null;
            difList = BootUpFunctions.FindDiff(travelLinkLocDic, recent);

            if (difList.Count == 0)
                return;
            UpdateList<travelLinks>.UpdateCurrentList(travelLinkLocDic, difList);

            maxTravelTimeConfigID++;
            //UpdateDatabase(DBTableStrings.FreewayConfigTableName, maxFreewayConfigID, difList);
            writer.UpdateTransitDatabase(DBTableStrings.TravelTimeConfigTableName,
                maxTravelTimeConfigID, recent);


            List<Object> pairsList = new List<Object>();

            foreach (travelLinks sensor in recent.Values)
            {
                pairsList.Add(new TravelTimeConfigEntityTuple(sensor, maxTravelTimeConfigID, packetDateTime));
            }
            new AzureTableStorageMediaWriter<TravelTimeConfigEntityTuple>(AzureTableStrings.TravelTimeConfigTableName).Write(pairsList);


            firstTimeTravel = false;
        }

        
        //excluding the endTime
        public double AllAverage(String tableName, DateTime startDate, DateTime endDate)
        {
            double averageSpeed = 0;
            string query = "select avg(speed) from " + tableName + " where " +
                           "date_and_time >= to_date( '" + startDate.ToString("yyyyMMdd HH:mm") +
                           "', 'YYYYMMDD HH24:MI') " +
                            "and date_and_time < to_date( '" + endDate.ToString("yyyyMMdd HH:mm") + "', 'YYYYMMDD HH24:MI')";

            string connectionString = "Driver={Microsoft ODBC for Oracle};Server=geodb.usc.edu:1521/geodbs;Uid=cs599;Pwd=cs599;";
            OdbcConnection connection = new OdbcConnection(connectionString);
            //  TextWriter deleteme = new StreamWriter(@"C:\deleteme.txt");
            // deleteme.WriteLine(query + timePart);
            //deleteme.Close();
            OdbcCommand command = new OdbcCommand(query, connection);
            connection.Open();
            OdbcDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader[0].GetType() != typeof(System.DBNull))
                {
                    decimal dec = (decimal)reader[0];
                    double dd;
                    Double.TryParse(dec.ToString(), out dd);
                    averageSpeed = dd;
                }

                //var average = reader[0];
            }
            connection.Close();

            return averageSpeed;

        }

        public  double AverageForRegion(String tableName, DateTime startDate, DateTime endDate, double lat1, double lng1, double lat2, double lng2)
        {

            String polygonText = "POLYGON (("; 
            double minlat = Math.Min(lat1, lat2);
            double maxlat = Math.Max(lat1, lat2);

            double minlong = Math.Min(lng1, lng2);
            double maxlong = Math.Max(lng1, lng2);

            polygonText += minlong + " " + minlat + ", ";
            polygonText += maxlong + " " + minlat + ", ";
            polygonText += maxlong + " " + maxlat + ", ";
            polygonText += minlong + " " + maxlat + ", ";
            polygonText += minlong + " " + minlat + "))";
            SqlGeography rectangle = SqlGeography.Parse(polygonText);
            List<string> filteredSensors = new List<string>();
            foreach (KeyValuePair<string, Sensor> sensor in arterialLinkLocDic)
            {
                if( rectangle.STIntersects(sensor.Value.GeogLocation).IsTrue)
                    filteredSensors.Add(sensor.Key);
            }
            string validSensors= "";
            foreach (string filteredSensor in filteredSensors)
            {
                validSensors += filteredSensor + ",";
            }
            validSensors= validSensors.Remove(validSensors.Length - 1);
            double averageSpeed = 0;
            string query = " select avg(speed) from "+tableName +" where link_id in (" + validSensors + ") ";// (759906, 759914)
            //string query = " select to_char( date_and_time, 'YYYYMMDD HH24:MI') chardate, speed from Feb17DemoArterial where link_id in (" + validSensors + ") ";// (759906, 759914)
            string timePart = " and date_and_time >= to_date( '" + startDate.ToString("yyyyMMdd HH:mm") +
                              "', 'YYYYMMDD HH24:MI')" +
                              "and date_and_time < to_date( '" + endDate.ToString("yyyyMMdd HH:mm") + "', 'YYYYMMDD HH24:MI')";
                             // "and date_and_time >= to_date( '20110118 09:00', 'YYYYMMDD HH24:MI')and date_and_time <= to_date( '20110118 12:00', 'YYYYMMDD HH24:MI')" +
                             // " order by chardate";
            string connectionString = "Driver={Microsoft ODBC for Oracle};Server=geodb.usc.edu:1521/geodbs;Uid=cs599;Pwd=cs599;";
            OdbcConnection connection = new OdbcConnection(connectionString);
          //  TextWriter deleteme = new StreamWriter(@"C:\deleteme.txt");
           // deleteme.WriteLine(query + timePart);
            //deleteme.Close();
            OdbcCommand command = new OdbcCommand(query + timePart, connection);
            connection.Open();
            OdbcDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                if (reader[0].GetType() != typeof(System.DBNull))
                {
                    decimal dec= (decimal) reader[0];
                    double dd;
                     Double.TryParse(dec.ToString(), out dd);
                    averageSpeed = dd;
                }

                //var average = reader[0];
            }
            connection.Close();

            

            return averageSpeed;
        }

        private static Dictionary<string, Sensor> LoadFreewaySensorInfo(out DateTime packetDateTime)
        {
            var result = new Dictionary<string, Sensor>();

            LoadSensorFromAgency(result, RIITSFreewayAgency.Caltrans_D8, out packetDateTime);
            LoadSensorFromAgency(result, RIITSFreewayAgency.Caltrans_D12, out packetDateTime);
            LoadSensorFromAgency(result, RIITSFreewayAgency.Caltrans_D7, out packetDateTime);
            
            return result;   
        }

        private static void LoadSensorFromAgency(Dictionary<string, Sensor> result, string agency, out DateTime packetDateTime)
        {
            try
            {

                //if no data exist in the database, ask load agent for more info
                var freewayInventoryParser = new FreewayInventoryParser(agency);
                freewayInventoryParser.Init();

                if (!freewayInventoryParser.XMLLoaded())
                {
                    //the ADMS server is being started. We need to have initial sensor location values but the RIITS server is not giving us that information. So we should load this information either from DB or local machine.
                    //rahe dige: age file moshkel dasht, boro catch kon va dar catch file khodet ro load kon.

                    packetDateTime = default(DateTime);
                    return;
                }
                packetDateTime = freewayInventoryParser.SpringOverHeaders(new CultureInfo(Constants.GetConfigurationFileCulture()));
                if (packetDateTime == default(DateTime))
                    return; //empty list
                while (freewayInventoryParser.IsNotEndElement())
                {
                    Sensor temp = freewayInventoryParser.ReadASensorInfo();
                    result.Add(temp.ID, temp);

                    freewayInventoryParser.ReadNode();
                }

            }
            catch (Exception e)
            {
                packetDateTime = default(DateTime);
                String Msg = " Freeway Inventory Load Exception: "+ agency;
                Console.Out.WriteLine(DateTime.Now + Msg);
            }
        }

        private static Dictionary<string, Sensor> LoadArterialSensorInfo(out DateTime packetDateTime)
        {
            var result = new Dictionary<string, Sensor>();

            LoadArterialSensorFromAgency(result, RIITSArterialAgency.LADOT, out packetDateTime);
            LoadArterialSensorFromAgency(result, RIITSFreewayAgency.Caltrans_D7, out packetDateTime);

            return result;
        }

        private static void LoadArterialSensorFromAgency(Dictionary<string, Sensor> result, string agency, out DateTime packetDateTime)
        {
            try
            {
                var arterialInventoryParser = new ArterialInventoryParser(agency);

                arterialInventoryParser.Init();

                if (!arterialInventoryParser.XMLLoaded())
                {
                    // the server is being started. We need to have initial sensor location values but the RIITS server is not giving us that information. So we should load this information either from DB or local machine.
                    packetDateTime = default(DateTime);
                    return;
                }
                packetDateTime =
                    arterialInventoryParser.SpringOverHeaders(new CultureInfo(Constants.GetConfigurationFileCulture()));
                if (packetDateTime == default(DateTime))
                    return; //empty list

                while (arterialInventoryParser.IsNotEndElement())
                {
                    Sensor temp = arterialInventoryParser.ReadASensorInfo();
                    result.Add(temp.ID, temp);

                    arterialInventoryParser.ReadNode();
                }
            }
            catch (Exception e)
            {
                packetDateTime = default(DateTime);
                String Msg = " Arterial Inventory Load Exception";
                Console.Out.WriteLine(DateTime.Now + Msg);
            }
        }

        private static Dictionary<int, CmsDevice> LoadCmsDeviceInfo(out DateTime packetDateTime)
        {
            var result = new Dictionary<int, CmsDevice>();
            LoadCmsDeviceFromAgency(result, RIITSFreewayAgency.Caltrans_D8, out packetDateTime);
            LoadCmsDeviceFromAgency(result, RIITSFreewayAgency.Caltrans_D12, out packetDateTime);
            LoadCmsDeviceFromAgency(result, RIITSFreewayAgency.Caltrans_D7, out packetDateTime);
            
            return result;
        }

        private static void LoadCmsDeviceFromAgency(Dictionary<int, CmsDevice> result, string agency, out DateTime packetDateTime)
        {
            try
            {
                var cmsDeviceParser = new CmsDeviceParser(agency);

                cmsDeviceParser.Init();

                if (!cmsDeviceParser.XMLLoaded())
                {
                    // the server is being started. We need to have initial sensor location values but the RIITS server is not giving us that information. So we should load this information either from DB or local machine.
                    packetDateTime = default(DateTime);
                    return;
                }
                packetDateTime =
                    cmsDeviceParser.SpringOverHeaders(new CultureInfo(Constants.GetConfigurationFileCulture()));
                if (packetDateTime == default(DateTime))
                    return; //empty list

                while (cmsDeviceParser.IsNotEndElement())
                {
                    CmsDevice temp = cmsDeviceParser.ReadADeviceInfo();
                    result.Add(temp.ID, temp);

                    cmsDeviceParser.ReadNode();
                }
            }
            catch (Exception e)
            {
                packetDateTime = default(DateTime);
                String Msg = " CMS Inventory Load Exception";
                Console.Out.WriteLine(DateTime.Now + Msg);


            }
        }


        private static Dictionary<int, BusRoute> LoadBusRouteInfo(out DateTime packetDateTime)
        {
            var result = new Dictionary<int, BusRoute>();
            LoadBusRouteFromAgency(result, RIITSBusAgency.LBT, out packetDateTime);
            LoadBusRouteFromAgency(result, RIITSBusAgency.FHT, out packetDateTime);
            LoadBusRouteFromAgency(result, RIITSBusAgency.MTA_Metro, out packetDateTime);
            
            return result;
        }

        private static void LoadBusRouteFromAgency(Dictionary<int, BusRoute> result, string agency, out DateTime packetDateTime)
        {
            try
            {
                //if no data exist in the database, ask load agent for more info
                var InventoryParser = new BusInventoryParser(agency);
                InventoryParser.Init();

                packetDateTime =
                    InventoryParser.SpringOverHeaders(new CultureInfo(Constants.GetConfigurationFileCulture()));
                if (packetDateTime == default(DateTime))
                    return ; //empty list
                while (InventoryParser.IsNotEndElement())
                {
                    BusRoute temp = InventoryParser.ReadABusRouteInfo();
                    result.Add(temp.routeId, temp);

                    InventoryParser.ReadNode();
                }
            }
            catch (Exception e)
            {
                packetDateTime = default(DateTime);
                String Msg = " Bus Inventory Load Exception";
                Console.Out.WriteLine(DateTime.Now + Msg);
            }
        }

        private static Dictionary<int, RailRoute> LoadRailRouteInfo(out DateTime packetDateTime)
        {
            var result = new Dictionary<int, RailRoute>();
            try
            {
                //if no data exist in the database, ask load agent for more info
                var InventoryParser = new RailInventory(RIITSBusAgency.MTA_Metro);
                InventoryParser.Init();

                packetDateTime =
                    InventoryParser.SpringOverHeaders(new CultureInfo(Constants.GetConfigurationFileCulture()));
                if (packetDateTime == default(DateTime))
                    return result; //empty list
                while (InventoryParser.IsNotEndElement())
                {
                    RailRoute temp = InventoryParser.ReadARailRouteInfo();
                    result.Add(temp.routeId, temp);

                    InventoryParser.ReadNode();
                }
            }
            catch (Exception e)
            {
                packetDateTime = default(DateTime);
                String Msg = " Rail Inventory Load Exception";
                Console.Out.WriteLine(DateTime.Now + Msg);


            }
            return result;
        }

        private static Dictionary<int, RampMeter> LoadRampInfo(out DateTime packetDateTime)
        {
            var result = new Dictionary<int, RampMeter>();
            LoadRampInfoFromAgency(result, RIITSFreewayAgency.Caltrans_D8, out packetDateTime);
            LoadRampInfoFromAgency(result, RIITSFreewayAgency.Caltrans_D12, out packetDateTime);
            LoadRampInfoFromAgency(result, RIITSFreewayAgency.Caltrans_D7, out packetDateTime);
            
            return result;
        }

        private static void LoadRampInfoFromAgency(Dictionary<int, RampMeter> result, string agency, out DateTime packetDateTime)
        {
            try
            {

                //if no data exist in the database, ask load agent for more info
                var InventoryParser = new RampInventoryParser(agency);
                InventoryParser.Init();

                packetDateTime =
                    InventoryParser.SpringOverHeaders(new CultureInfo(Constants.GetConfigurationFileCulture()));
                if (packetDateTime == default(DateTime))
                    return; //empty list
                while (InventoryParser.IsNotEndElement())
                {
                    RampMeter temp = InventoryParser.ReadARampMeterInfo();
                    result.Add(temp.ID, temp);

                    InventoryParser.ReadNode();
                }
            }
            catch (Exception e)
            {
                packetDateTime = default(DateTime);
                String Msg = " Ramp Inventory Load Exception";
                Console.Out.WriteLine(DateTime.Now + Msg);


            }
        }

        private static Dictionary<int, travelLinks> LoadTravelLinkInfo(out DateTime packetDateTime)
        {
            var result = new Dictionary<int, travelLinks>();
            LoadTravelLinkInfoFromAgency(result, RIITSFreewayAgency.Caltrans_D7, out packetDateTime);
            return result;
        }

        private static void LoadTravelLinkInfoFromAgency(Dictionary<int, travelLinks> result, string agency, out DateTime packetDateTime)
        {
            try
            {
                //if no data exist in the database, ask load agent for more info
                var InventoryParser = new TravelTimeInventoryParser(agency);
                InventoryParser.Init();

                packetDateTime =
                    InventoryParser.SpringOverHeaders(new CultureInfo(Constants.GetConfigurationFileCulture()));
                if (packetDateTime == default(DateTime))
                    return; //empty list
                while (InventoryParser.IsNotEndElement())
                {
                    travelLinks temp = InventoryParser.ReadATravelLinkInfo();
                    result.Add(temp.ID, temp);

                    InventoryParser.ReadNode();
                }
            }
            catch (Exception e)
            {
                packetDateTime = default(DateTime);
                String Msg = "TravelTime Inventory Load Exception";
                Console.Out.WriteLine(DateTime.Now + Msg);


            }
        }



        public Sensor GetFreewaySensInfo(string linkID)
        {
            Sensor result;

            freewayLinkLocDic.TryGetValue(linkID, out result);

            return result;

        }

        public Sensor GetArterialSensInfo(string linkID)
        {
            Sensor result;
            arterialLinkLocDic.TryGetValue(linkID, out result);
            return result;
        }

        public BusRoute GetBusRouteInfo(int routeID)
        {
            BusRoute result;
            busRouteLocDic.TryGetValue(routeID, out result);
            return result;
        }


        public RailRoute GetRailRouteInfo(int routeID)
        {
            RailRoute result;
            railRouteLocDic.TryGetValue(routeID, out result);
            return result;
        }


        public RampMeter GetRampMeterInfo(int ID)
        {
            RampMeter result;
            rampMeterLocDic.TryGetValue(ID, out result);
            return result;
        }

        public travelLinks GetTravelLinkInfo(int linkID)
        {
            travelLinks result;
            travelLinkLocDic.TryGetValue(linkID, out result);
            return result;
        }

        public static BootUp GetInstance()
        {
            if (instance == null)
                instance = new BootUp(string.Empty);
            return instance;
        }

        public static BootUp GetInstance(String DBAddress)
        {
            if (instance == null)
                instance = new BootUp(DBAddress);
            return instance;
        }

        #region Load Lat Long information

        //note: depricated! 
        private static Dictionary<string, Sensor> LoadArchivedFreewayLatLongs(String FileAddress)
            //TODO: temporary function for the time LocalDB is not available. You can load all objects in a better way later.
        {
            var result = new Dictionary<string, Sensor>();

            String path = Path.Combine(InputDataPath, @"SensorInfo\");
            var bformatter = new BinaryFormatter();
            Stream stream = File.Open(path + @"keys.ser",
                                      FileMode.Open);
            var sensorIDs = (List<string>) bformatter.Deserialize(stream);


            stream.Close();
            stream = File.Open(path + @"lats.ser",
                               FileMode.Open);
            var lats = (List<Double>) bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"longs.ser",
                               FileMode.Open);
            var longs = (List<Double>) bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"dircs.ser",
                               FileMode.Open);
            var dircs = (List<Int32>) bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"froms.ser",
                               FileMode.Open);
            var fromStreets = (List<String>) bformatter.Deserialize(stream);
            stream.Close();
            stream = File.Open(path + @"osts.ser",
                               FileMode.Open);
            var osts = (List<String>) bformatter.Deserialize(stream);
            stream.Close();
            for (int i = 0; i < sensorIDs.Count; i++)
                result.Add(sensorIDs[i],
                                      new Sensor(RIITSFreewayAgency.Caltrans_D7,sensorIDs[i], lats[i], longs[i], osts[i], dircs[i],
                                                        fromStreets[i]));
            return result;
        }

        #endregion

        public Sensor CheckFreewaySensor(string tempSensorId)
        {
            Sensor highway;
            FreewayLinkLocDic.TryGetValue(tempSensorId, out highway);
            return highway;

        }
    }
}