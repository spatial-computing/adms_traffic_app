/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Add Bus, Rail, RMS, TravelTime Query, StartQuery functions
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 05/11/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu)
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Purpose: Added writing to Azure Tables the following data: Bus, Rail, RMS, TravelTime, Events. Freeway and Arterial had been added initially.
 * Added writing to Oracle for event types (CHP-LA, D7, Regional-LA)
 * Date: 06/08/2011
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BaseOutputAdapter;
using BaseTrafficInputAdapters;
using EventTypes;

using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using MyMemory;
using Parsers;
using SocketInputAdapter;
using UDOs;
using Utils;
using XMLOutputAdapter;
using ExceptionReporter;

namespace SIServers
{
    public class ExplicitServer
    {
        private readonly DateTime alignment;
        private readonly Application application;
        private readonly String applicationName;
        private readonly String outputDirectory;



        private readonly Server server;
        private readonly OutputAdapter socketSensorOutputAdapter;
        private readonly TraceListener tracer;
        private readonly OutputAdapter xmlSensorOutputAdapter1;
        private readonly OutputAdapter xmlSensorOutputAdapter2;
        private readonly OutputAdapter SQLAzureSensorOutputAdapter3;

        private readonly Query freewayAverageQuery;
        private readonly Query XMLFreewayQuery;
        private readonly Query XMLArterialQuery;

        private readonly Query freewayPassThruQuery_D7;
        private readonly Query freewayFilteredQuery_D7;
        private readonly Query freewayPassThruQuery_D8;
        private readonly Query freewayFilteredQuery_D8;
        private readonly Query freewayPassThruQuery_D12;
        private readonly Query freewayFilteredQuery_D12;

        private readonly Query arterialPassThruQuery;
        private readonly Query arterialFilteredQuery;
        private readonly Query arterialPassThruQuery_D7;
        private readonly Query arterialFilteredQuery_D7;
        private readonly Query arterialPassThruQuery_D8;
        private readonly Query arterialFilteredQuery_D8;
        private readonly Query arterialPassThruQuery_D12;
        private readonly Query arterialFilteredQuery_D12;

        private readonly Query busPassThruQuery_MT;
        private readonly Query busFilteredQuery_MT;
        private readonly Query busPassThruQuery_FHT;
        private readonly Query busFilteredQuery_FHT;
        private readonly Query busPassThruQuery_LBT;
        private readonly Query busFilteredQuery_LBT;

        private readonly Query railPassThruQuery;
        private readonly Query railFilteredQuery;

        private readonly Query rampPassThruQuery_D7;
        private readonly Query rampFilteredQuery_D7;
        private readonly Query rampPassThruQuery_D8;
        private readonly Query rampFilteredQuery_D8;
        private readonly Query rampPassThruQuery_D12;
        private readonly Query rampFilteredQuery_D12;

        private readonly Query travelPassThruQuery_D7;
        private readonly Query travelFilteredQuery_D7;
        private readonly Query travelPassThruQuery_D8;
        private readonly Query travelFilteredQuery_D8;
        private readonly Query travelPassThruQuery_D12;
        private readonly Query travelFilteredQuery_D12;


        private readonly Query EventPassThruQuery_D7;
        private readonly Query EventOracleQuery_D7;
        private readonly Query EventPassThruQuery_D8;
        private readonly Query EventOracleQuery_D8;
        private readonly Query EventPassThruQuery_D12;
        private readonly Query EventOracleQuery_D12;

        private readonly Query RegionalLAEventPassThruQuery;
        private readonly Query RegionalLAEventOracleQuery;

        private readonly Query CHPLAEventPassThruQuery;
        private readonly Query CHPLAEventOracleQuery;
        private readonly Query CHPILEventPassThruQuery;
        private readonly Query CHPILEventOracleQuery;
        private readonly Query CHPOCEventPassThruQuery;
        private readonly Query CHPOCEventOracleQuery;

        private readonly Query cmsPassThruQuery_D7;
        private readonly Query cmsFilteredQuery_D7;
        private readonly Query cmsPassThruQuery_D8;
        private readonly Query cmsFilteredQuery_D8;
        private readonly Query cmsPassThruQuery_D12;
        private readonly Query cmsFilteredQuery_D12;

        private readonly Query azureTableStorageFreewayQuery;
        private readonly Query azureTableStorageArterialQuery;
        private readonly Query azureTableStorageBusGPSQuery;
        private readonly Query azureTableStorageRailGPSQuery;
        private readonly Query azureTableStorageRampQuery;
        private readonly Query azureTableStorageTravelTimeQuery;
        private readonly Query azureTableStorageD7EventQuery;
        private readonly Query azureTableStorageCHPLAEventQuery;
        private readonly Query azureTableStorageRegionalLAEventQuery;
        private readonly Query azureTableStorageCMSQuery;

        private readonly OutputAdapter XMLOutputAdapter_freeway;
        private readonly OutputAdapter XMLOutputAdapter_arterial;
        private readonly OutputAdapter oracleOutputAdapter_freeway_D7;
        private readonly OutputAdapter oracleOutputAdapter_freeway_D8;
        private readonly OutputAdapter oracleOutputAdapter_freeway_D12;
        private readonly OutputAdapter oracleOutputAdapter_arterial;
        private readonly OutputAdapter oracleOutputAdapter_arterial_D7;
        private readonly OutputAdapter oracleOutputAdapter_arterial_D8;
        private readonly OutputAdapter oracleOutputAdapter_arterial_D12;
        private readonly OutputAdapter oracleOutputAdapter_bus_MT;
        private readonly OutputAdapter oracleOutputAdapter_bus_LBT;
        private readonly OutputAdapter oracleOutputAdapter_bus_FHT;
        private readonly OutputAdapter oracleOutputAdapter_rail;
        private readonly OutputAdapter oracleOutputAdapter_ramp_D7;
        private readonly OutputAdapter oracleOutputAdapter_ramp_D8;
        private readonly OutputAdapter oracleOutputAdapter_ramp_D12;
        private readonly OutputAdapter oracleOutputAdapter_travel_D7;
        private readonly OutputAdapter oracleOutputAdapter_travel_D8;
        private readonly OutputAdapter oracleOutputAdapter_travel_D12;
        private readonly OutputAdapter oracleOutputAdapter_CHPLAEvent;
        private readonly OutputAdapter oracleOutputAdapter_CHPILEvent;
        private readonly OutputAdapter oracleOutputAdapter_CHPOCEvent;
        private readonly OutputAdapter oracleOutputAdapter_Event_D7;
        private readonly OutputAdapter oracleOutputAdapter_Event_D8;
        private readonly OutputAdapter oracleOutputAdapter_Event_D12;
        private readonly OutputAdapter oracleOutputAdapter_RegionalLAEvent;
        private readonly OutputAdapter oracleOutputAdapter_cms_D7;
        private readonly OutputAdapter oracleOutputAdapter_cms_D8;
        private readonly OutputAdapter oracleOutputAdapter_cms_D12;

        private readonly OutputAdapter azureTableOutputAdapter_freeway;
        private readonly OutputAdapter azureTableOutputAdapter_arterial;
        private readonly OutputAdapter azureTableOutputAdapter_bus;
        private readonly OutputAdapter azureTableOutputAdapter_rail;
        private readonly OutputAdapter azureTableOutputAdapter_ramp;
        private readonly OutputAdapter azureTableOutputAdapter_travel;
        private readonly OutputAdapter azureTableOutputAdapter_eventD7;
        private readonly OutputAdapter azureTableOutputAdapter_eventCHPLA;
        private readonly OutputAdapter azureTableOutputAdapter_eventRegionalLA;
        private readonly OutputAdapter azureTableOutputAdapter_cms;

        private int lastKMinutes = -1;
        private Query query3;
        private InputAdapter socketInputAdapter;
        //private Query passThruQuery2;
        public ExplicitServer(String DBAddress, String outputDirectory)
        {
            BootUp utils = BootUp.GetInstance(DBAddress);


            // DateTime start = new DateTime(2011, 1, 10, 9, 0, 0);
            //for (int i = 0; i < 180; i++)
            //{
            //    TextWriter writer = new StreamWriter(@"C:\Users\Jalal\Documents\My Dropbox\C# projects\ADMS\averagePerMinuteFreewayJan17th.xls", true);
            //    writer.WriteLine(start.AddMinutes(i).ToString() + "\t" + utils.AllAverage("FEB17DEMOHIGHWAY3HOURS", start.AddMinutes(i), start.AddMinutes(i + 1)));
            //    writer.Close();
            //}

            //for (int i = 0; i < 24; i++)
            //{
            //    TextWriter writer = new StreamWriter(@"C:\Users\Jalal\Documents\My Dropbox\C# projects\ADMS\averageHourlyFreewayJan17th.xls", true);
            //    writer.WriteLine(start.AddHours(i).ToString() + "\t" + utils.AllAverage("Feb17DemoHighway1Day", start.AddHours(i), start.AddHours(i + 1)));
            //    writer.Close();
            //}

            this.outputDirectory = outputDirectory;
            alignment = new DateTime(TimeSpan.FromHours(6).Ticks, DateTimeKind.Utc);
            tracer = new ConsoleTraceListener();
            tracer.WriteLine("Creating CEP Server");
            server = Server.Create("Infolab2");
            tracer.WriteLine("Nitin Comment" + server.ToString());
            
            // TODO: ask: what happens if I open two VSs and create a server in each of them?

            tracer.WriteLine("Creating CEP Application");

            application = server.CreateApplication(QueryStrings.ApplicationName);


            #region Jalal's previous part (2011/3/22) Commentted by Penny
            //create a query whose output will be fanned-out to other queries (dynamic query composition)

            //freewayPassThruQuery = QueryUtils.GetFreewayPassThroughQuery(application, QueryStrings.FreewayPassThruQueryName, "arterial pass thru query", RIITSFreewayAgency.Caltrans_D7);
            //freewayPassThruQuery.Start();

            /*            #region Arterial Query
                        arterialPassThruQuery = QueryUtils.GetArterialPassThroughQuery(application, QueryStrings.ArterialPassThruQueryName, "arterial pass thru query", RIITSArterialAgency.LADOT);
                        arterialPassThruQuery.Start();
                      //  filteredArterialQuery = GetQueryFilteredStream(application, arterialPassThruQuery.ToStream<TrafficSensorReading>(), QueryStrings.ArterialFilteredQueryName, "filtered arterial pass thrue query");
                        //GetQueryFilteredStream(sensorStream); don't use this! this causes another input adapter generation followed by exceptions in writing to output file.
                        //filteredArterialQuery.Start();
                        String arterialInputName = "arterialSensorInput";
                        //QueryTemplate arterialFilteredSensorSpeedQT = application.CreateQueryTemplate("ArterialFilteredSpeedQT", "",
                        //                                                                              StreamUtils.
                        //                                                                                  GetFilteredSensorSpeedStream
                        //                                                                                  (arterialInputName));
                        QueryTemplate arterialSelectedAreaSensorSpeedQT = application.CreateQueryTemplate("ArterialSelectedAreaSpeedQT", "",
                                                                                                      StreamUtils.GetUnboundFilteredSelectedAreaArterialStream(arterialInputName,alignment));

                        TrafficOutputConfig outputConfSensorSpeedXMLArterial = OutputConfigUtils.GetSensorSpeedXMLArterialOutputConf(outputDirectory);

                        xmlSensorOutputAdapter2 = application.CreateOutputAdapter<OutputAdapterFactory>("Xml Output2",
                                                                                                   "Writing result events to an xml file");
                        QueryBinder arterialSensorSpeedQB = BindQueryToAnotherQuery(arterialPassThruQuery, arterialInputName,
                                                                                    xmlSensorOutputAdapter2,
                                                                                    outputConfSensorSpeedXMLArterial,
                                                                                    //arterialFilteredSensorSpeedQT);
                                                                                    arterialSelectedAreaSensorSpeedQT);

                        arterialSelectedAreaQuery = application.CreateQuery("ArterialSelectedAreaSensorSpeedQuery", "arterial Current Speed", arterialSensorSpeedQB);
                        #endregion*/

            #region Event Query
            /*
            EventQuery = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.EventPassThruQueryName, "event pass thru query", RIITSEventAgency.Caltrans_D7);
            EventQuery.Start();
            */
            #endregion
            //  filteredFreewayQuery = GetQueryFilteredStream(application, freewayPassThruQuery.ToStream<TrafficSensorReading>(), QueryStrings.FreewayFilteredQueryName,"filtered freeway pass thrue query");
            //GetQueryFilteredStream(sensorStream); don't use this! this causes another input adapter generation followed by exceptions in writing to output file.
            //filteredFreewayQuery.Start();

            /*

             // Start the pass thru query
             String freewayInputName = "frewaySensorInput";


             QueryTemplate freewayFilteredSensorSpeedQT = application.CreateQueryTemplate("FreewayFilteredSpeedsQT", "",
                                                                                          StreamUtils.
                                                                                              GetFilteredSensorSpeedStream
                                                                                              (freewayInputName));
                                                                                              //todo: delete the following line and uncomment the above line. just for Barak.
                                                                                          //    GetUnboundFilteredSelectedAreaFreewayStream
                                                                                           //   (freewayInputName,alignment));
                                                                                             
                 //StreamUtils.GetFilteredSensorSpeedQT(application,freewayInputName, "FreewayAllSpeedsQT");
            

             //QueryTemplate sensorSpeedCloutQT = StreamUtils.GetFilteredSensorSpeedQT(application,inputName, "AllSpeeds LocalDB");

             //QueryTemplate avgSpeedForFreewaySltAreaQT = GetWeightAvgSpeedForSltAreaQT(application, StreamUtils.GetUnboundFilteredSelectedAreaFreewayStream(freewayInputName, alignment, new UDOConfig(BootUp.FreewayLinkLocDic)), QueryStrings.AverageSpeedFreewayQTName,"");
             //QueryTemplate avgSpeedForArterialSltAreaQT = GetWeightAvgSpeedForSltAreaQT(application, StreamUtils.GetUnboundSelectedAreaFreewayStream(arterialInputName, alignment, new UDOConfig(BootUp.ArterialLinkLocDic)), QueryStrings.AverageSpeedFreewayQTName, "");

             tracer.WriteLine("Registering Adapter Factories");
             //socketInputAdapter = application.CreateInputAdapter<SocketInputFactory>("Socket Input",
             //                                                                        "Reading tuples from Socket");
             xmlSensorOutputAdapter1 = application.CreateOutputAdapter<OutputAdapterFactory>("Xml Output",
                                                                                         "Writing result events to an xml file");
           
             SQLAzureSensorOutputAdapter3 = application.CreateOutputAdapter<OutputAdapterFactory>("LocalDB Output",
                                                                                          "Writing result events to SQL Azure");
             AzureTableStorageSensorOutputAdapter4 = application.CreateOutputAdapter<OutputAdapterFactory>("Azure Table Output",
                                                                                          "Writing result events to Azure Table");

             //socketSensorOutputAdapter = application.CreateOutputAdapter<SocketOutputFactory>("Raw sensor readings",
               //                                                                               "Writing sensor readings to socket");
             //OutputMessageConfig temp = GetOutputXMLMessageConfigFreeway();

             TrafficOutputConfig outputConfSensorSpeedXMLFreeway = OutputConfigUtils.GetSensorSpeedXMLFreewayOutputConf(outputDirectory); 

             TrafficOutputConfig outputConfSensorSpeedSQLAzureFreeway = OutputConfigUtils.GetSensorSpeedSQLAzureFreewayOutputConf(outputDirectory, DBTableStrings.FreewayTableName);

             TrafficOutputConfig outputConfSensorSpeedAzureTableFreeway =
                 OutputConfigUtils.GetSensorSpeedAzureTableFreewayOutputConf(AzureTableStrings.FreewayTableName);
            

             //TrafficOutputConfig averageSpeedOutputConf = GetAverageSpeedOutputConf(outputDirectory);
            
             //QueryBinder freewayFilteredSensorSpeedQB = BindQueryToAnotherQuery(freewayPassThruQuery, freewayInputName, xmlSensorOutputAdapter1,
               //                                                  outputConfSensorSpeedXMLFreeway,
                 //                                                freewayFilteredSensorSpeedQT);

             //QueryBinder sensorSpeedQBCloud = BindQueryToAnotherQuery(freewayPassThruQuery, freewayInputName, SQLAzureSensorOutputAdapter3,
               //                                                  outputConfSensorSpeedSQLAzureFreeway,
                 //                                                freewayFilteredSensorSpeedQT);

             //QueryBinder sensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(freewayPassThruQuery, freewayInputName, AzureTableStorageSensorOutputAdapter4,
               //                                                outputConfSensorSpeedAzureTableFreeway,
                 //                                            freewayFilteredSensorSpeedQT);
  

             //QueryBinder avgSpeedForSltAreaQB = BindQueryToAnotherQuery(passThruQuery2, inputName,
             //                                                           xmlSensorOutputAdapter2, averageSpeedOutputConf,
             //                                                           avgSpeedForSltAreaQT);


             tracer.WriteLine("Registering bound query");
             //freewayFilteredQuery = application.CreateQuery("FreewaySensorSpeedQuery", "freeway Current Speed", freewayFilteredSensorSpeedQB);
             //Query jjCloud = application.CreateQuery("SensorSpeedCloudQuery", "Current Speed LocalDB", sensorSpeedQBCloud);
             //jjCloud.Start();

             //Query azureTableStorageQ= application.CreateQuery("SensorSpeedAzureTableQuery", "Current Speed Table Storage", sensorSpeedQBAzureTableQB);
             //azureTableStorageQ.Start();
            

             //QueryBinder sensorSpeedCloudQB = BindQueryToAnotherQuery(filteredFreewayQuery, inputName,
             //                                                         SQLAzureSensorOutputAdapter3,
             //                                                         sensorSpeedOutputConfToCloud, sensorSpeedCloutQT);
             //Query cloudQuery = application.CreateQuery("SensorSpeedQueryCloud", "hichi", sensorSpeedCloudQB);
             //cloudQuery.Start();
            // freewayAverageQuery = application.CreateQuery("AverageSpeedQuery", "Average Speed", avgSpeedForSltAreaQB);
             // Query rawValues = application.CreateQuery("RawSensorSpeedQuery", "Raw Speed", socketSensorSpeedQB);
             //rawValues.Start();
             //SetK(1);
             tracer.WriteLine("Start query");
             * 
             */
            #endregion


            // Jalal Added XML and Azure tables.
            // #region Penny Added Part (Freeway, Arterial, Bus, Rail, RMS, TravelTime) for Oracle
            #region Higway Query D7 (Oracle & Azure)

            freewayPassThruQuery_D7 = QueryUtils.GetFreewayPassThroughQuery(application, QueryStrings.FreewayPassThruQueryName, "freeway pass thru query", RIITSFreewayAgency.Caltrans_D7);
            freewayPassThruQuery_D7.Start();
            // Start the pass thru query
            String freewayInputName = "frewaySensorInput";


            QueryTemplate freewayFilteredSensorSpeedQT = application.CreateQueryTemplate("FreewayFilteredSpeedsQT", "",
                                                                                         StreamUtils.
                                                                                             GetFilteredSensorSpeedStream
                                                                                             (freewayInputName));

            oracleOutputAdapter_freeway_D7 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Freeway",
                                                                                         "Writing freeway events to oracle");


            //Oracle Config
            TrafficOutputConfig outputConfSensorSpeedOracleFreeway = OutputConfigUtils.GetOracleFreewayOutputConf(outputDirectory,
                DBTableStrings.FreewayTableName, RIITSFreewayAgency.Caltrans_D7);

            QueryBinder sensorSpeedQBOracle = BindQueryToAnotherQuery(freewayPassThruQuery_D7, freewayInputName, oracleOutputAdapter_freeway_D7,
                                                    outputConfSensorSpeedOracleFreeway,
                                                    freewayFilteredSensorSpeedQT);

            
            freewayFilteredQuery_D7 = application.CreateQuery("FreewaySensorSpeedQuery", "freeway Current Speed", sensorSpeedQBOracle);

            azureTableOutputAdapter_freeway = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output Freeway",
                                                                                         "Writing freeway events to Azure table");


            TrafficOutputConfig outputConfSensorSpeedAzureTableFreeway =
                OutputConfigUtils.GetSensorSpeedAzureTableFreewayOutputConf(AzureTableStrings.FreewayDataTableName, RIITSFreewayAgency.Caltrans_D7);

            QueryBinder freewaySensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(freewayPassThruQuery_D7, freewayInputName, azureTableOutputAdapter_freeway,
                                                                         outputConfSensorSpeedAzureTableFreeway,
                                                                       freewayFilteredSensorSpeedQT);

            azureTableStorageFreewayQuery = application.CreateQuery("freewaySensorSpeedAzureTableQuery", "Current Speed Table Storage", freewaySensorSpeedQBAzureTableQB);

            
            TrafficOutputConfig outputConfSensorSpeedXMLFreeway = OutputConfigUtils.GetSensorSpeedXMLFreewayOutputConf(outputDirectory, RIITSFreewayAgency.Caltrans_D7);

            XMLOutputAdapter_freeway = application.CreateOutputAdapter<OutputAdapterFactory>("Xml Output2",
                                                                                       "Writing result events to an xml file");
            QueryBinder freewaySensorSpeedQB = BindQueryToAnotherQuery(freewayPassThruQuery_D7, freewayInputName,
                                                                        XMLOutputAdapter_freeway,
                                                                        outputConfSensorSpeedXMLFreeway,
                                                                        freewayFilteredSensorSpeedQT);

            XMLFreewayQuery = application.CreateQuery("FreewaySensorSpeedXMLQuery", "freeway Current Speed", freewaySensorSpeedQB);
            
            
            #endregion

            #region Highway Query D8 (Oracle)
            freewayPassThruQuery_D8 = QueryUtils.GetFreewayPassThroughQuery(application, QueryStrings.FreewayPassThruQueryName + "_D8", "freeway pass thru query", RIITSFreewayAgency.Caltrans_D8);
            freewayPassThruQuery_D8.Start();
            // Start the pass thru query
            String freewayInputName_D8 = "frewaySensorInput_D8";


            QueryTemplate freewayFilteredSensorSpeedQT_D8 = application.CreateQueryTemplate("FreewayFilteredSpeedsQT_D8", "",
                                                                                         StreamUtils.
                                                                                             GetFilteredSensorSpeedStream
                                                                                             (freewayInputName_D8));

            oracleOutputAdapter_freeway_D8 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Freeway D8",
                                                                                         "Writing freeway events to oracle D8");


            //Oracle Config
            TrafficOutputConfig outputConfSensorSpeedOracleFreeway_D8 = OutputConfigUtils.GetOracleFreewayOutputConf(outputDirectory,
                DBTableStrings.FreewayTableName, RIITSFreewayAgency.Caltrans_D8);

            QueryBinder sensorSpeedQBOracle_D8 = BindQueryToAnotherQuery(freewayPassThruQuery_D8, freewayInputName_D8, oracleOutputAdapter_freeway_D8,
                                                    outputConfSensorSpeedOracleFreeway_D8,
                                                    freewayFilteredSensorSpeedQT_D8);


            freewayFilteredQuery_D8 = application.CreateQuery("FreewaySensorSpeedQuery_D8", "freeway Current Speed_D8", sensorSpeedQBOracle_D8);


            #endregion

            #region Highway Query D12 (Oracle)

            freewayPassThruQuery_D12 = QueryUtils.GetFreewayPassThroughQuery(application, QueryStrings.FreewayPassThruQueryName+ "_D12", "freeway pass thru query_D12", RIITSFreewayAgency.Caltrans_D12);
            freewayPassThruQuery_D12.Start();
            // Start the pass thru query
            String freewayInputName_D12 = "frewaySensorInput_D12";


            QueryTemplate freewayFilteredSensorSpeedQT_D12 = application.CreateQueryTemplate("FreewayFilteredSpeedsQT_D12", "",
                                                                                         StreamUtils.
                                                                                             GetFilteredSensorSpeedStream
                                                                                             (freewayInputName_D12));

            oracleOutputAdapter_freeway_D12 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Freeway D12",
                                                                                         "Writing freeway events to oracle D12");


            //Oracle Config
            TrafficOutputConfig outputConfSensorSpeedOracleFreeway_D12 = OutputConfigUtils.GetOracleFreewayOutputConf(outputDirectory,
                DBTableStrings.FreewayTableName, RIITSFreewayAgency.Caltrans_D12);

            QueryBinder sensorSpeedQBOracle_D12 = BindQueryToAnotherQuery(freewayPassThruQuery_D12, freewayInputName_D12, oracleOutputAdapter_freeway_D12,
                                                    outputConfSensorSpeedOracleFreeway_D12,
                                                    freewayFilteredSensorSpeedQT_D12);


            freewayFilteredQuery_D12 = application.CreateQuery("FreewaySensorSpeedQuery_D12", "freeway Current Speed_D12", sensorSpeedQBOracle_D12);

            #endregion


            #region Arterial Query LADOT  (Oracle & Azure)

            arterialPassThruQuery = QueryUtils.GetArterialPassThroughQuery(application, QueryStrings.ArterialPassThruQueryName, "arterial pass thru query", RIITSArterialAgency.LADOT);
            arterialPassThruQuery.Start();


            String arterialInputName = "arterialSensorInput";
            QueryTemplate arterialFilteredSensorSpeedQT = application.CreateQueryTemplate("ArterialFilteredSpeedQT", "",
                                                                                    StreamUtils.
                                                                                        GetFilteredSensorSpeedStream
                                                                                        (arterialInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_arterial = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Arterial",
                                                                        "Writing arterial events to oracle");


            TrafficOutputConfig outputConfSensorSpeedOracleArterial = OutputConfigUtils.GetOracleArterialOutputConf(outputDirectory,
                DBTableStrings.ArterialDataTableName, RIITSArterialAgency.LADOT);



            QueryBinder sensorSpeedQBOracle2 = BindQueryToAnotherQuery(arterialPassThruQuery, arterialInputName, oracleOutputAdapter_arterial,
                                                outputConfSensorSpeedOracleArterial,
                                                arterialFilteredSensorSpeedQT);



            arterialFilteredQuery = application.CreateQuery("ArterialSensorSpeedQuery", "Arterial Current Speed", sensorSpeedQBOracle2);

            azureTableOutputAdapter_arterial = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output Arterial",
                                                                                        "Writing arterial events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableArterial =
                OutputConfigUtils.GetSensorSpeedAzureTableArterialOutputConf(AzureTableStrings.ArterialDataTableName, RIITSArterialAgency.LADOT);

            QueryBinder arterialSensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(arterialPassThruQuery, arterialInputName, azureTableOutputAdapter_arterial,
                                                                         outputConfSensorSpeedAzureTableArterial,
                                                                       arterialFilteredSensorSpeedQT);

            azureTableStorageArterialQuery = application.CreateQuery("arterialSensorSpeedAzureTableQuery", "Current Speed Table Storage", arterialSensorSpeedQBAzureTableQB);

            
            TrafficOutputConfig outputConfSensorSpeedXMLArterial = OutputConfigUtils.GetSensorSpeedXMLArterialOutputConf(outputDirectory, RIITSArterialAgency.LADOT);

            XMLOutputAdapter_arterial = application.CreateOutputAdapter<OutputAdapterFactory>("Xml Output3",
                                                                                       "Writing result events to an xml file");
            QueryBinder arterialSensorSpeedQB = BindQueryToAnotherQuery(arterialPassThruQuery, arterialInputName,
                                                                        XMLOutputAdapter_arterial,
                                                                        outputConfSensorSpeedXMLArterial,
                                                                        arterialFilteredSensorSpeedQT);

            XMLArterialQuery = application.CreateQuery("ArterialSensorSpeedXMLQuery", "arterial Current Speed", arterialSensorSpeedQB);
            
            
            #endregion


            #region Arterial Query D7 (Oracle)

            arterialPassThruQuery_D7 = QueryUtils.GetArterialPassThroughQuery(application, QueryStrings.ArterialPassThruQueryName + "_D7", "arterial pass thru query_D7", RIITSFreewayAgency.Caltrans_D7);
            arterialPassThruQuery_D7.Start();


            String arterialInputName_D7 = "arterialSensorInput_D7";
            QueryTemplate arterialFilteredSensorSpeedQT_D7 = application.CreateQueryTemplate("ArterialFilteredSpeedQT_D7", "",
                                                                                    StreamUtils.
                                                                                        GetFilteredSensorSpeedStream
                                                                                        (arterialInputName_D7));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_arterial_D7 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Arterial_D7",
                                                                        "Writing arterial events to oracle_D7");


            TrafficOutputConfig outputConfSensorSpeedOracleArterial_D7 = OutputConfigUtils.GetOracleArterialOutputConf(outputDirectory,
                DBTableStrings.ArterialDataTableName, RIITSFreewayAgency.Caltrans_D7);



            QueryBinder sensorSpeedQBOracle2_D7 = BindQueryToAnotherQuery(arterialPassThruQuery_D7, arterialInputName_D7, oracleOutputAdapter_arterial_D7,
                                                outputConfSensorSpeedOracleArterial_D7,
                                                arterialFilteredSensorSpeedQT_D7);



            arterialFilteredQuery_D7 = application.CreateQuery("ArterialSensorSpeedQuery_D7", "Arterial Current Speed_D7", sensorSpeedQBOracle2_D7);


            #endregion

            #region Arterial Query D8  (Oracle)  Commented
            
            /*
            arterialPassThruQuery_D8 = QueryUtils.GetArterialPassThroughQuery(application, QueryStrings.ArterialPassThruQueryName + "_D8", "arterial pass thru query_D8", RIITSFreewayAgency.Caltrans_D8);
            arterialPassThruQuery_D8.Start();


            String arterialInputName_D8 = "arterialSensorInput_D8";
            QueryTemplate arterialFilteredSensorSpeedQT_D8 = application.CreateQueryTemplate("ArterialFilteredSpeedQT_D8", "",
                                                                                    StreamUtils.
                                                                                        GetFilteredSensorSpeedStream
                                                                                        (arterialInputName_D8));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_arterial_D8 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Arterial_D8",
                                                                        "Writing arterial events to oracle_D8");


            TrafficOutputConfig outputConfSensorSpeedOracleArterial_D8 = OutputConfigUtils.GetOracleArterialOutputConf(outputDirectory,
                DBTableStrings.ArterialDataTableName, RIITSFreewayAgency.Caltrans_D8);



            QueryBinder sensorSpeedQBOracle2_D8 = BindQueryToAnotherQuery(arterialPassThruQuery_D8, arterialInputName_D8, oracleOutputAdapter_arterial_D8,
                                                outputConfSensorSpeedOracleArterial_D8,
                                                arterialFilteredSensorSpeedQT_D8);



            arterialFilteredQuery_D8 = application.CreateQuery("ArterialSensorSpeedQuery_D8", "Arterial Current Speed_D8", sensorSpeedQBOracle2_D8);
            */

            #endregion

            #region Arterial Query D12 (Oracle)  Commented
            /*
            arterialPassThruQuery_D12 = QueryUtils.GetArterialPassThroughQuery(application, QueryStrings.ArterialPassThruQueryName + "_D12", "arterial pass thru query_D12", RIITSFreewayAgency.Caltrans_D12);
            arterialPassThruQuery_D12.Start();


            String arterialInputName_D12 = "arterialSensorInput_D12";
            QueryTemplate arterialFilteredSensorSpeedQT_D12 = application.CreateQueryTemplate("ArterialFilteredSpeedQT_D12", "",
                                                                                    StreamUtils.
                                                                                        GetFilteredSensorSpeedStream
                                                                                        (arterialInputName_D12));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_arterial_D12 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Arterial_D12",
                                                                        "Writing arterial events to oracle_D12");


            TrafficOutputConfig outputConfSensorSpeedOracleArterial_D12 = OutputConfigUtils.GetOracleArterialOutputConf(outputDirectory,
                DBTableStrings.ArterialDataTableName, RIITSFreewayAgency.Caltrans_D12);



            QueryBinder sensorSpeedQBOracle2_D12 = BindQueryToAnotherQuery(arterialPassThruQuery_D12, arterialInputName_D12, oracleOutputAdapter_arterial_D12,
                                                outputConfSensorSpeedOracleArterial_D12,
                                                arterialFilteredSensorSpeedQT_D12);



            arterialFilteredQuery_D12 = application.CreateQuery("ArterialSensorSpeedQuery_D12", "Arterial Current Speed_D12", sensorSpeedQBOracle2_D12);
            */
            
            #endregion


            #region Bus query Metro (Oracle & Azure)
            
            busPassThruQuery_MT = QueryUtils.GetBusPassThroughQuery(application, QueryStrings.BusPassThruQueryName, "bus pass thru query", RIITSBusAgency.MTA_Metro);
            busPassThruQuery_MT.Start();


            String busInputName = "busSensorInput";
            QueryTemplate busFilteredSensorSpeedQT = application.CreateQueryTemplate("BusFilteredSpeedQT", "",
                                                                                    StreamUtils.GetFilteredBusStream
                                                                                        (busInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_bus_MT = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Bus",
                                                                        "Writing Bus events to oracle");


            TrafficOutputConfig outputConfOracleBus = OutputConfigUtils.GetOracleBusOutputConf(outputDirectory, 
                DBTableStrings.MetroBusTableName, RIITSBusAgency.MTA_Metro);



            QueryBinder BusOracle2 = BindQueryToAnotherQuery(busPassThruQuery_MT, busInputName, oracleOutputAdapter_bus_MT,
                                                outputConfOracleBus,
                                                busFilteredSensorSpeedQT);



            busFilteredQuery_MT = application.CreateQuery("BusQuery", "Bus Current Location", BusOracle2);

            azureTableOutputAdapter_bus = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output bus GPS",
                                                                                        "Writing bus GPS events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableBusGPS=
               OutputConfigUtils.GetAzureTableBusGPSOutputConf(AzureTableStrings.BusGPSDataTableName, RIITSBusAgency.MTA_Metro);

            QueryBinder busGPSSensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(busPassThruQuery_MT, busInputName, azureTableOutputAdapter_bus,
                                                                         outputConfSensorSpeedAzureTableBusGPS,
                                                                       busFilteredSensorSpeedQT);

            azureTableStorageBusGPSQuery = application.CreateQuery("busSensorSpeedAzureTableQuery", "Current Speed Table Storage", busGPSSensorSpeedQBAzureTableQB);

            #endregion 

            #region Bus Query FHT (Oracle)

            busPassThruQuery_FHT = QueryUtils.GetBusPassThroughQuery(application, QueryStrings.BusPassThruQueryName + "_FHT", "bus pass thru query_FHT", RIITSBusAgency.FHT);
            busPassThruQuery_FHT.Start();


            String busInputName_FHT = "busSensorInput_FHT";
            QueryTemplate busFilteredSensorSpeedQT_FHT = application.CreateQueryTemplate("BusFilteredSpeedQT_FHT", "",
                                                                                    StreamUtils.GetFilteredBusStream
                                                                                        (busInputName_FHT));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_bus_FHT = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Bus_FHT",
                                                                        "Writing Bus events to oracle_FHT");


            TrafficOutputConfig outputConfOracleBus_FHT = OutputConfigUtils.GetOracleBusOutputConf(outputDirectory,
                DBTableStrings.MetroBusTableName, RIITSBusAgency.FHT);



            QueryBinder BusOracle_FHT = BindQueryToAnotherQuery(busPassThruQuery_FHT, busInputName_FHT, oracleOutputAdapter_bus_FHT,
                                                outputConfOracleBus_FHT,
                                                busFilteredSensorSpeedQT_FHT);



            busFilteredQuery_FHT = application.CreateQuery("BusQuery_FHT", "Bus Current Location_FHT", BusOracle_FHT);


            #endregion

            #region Bus Query LBT (Oracle)
            busPassThruQuery_LBT = QueryUtils.GetBusPassThroughQuery(application, QueryStrings.BusPassThruQueryName + "_LBT", "bus pass thru query_LBT", RIITSBusAgency.LBT);
            busPassThruQuery_LBT.Start();


            String busInputName_LBT = "busSensorInput_LBT";
            QueryTemplate busFilteredSensorSpeedQT_LBT = application.CreateQueryTemplate("BusFilteredSpeedQT_LBT", "",
                                                                                    StreamUtils.GetFilteredBusStream
                                                                                        (busInputName_LBT));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_bus_LBT = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Bus_LBT",
                                                                        "Writing Bus events to oracle_LBT");


            TrafficOutputConfig outputConfOracleBus_LBT = OutputConfigUtils.GetOracleBusOutputConf(outputDirectory,
                DBTableStrings.MetroBusTableName, RIITSBusAgency.LBT);



            QueryBinder BusOracle_LBT = BindQueryToAnotherQuery(busPassThruQuery_LBT, busInputName_LBT, oracleOutputAdapter_bus_LBT,
                                                outputConfOracleBus_LBT,
                                                busFilteredSensorSpeedQT_LBT);



            busFilteredQuery_LBT = application.CreateQuery("BusQuery_LBT", "Bus Current Location_LBT", BusOracle_LBT);


            #endregion

            #region Rail query (Oracle & Azure)

            railPassThruQuery = QueryUtils.GetRailPassThroughQuery(application, QueryStrings.RailPassThruQueryName, "rail pass thru query", RIITSBusAgency.MTA_Metro);
            railPassThruQuery.Start();


            String railInputName = "railSensorInput";
            QueryTemplate railFilteredSensorSpeedQT = application.CreateQueryTemplate("RailFilteredSpeedQT", "",
                                                                                    StreamUtils.GetFilteredRailStream
                                                                                        (railInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_rail = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Rail",
                                                                        "Writing Rail events to oracle");


            TrafficOutputConfig outputConfOracleRail = OutputConfigUtils.GetOracleRailOutputConf(outputDirectory, 
                DBTableStrings.MetroRailTableName, RIITSBusAgency.MTA_Metro);



            QueryBinder RailOracle2 = BindQueryToAnotherQuery(railPassThruQuery, railInputName, oracleOutputAdapter_rail,
                                                outputConfOracleRail,
                                                railFilteredSensorSpeedQT);



            railFilteredQuery = application.CreateQuery("RailQuery", "Rail Current Location", RailOracle2);

            azureTableOutputAdapter_rail = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output rail GPS",
                                                                                        "Writing rail GPS events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableRailGPS =
               OutputConfigUtils.GetAzureTableRailGPSOutputConf(AzureTableStrings.RailGPSDataTableName,RIITSBusAgency.MTA_Metro);

            QueryBinder railGPSSensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(railPassThruQuery, railInputName, azureTableOutputAdapter_rail,
                                                                         outputConfSensorSpeedAzureTableRailGPS,
                                                                       railFilteredSensorSpeedQT);

            azureTableStorageRailGPSQuery = application.CreateQuery("railSensorSpeedAzureTableQuery", "Current rail Speed Table Storage", railGPSSensorSpeedQBAzureTableQB);


            #endregion

            #region Travel Time D7 query  (Oracle & Azure)
            travelPassThruQuery_D7 = QueryUtils.GetTravelTimePassThroughQuery(application, QueryStrings.TravelTimePassThruQueryName + "_D7", "travel pass thru query_D7", RIITSFreewayAgency.Caltrans_D7);
            travelPassThruQuery_D7.Start();


            String travelInputName_D7 = "travelSensorInput_D7";
            QueryTemplate travelFilteredSensorSpeedQT_D7 = application.CreateQueryTemplate("TravelFilteredSpeedQT_D7", "",
                                                                                    StreamUtils.GetFilteredTravelTimeStream
                                                                                        (travelInputName_D7));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_travel_D7 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Travel_D7",
                                                                        "Writing Travel events to oracle_D7");


            TrafficOutputConfig outputConfOracletravel_D7 = OutputConfigUtils.GetOracleTravelOutputConf(outputDirectory,
                DBTableStrings.TravelTimeTableName, RIITSFreewayAgency.Caltrans_D7);



            QueryBinder TravelOracle2_D7 = BindQueryToAnotherQuery(travelPassThruQuery_D7, travelInputName_D7, oracleOutputAdapter_travel_D7,
                                                outputConfOracletravel_D7,
                                                travelFilteredSensorSpeedQT_D7);



            travelFilteredQuery_D7 = application.CreateQuery("TravelQuery_D7", "Travel Current Location_D7", TravelOracle2_D7);


            tracer.WriteLine("Registering bound query");

            azureTableOutputAdapter_travel = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output travel",
                                                                                       "Writing Travel events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableTravel =
               OutputConfigUtils.GetAzureTableTravelOutputConf(AzureTableStrings.TravelTimeTableName, RIITSFreewayAgency.Caltrans_D7);

            QueryBinder travelSensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(travelPassThruQuery_D7, travelInputName_D7, azureTableOutputAdapter_travel,
                                                                         outputConfSensorSpeedAzureTableTravel,
                                                                       travelFilteredSensorSpeedQT_D7);

            azureTableStorageTravelTimeQuery = application.CreateQuery("travelAzureTableQuery", "Current travel Table Storage", travelSensorSpeedQBAzureTableQB);

            #endregion

            #region Travel Time D8 query  (Oracle)  Commented
            /*
            travelPassThruQuery_D8 = QueryUtils.GetTravelTimePassThroughQuery(application, QueryStrings.TravelTimePassThruQueryName + "_D8", "travel pass thru query_D8", RIITSFreewayAgency.Caltrans_D8);
            travelPassThruQuery_D8.Start();


            String travelInputName_D8 = "travelSensorInput_D8";
            QueryTemplate travelFilteredSensorSpeedQT_D8 = application.CreateQueryTemplate("TravelFilteredSpeedQT_D8", "",
                                                                                    StreamUtils.GetFilteredTravelTimeStream
                                                                                        (travelInputName_D8));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_travel_D8 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Travel_D8",
                                                                        "Writing Travel events to oracle_D8");


            TrafficOutputConfig outputConfOracletravel_D8 = OutputConfigUtils.GetOracleTravelOutputConf(outputDirectory,
                DBTableStrings.TravelTimeTableName, RIITSFreewayAgency.Caltrans_D8);



            QueryBinder TravelOracle2_D8 = BindQueryToAnotherQuery(travelPassThruQuery_D8, travelInputName_D8, oracleOutputAdapter_travel_D8,
                                                outputConfOracletravel_D8,
                                                travelFilteredSensorSpeedQT_D8);



            travelFilteredQuery_D8 = application.CreateQuery("TravelQuery_D8", "Travel Current Location_D8", TravelOracle2_D8);
            */
            #endregion

            #region Travel Time D12 query  (Oracle) Commented
            /*
            travelPassThruQuery_D12 = QueryUtils.GetTravelTimePassThroughQuery(application, QueryStrings.TravelTimePassThruQueryName + "_D12", "travel pass thru query_D12", RIITSFreewayAgency.Caltrans_D12);
            travelPassThruQuery_D12.Start();


            String travelInputName_D12 = "travelSensorInput_D12";
            QueryTemplate travelFilteredSensorSpeedQT_D12 = application.CreateQueryTemplate("TravelFilteredSpeedQT_D12", "",
                                                                                    StreamUtils.GetFilteredTravelTimeStream
                                                                                        (travelInputName_D12));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_travel_D12 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Travel_D12",
                                                                        "Writing Travel events to oracle_D12");


            TrafficOutputConfig outputConfOracletravel_D12 = OutputConfigUtils.GetOracleTravelOutputConf(outputDirectory,
                DBTableStrings.TravelTimeTableName, RIITSFreewayAgency.Caltrans_D12);



            QueryBinder TravelOracle2_D12 = BindQueryToAnotherQuery(travelPassThruQuery_D12, travelInputName_D12, oracleOutputAdapter_travel_D12,
                                                outputConfOracletravel_D12,
                                                travelFilteredSensorSpeedQT_D12);



            travelFilteredQuery_D12 = application.CreateQuery("TravelQuery_D12", "Travel Current Location_D12", TravelOracle2_D12);
            */
            #endregion


            #region Ramp Query D7 (Oracle & Azure)
            rampPassThruQuery_D7 = QueryUtils.GetRampPassThroughQuery(application, QueryStrings.RampPassThruQueryName, "ramp pass thru query", RIITSFreewayAgency.Caltrans_D7);
            rampPassThruQuery_D7.Start();


            String rampInputName = "rampSensorInput";
            QueryTemplate rampFilteredSensorSpeedQT = application.CreateQueryTemplate("RampFilteredSpeedQT", "",
                                                                                    StreamUtils.GetFilteredRampStream
                                                                                        (rampInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_ramp_D7 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Ramp",
                                                                        "Writing Ramp events to oracle");


            TrafficOutputConfig outputConfOracleramp = OutputConfigUtils.GetOracleRampOutputConf(outputDirectory,
                DBTableStrings.RampMeterTableName, RIITSFreewayAgency.Caltrans_D7);



            QueryBinder RampOracle2 = BindQueryToAnotherQuery(rampPassThruQuery_D7, rampInputName, oracleOutputAdapter_ramp_D7,
                                                outputConfOracleramp,
                                                rampFilteredSensorSpeedQT);



            rampFilteredQuery_D7 = application.CreateQuery("RampQuery", "Ramp Current Reading", RampOracle2);

            azureTableOutputAdapter_ramp = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output Ramp reading",
                                                                                        "Writing Ramp events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableRamp =
               OutputConfigUtils.GetAzureTableRampOutputConf(AzureTableStrings.RampDataTableName, RIITSFreewayAgency.Caltrans_D7);

            QueryBinder rampSensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(rampPassThruQuery_D7, rampInputName, azureTableOutputAdapter_ramp,
                                                                         outputConfSensorSpeedAzureTableRamp,
                                                                       rampFilteredSensorSpeedQT);

            azureTableStorageRampQuery = application.CreateQuery("rampAzureTableQuery", "Current ramp Table Storage", rampSensorSpeedQBAzureTableQB);

            #endregion

            #region Ramp Query D8 (Oracle)

            rampPassThruQuery_D8 = QueryUtils.GetRampPassThroughQuery(application, QueryStrings.RampPassThruQueryName + "_D8", "ramp pass thru query_D8", RIITSFreewayAgency.Caltrans_D8);
            rampPassThruQuery_D8.Start();


            String rampInputName_D8 = "rampSensorInput_D8";
            QueryTemplate rampFilteredSensorSpeedQT_D8 = application.CreateQueryTemplate("RampFilteredSpeedQT_D8", "",
                                                                                    StreamUtils.GetFilteredRampStream
                                                                                        (rampInputName_D8));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_ramp_D8 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Ramp_D8",
                                                                        "Writing Ramp events to oracle_D8");


            TrafficOutputConfig outputConfOracleramp_D8 = OutputConfigUtils.GetOracleRampOutputConf(outputDirectory,
                DBTableStrings.RampMeterTableName, RIITSFreewayAgency.Caltrans_D8);



            QueryBinder RampOracle2_D8 = BindQueryToAnotherQuery(rampPassThruQuery_D8, rampInputName_D8, oracleOutputAdapter_ramp_D8,
                                                outputConfOracleramp_D8,
                                                rampFilteredSensorSpeedQT_D8);



            rampFilteredQuery_D8 = application.CreateQuery("RampQuery_D8", "Ramp Current Reading_D8", RampOracle2_D8);


            #endregion

            #region Ramp Query D12 (Oracle)

            rampPassThruQuery_D12 = QueryUtils.GetRampPassThroughQuery(application, QueryStrings.RampPassThruQueryName + "_D12", "ramp pass thru query_D12", RIITSFreewayAgency.Caltrans_D12);
            rampPassThruQuery_D12.Start();


            String rampInputName_D12 = "rampSensorInput_D12";
            QueryTemplate rampFilteredSensorSpeedQT_D12 = application.CreateQueryTemplate("RampFilteredSpeedQT_D12", "",
                                                                                    StreamUtils.GetFilteredRampStream
                                                                                        (rampInputName_D12));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_ramp_D12 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Ramp_D12",
                                                                        "Writing Ramp events to oracl_D12");


            TrafficOutputConfig outputConfOracleramp_D12 = OutputConfigUtils.GetOracleRampOutputConf(outputDirectory,
                DBTableStrings.RampMeterTableName, RIITSFreewayAgency.Caltrans_D12);



            QueryBinder RampOracle2_D12 = BindQueryToAnotherQuery(rampPassThruQuery_D12, rampInputName_D12, oracleOutputAdapter_ramp_D12,
                                                outputConfOracleramp_D12,
                                                rampFilteredSensorSpeedQT_D12);



            rampFilteredQuery_D12 = application.CreateQuery("RampQuery_D12", "Ramp Current Reading_D12", RampOracle2_D12);


            #endregion

           #region Caltrans D7 Event Query  (Oracle & Azure)
            EventPassThruQuery_D7 = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.D7EventPassThruQueryName + "_D7", "D7 Event pass thru query", RIITSEventAgency.Caltrans_D7);
            EventPassThruQuery_D7.Start();


            String D7EventInputName = "D7EventInput";
            QueryTemplate D7EventQT = application.CreateQueryTemplate("D7EventQT", "",
                                                                                    StreamUtils.GetEventStream
                                                                                        (D7EventInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_Event_D7 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output D7 Events",
                                                                        "Writing D7 events to oracle");


            TrafficOutputConfig outputConfOracleD7Event = OutputConfigUtils.GetOracleEventOutputConf(DBTableStrings.EventTableName,
                RIITSFreewayAgency.Caltrans_D7);



            QueryBinder D7EventOracle2 = BindQueryToAnotherQuery(EventPassThruQuery_D7, D7EventInputName, oracleOutputAdapter_Event_D7,
                                                outputConfOracleD7Event,
                                                D7EventQT);
            EventOracleQuery_D7 = application.CreateQuery("D7EventQuery", "D7 Event", D7EventOracle2);

            azureTableOutputAdapter_eventD7= application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output D7 Event",
                                                                                        "Writing D7 events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableD7Event =
               OutputConfigUtils.GetAzureTableEventOutputConf(AzureTableStrings.EventTableName, RIITSFreewayAgency.Caltrans_D7);

            QueryBinder D7EventQBAzureTableQB = BindQueryToAnotherQuery(EventPassThruQuery_D7, D7EventInputName, azureTableOutputAdapter_eventD7,
                                                                         outputConfSensorSpeedAzureTableD7Event,
                                                                       D7EventQT);

            azureTableStorageD7EventQuery = application.CreateQuery("D7EventAzureTableQuery", "D7 Event Table Storage", D7EventQBAzureTableQB);

            #endregion

            #region Caltrans D8 Event Query  (Oracle)
            EventPassThruQuery_D8 = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.D7EventPassThruQueryName + "_D8", "D8 Event pass thru query", RIITSFreewayAgency.Caltrans_D8);
            EventPassThruQuery_D8.Start();


            String D8EventInputName = "D8EventInput";
            QueryTemplate D8EventQT = application.CreateQueryTemplate("D8EventQT", "",
                                                                                    StreamUtils.GetEventStream
                                                                                        (D8EventInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_Event_D8 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output D8 Events",
                                                                        "Writing D8 events to oracle");


            TrafficOutputConfig outputConfOracleD8Event = OutputConfigUtils.GetOracleEventOutputConf(DBTableStrings.EventTableName,
                RIITSFreewayAgency.Caltrans_D8);



            QueryBinder D8EventOracle2 = BindQueryToAnotherQuery(EventPassThruQuery_D8, D8EventInputName, oracleOutputAdapter_Event_D8,
                                                outputConfOracleD8Event,
                                                D8EventQT);
            EventOracleQuery_D8 = application.CreateQuery("D8EventQuery", "D8 Event", D8EventOracle2);
            #endregion

            #region Caltrans D12 Event Query  (Oracle)
            /*EventPassThruQuery_D12 = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.D7EventPassThruQueryName + "_D12", "D12 Event pass thru query", RIITSFreewayAgency.Caltrans_D12);
            EventPassThruQuery_D12.Start();


            String D12EventInputName = "D12EventInput";
            QueryTemplate D12EventQT = application.CreateQueryTemplate("D12EventQT", "",
                                                                                    StreamUtils.GetEventStream
                                                                                        (D12EventInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_Event_D12 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output D12 Events",
                                                                        "Writing D12 events to oracle");


            TrafficOutputConfig outputConfOracleD12Event = OutputConfigUtils.GetOracleEventOutputConf(DBTableStrings.EventTableName,
                RIITSFreewayAgency.Caltrans_D12);



            QueryBinder D12EventOracle2 = BindQueryToAnotherQuery(EventPassThruQuery_D12, D12EventInputName, oracleOutputAdapter_Event_D12,
                                                outputConfOracleD12Event,
                                                D12EventQT);
            EventOracleQuery_D12 = application.CreateQuery("D12EventQuery", "D12 Event", D12EventOracle2);
            */
            #endregion

            #region CHP LA Event Query  (Oracle & Azure)
            CHPLAEventPassThruQuery = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.CHPEventPassThruQueryName, "CHPLA Event pass thru query", RIITSEventAgency.CHP_LA);
            CHPLAEventPassThruQuery.Start();


            String CHPLAEventInputName = "CHPLAEventInput";
            QueryTemplate CHPLAEventQT = application.CreateQueryTemplate("CHPLAEventQT", "",
                                                                                    StreamUtils.GetEventStream
                                                                                        (CHPLAEventInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_CHPLAEvent = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output CHPLA Events",
                                                                        "Writing CHPLAevents to oracle");


            TrafficOutputConfig outputConfOracleCHPLAEvent = OutputConfigUtils.GetOracleEventOutputConf(DBTableStrings.EventTableName, RIITSEventAgency.CHP_LA);



            QueryBinder CHPLAEventOracle2 = BindQueryToAnotherQuery(CHPLAEventPassThruQuery, CHPLAEventInputName, oracleOutputAdapter_CHPLAEvent,
                                                outputConfOracleCHPLAEvent,
                                                CHPLAEventQT);



            CHPLAEventOracleQuery = application.CreateQuery("CHPLAEventQuery", "CHPLA Event", CHPLAEventOracle2);

            azureTableOutputAdapter_eventCHPLA = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output CHPLA Event",
                                                                                        "Writing CHPLA events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableCHPLAEvent =
               OutputConfigUtils.GetAzureTableEventOutputConf(AzureTableStrings.EventTableName, RIITSEventAgency.CHP_LA);

            QueryBinder CHPLAEventQBAzureTableQB = BindQueryToAnotherQuery(CHPLAEventPassThruQuery, CHPLAEventInputName, azureTableOutputAdapter_eventCHPLA,
                                                                         outputConfSensorSpeedAzureTableCHPLAEvent,
                                                                       CHPLAEventQT);

            azureTableStorageCHPLAEventQuery = application.CreateQuery("CHPLAEventAzureTableQuery", "CHPLA Event Table Storage", CHPLAEventQBAzureTableQB);

            #endregion

            #region CHP Inland Event Query  (Oracle)

            CHPILEventPassThruQuery = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.CHPEventPassThruQueryName + "_IL", "CHP IL Event pass thru query", RIITSEventAgency.CHP_Inland);
            CHPILEventPassThruQuery.Start();


            String CHPILEventInputName = "CHPILEventInput";
            QueryTemplate CHPILEventQT = application.CreateQueryTemplate("CHPILEventQT", "",
                                                                                    StreamUtils.GetEventStream
                                                                                        (CHPILEventInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_CHPILEvent = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output CHPIL Events",
                                                                        "Writing CHPILevents to oracle");


            TrafficOutputConfig outputConfOracleCHPILEvent = OutputConfigUtils.GetOracleEventOutputConf(DBTableStrings.EventTableName, RIITSEventAgency.CHP_Inland);



            QueryBinder CHPILEventOracle2 = BindQueryToAnotherQuery(CHPILEventPassThruQuery, CHPILEventInputName, oracleOutputAdapter_CHPILEvent,
                                                outputConfOracleCHPILEvent,
                                                CHPILEventQT);



            CHPILEventOracleQuery = application.CreateQuery("CHPILEventQuery", "CHPIL Event", CHPILEventOracle2);

            #endregion

            #region CHP OC Event Query (Oracle)

            CHPOCEventPassThruQuery = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.CHPEventPassThruQueryName + "_OC", "CHP OC Event pass thru query", RIITSEventAgency.CHP_OC);
            CHPOCEventPassThruQuery.Start();


            String CHPOCEventInputName = "CHPOCEventInput";
            QueryTemplate CHPOCEventQT = application.CreateQueryTemplate("CHPOCEventQT", "",
                                                                                    StreamUtils.GetEventStream
                                                                                        (CHPOCEventInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_CHPOCEvent = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output CHPOC Events",
                                                                        "Writing CHPOCevents to oracle");


            TrafficOutputConfig outputConfOracleCHPOCEvent = OutputConfigUtils.GetOracleEventOutputConf(DBTableStrings.EventTableName, RIITSEventAgency.CHP_OC);



            QueryBinder CHPOCEventOracle2 = BindQueryToAnotherQuery(CHPOCEventPassThruQuery, CHPOCEventInputName, oracleOutputAdapter_CHPOCEvent,
                                                outputConfOracleCHPOCEvent,
                                                CHPOCEventQT);



            CHPOCEventOracleQuery = application.CreateQuery("CHPOCEventQuery", "CHPOC Event", CHPOCEventOracle2);
            #endregion

            #region Regional LA Event Query (Oracle & Azure)
            RegionalLAEventPassThruQuery = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.RegionalLAEventPassThruQueryName, "Regional LA Event pass thru query", RIITSEventAgency.Regional_LA);
            RegionalLAEventPassThruQuery.Start();


            String RegionalLAEventInputName = "RegionalLAEventInput";
            QueryTemplate RegionalLAEventQT = application.CreateQueryTemplate("RegionalLAEventQT", "",
                                                                                    StreamUtils.GetEventStream
                                                                                        (RegionalLAEventInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_RegionalLAEvent = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output RegionalLA Events",
                                                                        "Writing RegionalLAevents to oracle");


            TrafficOutputConfig outputConfOracleRegionalLAEvent = OutputConfigUtils.GetOracleEventOutputConf(DBTableStrings.EventTableName
                , RIITSEventAgency.Regional_LA);



            QueryBinder RegionalLAEventOracle2 = BindQueryToAnotherQuery(RegionalLAEventPassThruQuery, RegionalLAEventInputName, oracleOutputAdapter_RegionalLAEvent,
                                                outputConfOracleRegionalLAEvent,
                                                RegionalLAEventQT);



            RegionalLAEventOracleQuery = application.CreateQuery("RegionalLAEventQuery", "RegionalLA Event", RegionalLAEventOracle2);

            azureTableOutputAdapter_eventRegionalLA = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output RegionalLA Event",
                                                                                        "Writing RegionalLA events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableRegionalLAEvent =
               OutputConfigUtils.GetAzureTableEventOutputConf(AzureTableStrings.EventTableName, RIITSEventAgency.Regional_LA);

            QueryBinder RegionalLAEventQBAzureTableQB = BindQueryToAnotherQuery(RegionalLAEventPassThruQuery, RegionalLAEventInputName, azureTableOutputAdapter_eventRegionalLA,
                                                                         outputConfSensorSpeedAzureTableRegionalLAEvent,
                                                                       RegionalLAEventQT);

            azureTableStorageRegionalLAEventQuery = application.CreateQuery("RegionalLAEventAzureTableQuery", "RegionalLA Event Table Storage", RegionalLAEventQBAzureTableQB);

            #endregion            
            
            #region CMS query - Afsin D7  (Oracle & Azure)

            cmsPassThruQuery_D7 = QueryUtils.GetCmsPassThroughQuery(application, QueryStrings.CmsPassThruQueryName, "cms pass thru query", RIITSFreewayAgency.Caltrans_D7);
            cmsPassThruQuery_D7.Start();


            String cmsInputName = "cmsSensorInput";
            QueryTemplate cmsFilteredSensorSpeedQT = application.CreateQueryTemplate("CmsFilteredSpeedQT", "", StreamUtils.GetFilteredCmsStream(cmsInputName));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_cms_D7 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Cms",
                                                                        "Writing Cms events to oracle");


            TrafficOutputConfig outputConfOracleCms = OutputConfigUtils.GetOracleCmsOutputConf(outputDirectory,
                DBTableStrings.CmsTableName, RIITSFreewayAgency.Caltrans_D7);



            QueryBinder cmsOracle2 = BindQueryToAnotherQuery(cmsPassThruQuery_D7, cmsInputName, oracleOutputAdapter_cms_D7,
                                                outputConfOracleCms,
                                                cmsFilteredSensorSpeedQT);



            cmsFilteredQuery_D7 = application.CreateQuery("CmsQuery", "Cms Current Location", cmsOracle2);


            azureTableOutputAdapter_cms = application.CreateOutputAdapter<OutputAdapterFactory>("Azure table Output CMS",
                                                                                        "Writing CMS events to Azure table");

            TrafficOutputConfig outputConfSensorSpeedAzureTableCMS =
                OutputConfigUtils.GetAzureTableCMSOutputConf(AzureTableStrings.CMSTableName,RIITSFreewayAgency.Caltrans_D7);

            QueryBinder CMSQBAzureTableQB = BindQueryToAnotherQuery(cmsPassThruQuery_D7, cmsInputName, azureTableOutputAdapter_cms,
                                                                         outputConfSensorSpeedAzureTableCMS,
                                                                       cmsFilteredSensorSpeedQT);

            azureTableStorageCMSQuery = application.CreateQuery("CmsAzureTableQuery", "Current Speed Table Storage", CMSQBAzureTableQB);



            #endregion

            #region CMS Query D8 (Oracle)

            cmsPassThruQuery_D8 = QueryUtils.GetCmsPassThroughQuery(application, QueryStrings.CmsPassThruQueryName + "_D8", "cms pass thru query_D8", RIITSFreewayAgency.Caltrans_D8);
            cmsPassThruQuery_D8.Start();


            String cmsInputName_D8 = "cmsSensorInput_D8";
            QueryTemplate cmsFilteredSensorSpeedQT_D8 = application.CreateQueryTemplate("CmsFilteredSpeedQT_D8", "", StreamUtils.GetFilteredCmsStream(cmsInputName_D8));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_cms_D8 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Cms_D8",
                                                                        "Writing Cms events to oracle_D8");


            TrafficOutputConfig outputConfOracleCms_D8 = OutputConfigUtils.GetOracleCmsOutputConf(outputDirectory,
                DBTableStrings.CmsTableName, RIITSFreewayAgency.Caltrans_D8);



            QueryBinder cmsOracle2_D8 = BindQueryToAnotherQuery(cmsPassThruQuery_D8, cmsInputName_D8, oracleOutputAdapter_cms_D8,
                                                outputConfOracleCms_D8,
                                                cmsFilteredSensorSpeedQT_D8);



            cmsFilteredQuery_D8 = application.CreateQuery("CmsQuery_D8", "Cms Current Location_D8", cmsOracle2_D8);

            #endregion

            #region CMS Query D12 (Oracle)
            cmsPassThruQuery_D12 = QueryUtils.GetCmsPassThroughQuery(application, QueryStrings.CmsPassThruQueryName + "_D12", "cms pass thru query_D12", RIITSFreewayAgency.Caltrans_D12);
            cmsPassThruQuery_D12.Start();


            String cmsInputName_D12 = "cmsSensorInput_D12";
            QueryTemplate cmsFilteredSensorSpeedQT_D12 = application.CreateQueryTemplate("CmsFilteredSpeedQT_D12", "", StreamUtils.GetFilteredCmsStream(cmsInputName_D12));

            tracer.WriteLine("Registering Adapter Factories");

            oracleOutputAdapter_cms_D12 = application.CreateOutputAdapter<OutputAdapterFactory>("Oracle Output Cms_D12",
                                                                        "Writing Cms events to oracle_D12");


            TrafficOutputConfig outputConfOracleCms_D12 = OutputConfigUtils.GetOracleCmsOutputConf(outputDirectory,
                DBTableStrings.CmsTableName, RIITSFreewayAgency.Caltrans_D12);



            QueryBinder cmsOracle2_D12 = BindQueryToAnotherQuery(cmsPassThruQuery_D12, cmsInputName_D12, oracleOutputAdapter_cms_D12,
                                                outputConfOracleCms_D12,
                                                cmsFilteredSensorSpeedQT_D12);



            cmsFilteredQuery_D12 = application.CreateQuery("CmsQuery_D12", "Cms Current Location_D12", cmsOracle2_D12);

            #endregion

            tracer.WriteLine("Start query");
        }



        private Query GetQueryFilteredStream(Application app, CepStream<TrafficSensorReading> sensorStream, String qName, String qDescription)
        {
            CepStream<TrafficSensorReading> filteredStream = from ev in sensorStream
                                                             where ev.Speed > 0
                                                             select ev;


            return filteredStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                          StreamEventOrder.FullyOrdered);
        }

        private QueryTemplate GetWeightAvgSpeedForSltAreaQT(Application app, CepStream<TrafficSensorReading> selectedArea, String qtName, String qtDescription)
        {
            var AvgSpdForSltAreaEvent = from aa in selectedArea.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
                                                                               WindowInputPolicy.ClipToWindow,
                                                                               HoppingWindowOutputPolicy.ClipToWindowEnd)
                                        select
                                            new
                                            {
                                                rawAverage = aa.Avg(temp => temp.Speed),
                                                historicAverage = aa.HistoricAverage()
                                            };

            CepStream<doubleClass> AvgSpdForSltArea = from ev in AvgSpdForSltAreaEvent
                                                      select
                                                          new doubleClass
                                                          {
                                                              Speed =
                                                                  WeightedAverage.ProcessedAvg(ev.historicAverage,
                                                                                               ev.rawAverage)
                                                          };


            return app.CreateQueryTemplate(qtName, qtDescription, AvgSpdForSltArea);
        }

        private void SetWeight(double weight)
        {
            WeightedAverage.HistoricWeight = weight;
        }

        private void RestartQuery3()
        {
            String predictionQuery = "Prediction Query Template";
            CepStream<doubleClass> averageStreamForPredictionQuery = CepStream<doubleClass>.Create(predictionQuery);

            CepStream<AverageSpeed> predictionResults =
                //from jj in averageStreamForPredictionQuery.AlterEventDuration(e => TimeSpan.FromMinutes(lastKMinutes)).TumblingWindow(TimeSpan.FromMinutes(1), alignment, HoppingWindowOutputPolicy.ClipToWindowEnd)
                //select jj.Predict();
                from jj in
                    averageStreamForPredictionQuery.HoppingWindow(TimeSpan.FromMinutes(lastKMinutes),
                                                                  TimeSpan.FromMinutes(1), alignment,
                                                                  HoppingWindowOutputPolicy.ClipToWindowEnd)
                select jj.Predict();


            QueryTemplate QT;
            application.QueryTemplates.TryGetValue(predictionQuery, out QT);
            try
            {
                if (QT != null)
                {
                    query3.Stop(); //todo: can become more professional if you check the status of the query.
                    query3.Delete();
                    QT.Delete();
                }
            }
            catch (Exception e)
            {
                ExceptionEmailReporter reporter = new ExceptionEmailReporter(e.Message, 872, "ExplicitServer.cs");
                reporter.SendEmailThread();
                ExceptionDatabaseReporter reporter2 = new ExceptionDatabaseReporter(e.Message, 872, "ExplicitServer.cs", "unknown", "SI");
                reporter2.SendDatabaseExceptionThread();

                Console.WriteLine(
                    "error occured but caught!!!!!!!!!!!!!!!!!!!!!! Everything is fine but it is a nice case that we have a query template but the related query is not running. how come?");
            }
            QueryTemplate query3Template = application.CreateQueryTemplate(predictionQuery, "", predictionResults);
        }


        public void SetK(int k)
        {
            if (k > 0 && k != lastKMinutes)
            {
                lastKMinutes = k;
                RestartQuery3();
            }
        }
        public void StartAzureStorageFreeway()
        {
            azureTableStorageFreewayQuery.Start();
        }

        public void StartAzureStorageArterial()
        {
            azureTableStorageArterialQuery.Start();
        }

        public void StartAzureStorageBusGPS()
        {
            azureTableStorageBusGPSQuery.Start();
        }

        public void StartAzureStorageRailGPS()
        {
            azureTableStorageRailGPSQuery.Start();
        }
        public void StartAzureStorageTravelTime()
        {
            azureTableStorageTravelTimeQuery.Start();
        }
        public void StartAzureStorageEventData()
        {
            azureTableStorageD7EventQuery.Start();
            azureTableStorageCHPLAEventQuery.Start();
            azureTableStorageRegionalLAEventQuery.Start();
        }

        public void StartAzureStorageRamp()
        {
            azureTableStorageRampQuery.Start();
        }

        public void StartAzureStorageCMS()
        {
            azureTableStorageCMSQuery.Start();
        }

        public void StartFreewayQuery()
        {
            freewayFilteredQuery_D7.Start();
            freewayFilteredQuery_D8.Start();
            freewayFilteredQuery_D12.Start();
        }

        public void StartArterialQuery()
        {
            //   arterialSelectedAreaQuery.Start();
            arterialFilteredQuery.Start();
            arterialFilteredQuery_D7.Start();
            //arterialFilteredQuery_D8.Start();
            //arterialFilteredQuery_D12.Start();
        }

        public void StartBusQuery()
        {
            busFilteredQuery_MT.Start();
            busFilteredQuery_FHT.Start();
            busFilteredQuery_LBT.Start();
        }

        public void StartRailQuery()
        {
            railFilteredQuery.Start();
        }

        public void StartRampQuery()
        {
            rampFilteredQuery_D7.Start();
            rampFilteredQuery_D8.Start();
            rampFilteredQuery_D12.Start();
        }

        public void StartTravelQuery()
        {
            travelFilteredQuery_D7.Start();
            //travelFilteredQuery_D8.Start();
            //travelFilteredQuery_D12.Start();
        }

        public void StartEventQuery()
        {
            EventOracleQuery_D7.Start();
            EventOracleQuery_D8.Start();
            //EventOracleQuery_D12.Start();

            CHPLAEventOracleQuery.Start();
            CHPILEventOracleQuery.Start();
            CHPOCEventOracleQuery.Start();

            RegionalLAEventOracleQuery.Start();
        }

        public void StartCmsQuery()
        {
            cmsFilteredQuery_D7.Start();
            cmsFilteredQuery_D8.Start();
            cmsFilteredQuery_D12.Start();
        }

        public void StartXMLHighwayQuery()
        {
            XMLFreewayQuery.Start();
        }

        public void StartXMLArterialQuery()
        {
            XMLArterialQuery.Start();
        }

        public void CloseSocket()
        {
        }



        private static QueryBinder BindQuery(InputAdapter inputAdapter, OutputAdapter outputAdapter,
                                             TrafficOutputConfig outputConf, QueryTemplate queryTemplate)
        {
            var queryBinder = new QueryBinder(queryTemplate);


            var sensorInputConf = new SocketInputConfig
            {
                CultureName = "en-US",
                InputFieldOrders =
                    new List<string>
                                                  {
                                                      "SensorId",
                                                      "Occupancy",
                                                      "Speed",
                                                      "Volume",
                                                      "HovSpeed",
                                                      "LinkDataStatus"
                                                  },
                ServerIP = "127.0.0.1",
                ServerPort = BootUp.ServerPort
            };

            queryBinder.BindProducer("sensorInput", inputAdapter, sensorInputConf, EventShape.Point);


            queryBinder.AddConsumer<TrafficOutputConfig>("queryresult", outputAdapter, outputConf, EventShape.Point,
                                                         StreamEventOrder.FullyOrdered);

            return queryBinder;
        }

        private static QueryBinder BindQueryToAnotherQuery(Query q, String inputName, OutputAdapter outputAdapter,
                                                           TrafficOutputConfig outputConf, QueryTemplate queryTemplate)
        {
            var queryBinder = new QueryBinder(queryTemplate);

            queryBinder.BindProducer(inputName, q);
            //use q's published stream as the input "inputName" of the new query.

            queryBinder.AddConsumer<TrafficOutputConfig>("queryresult", outputAdapter, outputConf, EventShape.Point,
                                                         StreamEventOrder.FullyOrdered);

            return queryBinder;
        }

        ////todo: this function can be replaced if you know a little C#!!!
        //private static QueryBinder BindQueryToAnotherQuery(Query q, String inputName, OutputAdapter outputAdapter,
        //                                                   SocketTrafficOutputConfig outputConf,
        //                                                   QueryTemplate queryTemplate)
        //{
        //    var queryBinder = new QueryBinder(queryTemplate);

        //    queryBinder.BindProducer(inputName, q);
        //    //use q's published stream as the input "inputName" of the new query.

        //    queryBinder.AddConsumer<SocketTrafficOutputConfig>("queryresult", outputAdapter, outputConf,
        //                                                       EventShape.Point,
        //                                                       StreamEventOrder.FullyOrdered);

        //    return queryBinder;
        //}

        public void StopAverageQuery()
        {
            freewayAverageQuery.Stop();
        }

        public void StartAverageQuery()
        {
            freewayAverageQuery.Start();
        }


        private void RetrieveDiagnostics(String queryName)
        {
            var URI = new Uri("cep:/Server/Application/" + applicationName + "/Query/" + queryName);
            DiagnosticView view = server.GetDiagnosticView(URI);
            Object queryStatus = view["QueryState"];
        }

        #region Output Configuration Setup Functions

        //private static SocketTrafficOutputConfig GetSocketTrafficOutputConfig(String ip, int port)
        //{
        //    return new SocketTrafficOutputConfig
        //               {
        //                   AdapterStopSignal = "StopAdapter",
        //                   Header = XmlPointOutput.GetDefaultHeader(),
        //                   IPAddress = ip,
        //                   PortNumber = port,
        //                   OtherTopStories = "<item>",
        //                   OutputFieldOrders =
        //                       new List<string> {"<sp>", "<lat>", "<lon>", "<ost>", "<dir>", "<fst>", "<utm>"},
        //                   OutputFileName = string.Empty,
        //                   OutputType = typeof (PredictionSpeedOutputElement).ToString(),
        //                   RootName = "<response>"
        //               };
        //}

        //private static TrafficOutputConfig GetPredictionOutputConf(String outputDirectory)
        //{
        //    return new TrafficOutputConfig
        //               {
        //                   OutputType = typeof (PredictionSpeedOutputElement).ToString(),
        //                   OutputFileName = outputDirectory + "PredictedSpeeds.xml",
        //                   Header = XmlPointOutput.GetDefaultHeader(),
        //                   RootName = "<response>",
        //                   OtherTopStories = "<item>",
        //                   OutputFieldOrders = new List<string> {"<updateDate>", "<updateTime>", "<averageSpeed>"},
        //                   AdapterStopSignal = "StopAdapter",
        //               };
        //}

        //private static TrafficOutputConfig GetAverageSpeedOutputConf(String outputDirectory)
        //{
        //    return new TrafficOutputConfig
        //               {
        //                   OutputType = typeof (AverageSpeedOutputElement).ToString(),
        //                   OutputFileName = outputDirectory + "AverageSpeed.xml",
        //                   Header = XmlPointOutput.GetDefaultHeader(),
        //                   RootName = "<response>",
        //                   OtherTopStories = "<item>",
        //                   OutputFieldOrders = new List<string> {"<updateDate>", "<updateTime>", "<averageSpeed>"},
        //                   AdapterStopSignal = "StopAdapter",
        //               };
        //}

        //private static TrafficOutputConfig GetArterialSensorSpeedOutputConf(string outputDirectory)
        //{
        //    TrafficOutputConfig conf = GetFreewaySensorSpeedOutputConf(outputDirectory);
        //    conf.OutputType = typeof (ArterialSensorSpeedOutputElement).ToString();
        //    conf.OutputFileName = outputDirectory + "ArterialTraffic.xml";
        //    return conf;
        //}




        //private static TrafficOutputConfig GetFreewaySensorSpeedOutputConf(string outputDirectory)
        //{
        //    return new TrafficOutputConfig
        //               {
        //                   OutputType = typeof (FreewaySensorSpeedOutputElement).ToString(),
        //                   OutputFileName = outputDirectory + "HighwayTraffic.xml",
        //                   Header = XmlPointOutput.GetDefaultHeader(),
        //                   RootName = "<mkrs>",
        //                   OtherTopStories = "<mkr>",
        //                   OutputFieldOrders =
        //                       new List<string> {"<sp>", "<lat>", "<lon>", "<ost>", "<dir>", "<fst>", "<utm>"},
        //                   AdapterStopSignal = "StopAdapter",
        //               };
        //}

        #endregion
    }
}