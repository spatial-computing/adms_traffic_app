/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BaseOutputAdapter;
using BaseTrafficInputAdapters;
using EventTypes;
using Memory;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using Parsers;
using SocketInputAdapter;
using UDOs;
using Utils;
using XMLOutputAdapter;

namespace SIServers
{
    public class ExplicitServer
    {
        private readonly DateTime alignment;
        private readonly Application application;
        private readonly String applicationName;
        private readonly String outputDirectory;
        private readonly Query freewayPassThruQuery;
        //private readonly Query filteredFreewayQuery;

        private readonly Query arterialPassThruQuery;
        //private readonly Query filteredArterialQuery;

        private readonly Query freewayFilteredQuery;
        private readonly Query freewayAverageQuery;
        private readonly Query arterialSelectedAreaQuery;
        private readonly Query EventQuery;

        private readonly Server server;
        private readonly OutputAdapter socketSensorOutputAdapter;
        private readonly TraceListener tracer;
        private readonly OutputAdapter xmlSensorOutputAdapter1;
        private readonly OutputAdapter xmlSensorOutputAdapter2;
        private readonly OutputAdapter SQLAzureSensorOutputAdapter3;
        private readonly OutputAdapter AzureTableStorageSensorOutputAdapter4;
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
            server = Server.Create("ADMS");
            
            tracer.WriteLine("Creating CEP Application");
            
            application = server.CreateApplication(QueryStrings.ApplicationName);

            //create a query whose output will be fanned-out to other queries (dynamic query composition)

            freewayPassThruQuery = QueryUtils.GetFreewayPassThroughQuery(application, QueryStrings.FreewayPassThruQueryName, "freeway pass thru query", RIITSFreewayAgency.Caltrans_D7);
            freewayPassThruQuery.Start();
            
           #region Arterial Query
            arterialPassThruQuery = QueryUtils.GetArterialPassThroughQuery(application, QueryStrings.ArterialPassThruQueryName, "arterial pass thru query", RIITSArterialAgency.LADOT);
            arterialPassThruQuery.Start();
            //filteredArterialQuery = GetQueryFilteredStream(application, arterialPassThruQuery.ToStream<TrafficSensorReading>(), QueryStrings.ArterialFilteredQueryName, "filtered arterial pass thrue query");
            //GetQueryFilteredStream(sensorStream); don't use this! this causes another input adapter generation followed by exceptions in writing to output file.
            //filteredArterialQuery.Start();
            String arterialInputName = "arterialSensorInput";
            QueryTemplate arterialFilteredSensorSpeedQT = application.CreateQueryTemplate("ArterialFilteredSpeedQT", "",
                CepStream<TrafficSensorReading>.Create(arterialInputName));
                                                                                          //StreamUtils.
                                                                                         //     GetFilteredSensorSpeedStream
                                                                                         //     (arterialInputName));
            QueryTemplate arterialSelectedAreaSensorSpeedQT = application.CreateQueryTemplate("ArterialSelectedAreaSpeedQT", "",
                                                                                          StreamUtils.GetUnboundFilteredSelectedAreaArterialStream(arterialInputName,alignment));

            TrafficOutputConfig outputConfSensorSpeedXMLArterial = OutputConfigUtils.GetSensorSpeedXMLArterialOutputConf(outputDirectory);

            xmlSensorOutputAdapter2 = application.CreateOutputAdapter<OutputAdapterFactory>("Xml Output2",
                                                                                       "Writing result events to an xml file");

            AzureTableStorageSensorOutputAdapter4 = application.CreateOutputAdapter<OutputAdapterFactory>("Azure Table Output",
                                                                                         "Writing result events to Azure Table");

            TrafficOutputConfig outputConfSensorSpeedSQLAzureArterial = OutputConfigUtils.GetSensorSpeedSQLAzureArterialOutputConf(outputDirectory, DBTableStrings.ArterialDataTableName);
            QueryBinder arterialSensorSpeedQB = BindQueryToAnotherQuery(arterialPassThruQuery, arterialInputName,
                                                                        //xmlSensorOutputAdapter2,
                                                                        AzureTableStorageSensorOutputAdapter4,
                                                                        //outputConfSensorSpeedXMLArterial,
                                                                        outputConfSensorSpeedSQLAzureArterial,
                                                                        arterialFilteredSensorSpeedQT);
                                                                        //arterialSelectedAreaSensorSpeedQT);

            arterialSelectedAreaQuery = application.CreateQuery("ArterialSelectedAreaSensorSpeedQuery", "arterial Current Speed", arterialSensorSpeedQB);
            #endregion*/

         /* #region Event Query
            EventQuery = QueryUtils.GetEventPassThroughQuery(application, QueryStrings.EventPassThruQueryName, "event pass thru query", RIITSEventAgency.Caltrans_D7);
            EventQuery.Start();
            #endregion*/
            //  filteredFreewayQuery = GetQueryFilteredStream(application, freewayPassThruQuery.ToStream<TrafficSensorReading>(), QueryStrings.FreewayFilteredQueryName,"filtered freeway pass thrue query");
            //GetQueryFilteredStream(sensorStream); don't use this! this causes another input adapter generation followed by exceptions in writing to output file.
            //filteredFreewayQuery.Start();

           

            // Start the pass thru query
            String freewayInputName = "frewaySensorInput";
            

            QueryTemplate freewayFilteredSensorSpeedQT = application.CreateQueryTemplate("FreewayFilteredSpeedsQT", "",
                                                                                         CepStream<TrafficSensorReading>
                                                                                             .Create(freewayInputName));
                                                                                         //StreamUtils.
                                                                                             //GetFilteredSensorSpeedStream
                                                                                             //(freewayInputName));
                                                                                             
                                                                                             //todo: delete the following line and uncomment the above line. just for Barak.
                                                                                             //GetUnboundFilteredSelectedAreaFreewayStream
                                                                                             //(freewayInputName,alignment));
                                                                                             
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
            
            //socketSensorOutputAdapter = application.CreateOutputAdapter<SocketOutputFactory>("Raw sensor readings",
              //                                                                               "Writing sensor readings to socket");
            //OutputMessageConfig temp = GetOutputXMLMessageConfigFreeway();

            TrafficOutputConfig outputConfSensorSpeedXMLFreeway = OutputConfigUtils.GetSensorSpeedXMLFreewayOutputConf(outputDirectory); 

            TrafficOutputConfig outputConfSensorSpeedSQLAzureFreeway = OutputConfigUtils.GetSensorSpeedSQLAzureFreewayOutputConf(outputDirectory, DBTableStrings.FreewayTableName);

            TrafficOutputConfig outputConfSensorSpeedAzureTableFreeway =
                OutputConfigUtils.GetSensorSpeedAzureTableFreewayOutputConf(AzureTableStrings.FreewayDataTableName);

            TrafficOutputConfig outputConfSensorSpeedAzureTableArterial =
                OutputConfigUtils.GetSensorSpeedAzureTableArterialOutputConf(AzureTableStrings.ArterialDataTableName);
            
            //TrafficOutputConfig averageSpeedOutputConf = GetAverageSpeedOutputConf(outputDirectory);
            
            //QueryBinder freewayFilteredSensorSpeedQB = BindQueryToAnotherQuery(freewayPassThruQuery, freewayInputName, xmlSensorOutputAdapter1,
              //                                                  outputConfSensorSpeedXMLFreeway,
                //                                                freewayFilteredSensorSpeedQT);

            //QueryBinder sensorSpeedQBCloud = BindQueryToAnotherQuery(freewayPassThruQuery, freewayInputName, SQLAzureSensorOutputAdapter3,
              //                                                  outputConfSensorSpeedSQLAzureFreeway,
                //                                                freewayFilteredSensorSpeedQT);

            QueryBinder freewaySensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(freewayPassThruQuery, freewayInputName, AzureTableStorageSensorOutputAdapter4,
                                                              outputConfSensorSpeedAzureTableFreeway,
                                                            freewayFilteredSensorSpeedQT);


            QueryBinder arterialSensorSpeedQBAzureTableQB = BindQueryToAnotherQuery(arterialPassThruQuery, arterialInputName, AzureTableStorageSensorOutputAdapter4,
                                                              outputConfSensorSpeedAzureTableArterial,
                                                            arterialFilteredSensorSpeedQT);
  
            //QueryBinder avgSpeedForSltAreaQB = BindQueryToAnotherQuery(passThruQuery2, inputName,
            //                                                           xmlSensorOutputAdapter2, averageSpeedOutputConf,
            //                                                           avgSpeedForSltAreaQT);


            tracer.WriteLine("Registering bound query");
            //freewayFilteredQuery = application.CreateQuery("FreewaySensorSpeedQuery", "freeway Current Speed", freewayFilteredSensorSpeedQB);
            //Query jjCloud = application.CreateQuery("SensorSpeedCloudQuery", "Current Speed LocalDB", sensorSpeedQBCloud);
            //jjCloud.Start();

            Query azureTableStorageFreewayQ= application.CreateQuery("freewaySensorSpeedAzureTableQuery", "Current Speed Table Storage", freewaySensorSpeedQBAzureTableQB);
            azureTableStorageFreewayQ.Start();

            Query azureTableStorageArterialQ = application.CreateQuery("ArterialSensorSpeedAzureTableQuery", "Current Speed Table Storage", arterialSensorSpeedQBAzureTableQB);
            azureTableStorageArterialQ.Start();
            
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
                Console.WriteLine(
                    "error occured but caught!!!!!!!!!!!!!!!!!!!!!! Everything is fine but it is a nice case that we have a query template but the related query is not running. how come?");
            }
            QueryTemplate query3Template = application.CreateQueryTemplate(predictionQuery, "", predictionResults);
        //    TrafficOutputConfig predictionSpeedsOutputConf = GetPredictionOutputConf(outputDirectory);
            //QueryBinder query3Binder = BindQueryToAnotherQuery2(freewayAverageQuery, predictionQuery, passThruQuery,"passThruStream", xmlSensorOutputAdapter1, predictionSpeedsOutputConf,
            //                                               query3Template);
            //QueryBinder query3Binder = BindQueryToAnotherQuery(freewayAverageQuery, predictionQuery, xmlSensorOutputAdapter1,
            //                                                   predictionSpeedsOutputConf, query3Template);
            //query3 = application.CreateQuery("PredictionQuery", "Predicted Speeds", query3Binder);
            //query3.Start();
            //RetrieveDiagnostics(predictionQuery);
        }

        //private QueryBinder BindQueryToAnotherQuery2(Query freewayFilteredQuery, string inputStr1, Query freewayAverageQuery, string inputStr2, OutputAdapter outputAdapter, TrafficOutputConfig outputConf, QueryTemplate queryTemplate)
        //{
        //    var queryBinder = new QueryBinder(queryTemplate);

        //    queryBinder.BindProducer(inputStr1, freewayFilteredQuery);
        //    queryBinder.BindProducer(inputStr2, freewayAverageQuery);

        //    queryBinder.AddConsumer<TrafficOutputConfig>("queryresult", outputAdapter, outputConf, EventShape.Point,
        //                                             StreamEventOrder.FullyOrdered);

        //    return queryBinder;
        //}


        public void SetK(int k)
        {
            if (k > 0 && k != lastKMinutes)
            {
                lastKMinutes = k;
                RestartQuery3();
            }
        }

        public void StartFreewayQuery()
        {
            //var adapterStopSignal = new EventWaitHandle(false, EventResetMode.ManualReset, "StopAdapter");

            //freewayFilteredQuery.Start();
            /*freewayAverageQuery.Start();
            
            // Restart and stop the query 
            // Wait 3 minutes then stop and restart query 2
            Console.WriteLine("Sleeping");
            Thread.Sleep(1000 * 1);
            Console.WriteLine("Stopping freewayAverageQuery");
            freewayAverageQuery.Stop();
            Console.WriteLine("Starting freewayAverageQuery");
            adapterStopSignal.Reset();
            freewayAverageQuery.Start();
            //////////
*/
            //adapterStopSignal.WaitOne();
            //tracer.WriteLine(string.Empty);
            //RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/EventManager")), tracer);
            //RetrieveDiagnostics(server.GetDiagnosticView(new Uri("cep:/Server/PlanManager")), tracer);
            //RetrieveDiagnostics(
            //    server.GetDiagnosticView(new Uri("cep:/Server/Application/ObjectModelSample/Query/TrafficSensorQuery")),
            //    tracer);
        }

        public void StartArterialQuery()
        {
         //   arterialSelectedAreaQuery.Start();
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