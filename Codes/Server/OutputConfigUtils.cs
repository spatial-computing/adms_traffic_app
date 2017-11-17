/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */


/**
 * Updated by Bei (Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Added OracleOutput config functions
 * Date: 04/18/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Purpose: Added the Azure Table config functions. Added the OracleOutput config for events.
 * Date: 06/09/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBServices;
using EventTypes;
using XMLOutputAdapter;

namespace SIServers
{
    internal class OutputConfigUtils
    {
        public static TrafficOutputConfig GetSensorSpeedSQLAzureFreewayOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Freeway.ToString(),
                OutputMessageType = OutputMessageType.DB.ToString(),
                OutputMediaType = OutputMediaType.SQLAzure.ToString(),
                OutputFileName = outputDirectory + "HighwayTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency=agency
            };
        }

        public static TrafficOutputConfig GetSensorSpeedAzureTableFreewayOutputConf(string tableName, string agency)
        {
            
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Freeway.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                TableName = tableName,
                Agency = agency

            };
        }


        public static TrafficOutputConfig GetSensorSpeedLocalDBFreewayOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Freeway.ToString(),
                OutputMessageType = OutputMessageType.DB.ToString(),
                OutputMediaType = OutputMediaType.LocalDB.ToString(),
                OutputFileName = outputDirectory + "HighwayTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetSensorSpeedXMLFreewayOutputConf(string outputDirectory, string agency)
        {
            XMLMessageConfig badbakhti = new XMLMessageConfig(FieldOrders.GetFreewayOutputXMLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Freeway.ToString(),
                OutputMessageType = OutputMessageType.XML.ToString(),
                OutputMediaType = OutputMediaType.File.ToString(),
                OutputFileName = outputDirectory + "HighwayTraffic.xml",
                Header = badbakhti.Header,
                RootName = badbakhti.RootName,
                OtherTopStories = badbakhti.OtherTopStories,
                OutputFieldOrders = badbakhti.OutputFieldOrders
            };
        }


        public static TrafficOutputConfig GetSensorSpeedSQLAzureArterialOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetArterialOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Arterial.ToString(),
                OutputMessageType = OutputMessageType.DB.ToString(),
                OutputMediaType = OutputMediaType.SQLAzure.ToString(),
                OutputFileName = outputDirectory + "ArterialTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetSensorSpeedAzureTableArterialOutputConf(string tableName, string agency)
        {
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Arterial.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                //OutputFileName = outputDirectory + "ArterialTraffic.xml",
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetAzureTableBusGPSOutputConf(string tableName, string agency)
        {
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Bus.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                //OutputFileName = outputDirectory + "BusGPS.xml",
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetAzureTableEventOutputConf(string tableName, string agency)
        {
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Event.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                // note: if interested to write in XML, should create different files for different event sources. same as any other data source.
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetAzureTableTravelOutputConf(string tableName, string agency)
        {
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.TravelTime.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                //OutputFileName = outputDirectory + "TravelTime.xml",
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetAzureTableRailGPSOutputConf(string tableName, string agency)
        {
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Rail.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                //OutputFileName = outputDirectory + "RailGPS.xml",
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetAzureTableRampOutputConf(string tableName, string agency)
        {
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Ramp.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                //OutputFileName = outputDirectory + "RampGPS.xml",
                TableName = tableName,
                Agency = agency
            };
        }


        public static TrafficOutputConfig GetAzureTableCMSOutputConf(string tableName, string agency)
        {
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Cms.ToString(),
                OutputMessageType = OutputMessageType.TableStorage.ToString(),
                OutputMediaType = OutputMediaType.AzureTable.ToString(),
                //OutputFileName = outputDirectory + "ArterialTraffic.xml",
                TableName = tableName,
                Agency = agency
            };
        }


        public static TrafficOutputConfig GetSensorSpeedLocalDBArterialOutputConf(string outputDirectory, string tableName,string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetArterialOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Arterial.ToString(),
                OutputMessageType = OutputMessageType.DB.ToString(),
                OutputMediaType = OutputMediaType.LocalDB.ToString(),
                OutputFileName = outputDirectory + "ArterialTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetSensorSpeedXMLArterialOutputConf(string outputDirectory, string agency)
        {
            XMLMessageConfig badbakhti = new XMLMessageConfig(FieldOrders.GetArterialOutputXMLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Arterial.ToString(),
                OutputMessageType = OutputMessageType.XML.ToString(),
                OutputMediaType = OutputMediaType.File.ToString(),
                OutputFileName = outputDirectory + "ArterialTraffic.xml",
                Header = badbakhti.Header,
                RootName = badbakhti.RootName,
                OtherTopStories = badbakhti.OtherTopStories,
                OutputFieldOrders = badbakhti.OutputFieldOrders
            };
        }

        private static XMLMessageConfig GetOutputXMLMessageConfigFreeway(string agency)
        {

            XMLMessageConfig result = new XMLMessageConfig(FieldOrders.GetFreewayOutputXMLFieldOrders(), agency);

            return result;
        }

        public static TrafficOutputConfig GetOracleEventOutputConf(string tableName,string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Event.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFieldOrders = khoshbakhti.OutputFieldOrders, // depricated! may be deleted. GetFields does the job instead.
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetOracleFreewayOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Freeway.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFileName = outputDirectory + "HighwayTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetOracleArterialOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetArterialOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Arterial.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFileName = outputDirectory + "ArterialTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }


        public static TrafficOutputConfig GetOracleBusOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Bus.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFileName = outputDirectory + "BusTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetOracleRailOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Rail.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFileName = outputDirectory + "RailTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetOracleRampOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Ramp.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFileName = outputDirectory + "RampTraffic.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetOracleTravelOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.TravelTime.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFileName = outputDirectory + "TravelTime.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }

        public static TrafficOutputConfig GetOracleCmsOutputConf(string outputDirectory, string tableName, string agency)
        {
            SQLMessageConfig khoshbakhti = new SQLMessageConfig(tableName, FieldOrders.GetFreewayOutputSQLFieldOrders(), agency);
            return new TrafficOutputConfig()
            {
                SourceDataType = SourceDataType.Cms.ToString(),
                OutputMessageType = OutputMessageType.Oracle.ToString(),
                OutputMediaType = OutputMediaType.Oracle.ToString(),
                OutputFileName = outputDirectory + "Cms.xml",
                OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                TableName = tableName,
                Agency = agency
            };
        }


    }
}
