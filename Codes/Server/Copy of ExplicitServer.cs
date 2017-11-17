using System;
using System.Collections.Generic;
using System.Diagnostics;
using BaseOutputAdapter;
using BaseTrafficInputAdapters;
using EventTypes;
using Memory;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using Parsers;
using SocketInputAdapter;
using UDOs;
using XMLOutputAdapter;

namespace SIServers
{
    public class ExplicitServer
    {
        private readonly DateTime alignment;
        private readonly Application application;
        private readonly String applicationName;
        private readonly String outputDirectory;
        private readonly Query passThruQuery;
        private readonly Query passThruQuery2;

        private readonly Query query1;
        private readonly Query query2;
        private readonly Server server;
        private readonly OutputAdapter socketSensorOutputAdapter;
        private readonly TraceListener tracer;
        private readonly OutputAdapter xmlSensorOutputAdapter1;
        private readonly OutputAdapter xmlSensorOutputAdapter2;
        private readonly OutputAdapter cloudSensorOutputAdapter3;
        private int lastKMinutes = -1;
        private Query query3;
        private InputAdapter socketInputAdapter;
        //private Query passThruQuery2;
        public ExplicitServer(String DBAddress, String outputDirectory)
        {
            BootUp utils = BootUp.GetInstance(DBAddress);
            this.outputDirectory = outputDirectory;
            alignment = new DateTime(TimeSpan.FromHours(6).Ticks, DateTimeKind.Utc);
            tracer = new ConsoleTraceListener();
            tracer.WriteLine("Creating CEP Server");
            server = Server.Create("ADMS");
            //Server server = Server.Connect(new System.ServiceModel.EndpointAddress(@"http://localhost/StreamInsight/jalal"));
            // TODO: ask: what happens if I open two VSs and create a server in each of them?

            tracer.WriteLine("Creating CEP Application");
            applicationName = "LA Traffic";
            application = server.CreateApplication(applicationName);

            // Create query logic as a query template
            tracer.WriteLine("Registering LINQ query template");


            //create a query whose output will be fanned-out to other queries (dynamic query composition)

            //CepStream<TrafficSensorReading> sensorStream = CepStream<TrafficSensorReading>.Create("sensorInput", typeof(SocketInputFactory), GetSensorSocketInputConfig(), EventShape.Point);
            CepStream<TrafficSensorReading> sensorStream = CepStream<TrafficSensorReading>.Create("sensorInput",
                                                                                                  typeof (
                                                                                                      WSDLInputFactory),
                                                                                                  //GetArterialInputConfig
                                                                                                  GetFreewayInputConfig
                                                                                                      (),
                                                                                                  EventShape.Point);
            passThruQuery = sensorStream.ToQuery(application, "passthru", "pass thru query", EventShape.Point,
                                                 StreamEventOrder.FullyOrdered);
            passThruQuery.Start();

            //  InsideRectangle.Rect = new Rectangle(Double.Parse("34.065956457628594"), Double.Parse("-118.21821212768556"), Double.Parse("34.04497889936591"), Double.Parse("-118.25099945068361")); 
            passThruQuery2 = GetQueryFilteredStream(passThruQuery.ToStream<TrafficSensorReading>());
            //GetQueryFilteredStream(sensorStream); don't use this! this causes another input adapter generation followed by exceptions in writing to output file.
            passThruQuery2.Start();
            // Start the pass thru query
            String inputName = "sensorInput";
            QueryTemplate sensorSpeedQT = GetSensorSpeedQT(inputName,"AllSpeeds");
            QueryTemplate sensorSpeedCloutQT = GetSensorSpeedQT(inputName,"AllSpeeds Cloud");

            QueryTemplate avgSpeedForSltAreaQT = GetAvgSpeedForSltAreaQT(GetSelectedAreaQT(inputName));

            tracer.WriteLine("Registering Adapter Factories");
            socketInputAdapter = application.CreateInputAdapter<SocketInputFactory>("Socket Input",
                                                                                    "Reading tuples from Socket");
            xmlSensorOutputAdapter1 = application.CreateOutputAdapter<XmlOutputFactory>("Xml Output",
                                                                                        "Writing result events to an xml file");
            xmlSensorOutputAdapter2 = application.CreateOutputAdapter<XmlOutputFactory>("Xml Output2",
                                                                                        "Writing result events to an xml file");
            cloudSensorOutputAdapter3 = application.CreateOutputAdapter<XmlOutputFactory>("Cloud Output",
                                                                                         "Writing result events to cloud");

            //socketSensorOutputAdapter = application.CreateOutputAdapter<SocketOutputFactory>("Raw sensor readings",
              //                                                                               "Writing sensor readings to socket");
            OutputMessageConfig temp = GetOutputXMLMessageConfigFreeway();
            XMLMessageConfig badbakhti = new XMLMessageConfig(temp.OutputFieldOrders);
            TrafficOutputConfig sensorSpeedOutputConf =
                //GetArterialSensorSpeedOutputConf(outputDirectory);
                //GetFreewaySensorSpeedOutputConf(outputDirectory);
                //new TrafficOutputConfig(SourceDataType.Freeway,GetOutputXMLMessageConfigFreeway(),GetOutputFileMediaConfig(outputDirectory));

                //new TrafficOutputConfig(SourceDataType.Freeway, OutputMessageType.XML, OutputMediaType.File,
                //                        outputDirectory, badbakhti.Header, badbakhti.RootName, badbakhti.OtherTopStories,
                //                        badbakhti.OutputFieldOrders);
            #region to file
                new TrafficOutputConfig()
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
            #endregion

            #region to cloud
            SQLMessageConfig khoshbakhti = GetOutputSqlOutputMessageConfigFreeway();
            TrafficOutputConfig sensorSpeedOutputConfToCloud = new TrafficOutputConfig()
                {
                    SourceDataType = SourceDataType.Freeway.ToString(),
                    OutputMessageType = OutputMessageType.DB.ToString(),
                    OutputMediaType = OutputMediaType.Cloud.ToString(),
                    OutputFileName = outputDirectory + "HighwayTraffic.xml",
                    OutputFieldOrders = khoshbakhti.OutputFieldOrders,
                    TableName = "DecemberFreeway"
            };
            #endregion
            //TrafficOutputConfig averageSpeedOutputConf = GetAverageSpeedOutputConf(outputDirectory);
           // SocketTrafficOutputConfig socketSensorSpeedOutputConfig = GetSocketTrafficOutputConfig("128.125.163.181",
             //                                                                                      11111);
            // bind query to event producers and consumers
            //QueryBinder socketSensorSpeedQB = BindQueryToAnotherQuery(passThruQuery, inputName,
            //                                                          socketSensorOutputAdapter,
            //                                                          socketSensorSpeedOutputConfig,
            //                                                          application.CreateQueryTemplate("RawSpeeds", "",
            //                                                                                          sensorStream));
            QueryBinder sensorSpeedQB = BindQueryToAnotherQuery(passThruQuery2, inputName, xmlSensorOutputAdapter1,
                                                                sensorSpeedOutputConf,
                                                                sensorSpeedQT);
            
            //QueryBinder avgSpeedForSltAreaQB = BindQueryToAnotherQuery(passThruQuery2, inputName,
            //                                                           xmlSensorOutputAdapter2, averageSpeedOutputConf,
            //                                                           avgSpeedForSltAreaQT);


            tracer.WriteLine("Registering bound query");
            query1 = application.CreateQuery("SensorSpeedQuery", "Current Speed", sensorSpeedQB);

            QueryBinder sensorSpeedCloudQB = BindQueryToAnotherQuery(passThruQuery2, inputName,
                                                                     cloudSensorOutputAdapter3,
                                                                     sensorSpeedOutputConfToCloud, sensorSpeedCloutQT);
            Query cloudQuery = application.CreateQuery("SensorSpeedQueryCloud", "hichi", sensorSpeedCloudQB);
            cloudQuery.Start();
           // query2 = application.CreateQuery("AverageSpeedQuery", "Average Speed", avgSpeedForSltAreaQB);
            // Query rawValues = application.CreateQuery("RawSensorSpeedQuery", "Raw Speed", socketSensorSpeedQB);
            //rawValues.Start();
            SetK(1);
            tracer.WriteLine("Start query");


            /*      #region deleteme
            CepStream<TrafficSensorReading> tempsensorStream = CepStream<TrafficSensorReading>.Create("tempsensorInput", typeof(WSDLInputFactory), GetSensorBaseInputConfig(), EventShape.Point);
            Query temppassThruQuery = sensorStream.ToQuery(application, "temppassthru", "temppass thru query", EventShape.Point, StreamEventOrder.FullyOrdered);
            temppassThruQuery.Start();

            CepStream<TrafficSensorReading> tempfilteredStream = from ev in tempsensorStream
                                                             where ev.Speed > 0
                                                             select ev;

            Query temppassThruQuery2 = tempfilteredStream.ToQuery(application, "temppassthru2", "temppass thru query2", EventShape.Point, StreamEventOrder.FullyOrdered);
            
            temppassThruQuery2.Start();
            // Start the pass thru query
            String tempinputName = "tempsensorInput";
            CepStream<TrafficSensorReading> tempsensorStreamForSensorSpeedQuery = CepStream<TrafficSensorReading>.Create(tempinputName);


            var tempsensorSpeed = from oneMinReading in tempsensorStreamForSensorSpeedQuery.AlterEventDuration(e => TimeSpan.FromMinutes(1))
                              group oneMinReading by oneMinReading.SensorId
                                  into oneGroup
                                  from eventWindow in oneGroup.SnapshotWindow(WindowInputPolicy.ClipToWindow, SnapshotWindowOutputPolicy.Clip)
                                  select new { speed = eventWindow.Avg(e => e.Speed), SensorId = oneGroup.Key };
            QueryTemplate tempsensorSpeedQT = application.CreateQueryTemplate("tempAllSpeeds", "", tempsensorSpeed);

            
            //CepStream<TrafficSensorReading> selectedArea = from evt in sensorStreamForAvgSpeedQuery
            //                                               where ListOfSensors.Includes(evt.SensorId)
            //                                               select evt;//TODO: instead of using static ListOfSensors, use a udaConfig with application time showing change time.



            CepStream<TrafficSensorReading> tempsensorStreamForAvgSpeedQuery = CepStream<TrafficSensorReading>.Create(tempinputName);
            CepStream<TrafficSensorReading> tempselectedArea =
                from win in
                    tempsensorStreamForAvgSpeedQuery.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
                                                                WindowInputPolicy.ClipToWindow,
                                                                HoppingWindowOutputPolicy.ClipToWindowEnd)
                select win.InsideRect();

            var AvgSpdForSltAreaEvent = from aa in tempselectedArea.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
                                                                               WindowInputPolicy.ClipToWindow,
                                                                               HoppingWindowOutputPolicy.ClipToWindowEnd)
                                        select
                                            new
                                            {
                                                rawAverage = aa.Avg(temp => temp.Speed),
                                                historicAverage = aa.HistoricAverage()
                                            };

            var AvgSpdForSltArea = from ev in AvgSpdForSltAreaEvent
                                   select
                                       new doubleClass { Speed = WeightedAverage.ProcessedAvg(ev.historicAverage, ev.rawAverage) };


            QueryTemplate tempavgSpeedForSltAreaQT = application.CreateQueryTemplate("tempAverageSpeed", "", AvgSpdForSltArea);

            

            tracer.WriteLine("Registering Adapter Factories");
            var tempsocketInputAdapter = application.CreateInputAdapter<SocketInputFactory>("tempSocket Input",
                                                                                                 "tempReading tuples from Socket");
            var tempxmlSensorOutputAdapter1 = application.CreateOutputAdapter<XmlOutputFactory>("tempXml Output",
                                                                                                     "tempWriting result events to an xml file");
            var tempxmlSensorOutputAdapter2 = application.CreateOutputAdapter<XmlOutputFactory>("tempXml Output2",
                                                                                                     "Writing result events to an xml file");

            var tempsocketSensorOutputAdapter = application.CreateOutputAdapter<SocketOutputFactory>("tempRaw sensor readings",
                                                                                                     "tempWriting sensor readings to socket");

            TrafficOutputConfig tempsensorSpeedOutputConf = GetSensorSpeedOutputConf(outputDirectory);
            TrafficOutputConfig tempaverageSpeedOutputConf = GetAverageSpeedOutputConf(outputDirectory);
            SocketTrafficOutputConfig tempsocketSensorSpeedOutputConfig = GetSocketTrafficOutputConfig("128.125.163.181", 11111);
            // bind query to event producers and consumers
            QueryBinder tempsocketSensorSpeedQB = BindQueryToAnotherQuery(temppassThruQuery, tempinputName, tempsocketSensorOutputAdapter, tempsocketSensorSpeedOutputConfig,
                                                 application.CreateQueryTemplate("tempRawSpeeds", "", tempsensorStream));
            QueryBinder tempsensorSpeedQB = BindQueryToAnotherQuery(temppassThruQuery2, tempinputName, tempxmlSensorOutputAdapter1, tempsensorSpeedOutputConf,
                                                 tempsensorSpeedQT);

            QueryBinder tempavgSpeedForSltAreaQB = BindQueryToAnotherQuery(temppassThruQuery2, tempinputName, tempxmlSensorOutputAdapter2, tempaverageSpeedOutputConf,
                                               tempavgSpeedForSltAreaQT);


            tracer.WriteLine("Registering bound query");
            Query tempquery1 = application.CreateQuery("tempSensorSpeedQuery", "Current Speed", tempsensorSpeedQB);
            Query tempquery2 = application.CreateQuery("tempAverageSpeedQuery", "Average Speed", tempavgSpeedForSltAreaQB);
            tempquery1.Start();
            tempquery2.Start();
            #endregion*/
        }

        private Query GetQueryFilteredStream(CepStream<TrafficSensorReading> sensorStream)
        {
            CepStream<TrafficSensorReading> filteredStream = from ev in sensorStream
                                                             where ev.Speed > 0
                                                             select ev;


            return filteredStream.ToQuery(application, "passthru2", "pass thru query2", EventShape.Point,
                                          StreamEventOrder.FullyOrdered);
            //CepStream<TrafficSensorReading> selectedArea =
            //    from win in
            //        filteredStream.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
            //                                                    WindowInputPolicy.ClipToWindow,
            //                                                    HoppingWindowOutputPolicy.ClipToWindowEnd)
            //    select win.InsideRect();
            //return selectedArea.ToQuery(application, "passthru2", "pass thru query2", EventShape.Point, StreamEventOrder.FullyOrdered);
        }

        private QueryTemplate GetSensorSpeedQT(string inputName,string QTName)
        {
            CepStream<TrafficSensorReading> sensorStreamForSensorSpeedQuery =
                CepStream<TrafficSensorReading>.Create(inputName);


            var sensorSpeed =
                from oneMinReading in sensorStreamForSensorSpeedQuery.AlterEventDuration(e => TimeSpan.FromMinutes(1))
                group oneMinReading by oneMinReading.SensorId
                into oneGroup
                from eventWindow in
                    oneGroup.SnapshotWindow(WindowInputPolicy.ClipToWindow, SnapshotWindowOutputPolicy.Clip)
                select new {speed = eventWindow.Avg(e => e.Speed), SensorId = oneGroup.Key};
            return application.CreateQueryTemplate(QTName, "", sensorSpeed);
        }

        private CepStream<TrafficSensorReading> GetSelectedAreaQT(string inputName)
        {
            CepStream<TrafficSensorReading> sensorStreamForAvgSpeedQuery =
                CepStream<TrafficSensorReading>.Create(inputName);
            CepStream<TrafficSensorReading> selectedArea =
                from win in
                    sensorStreamForAvgSpeedQuery.TumblingWindow(TimeSpan.FromMinutes(1), alignment,
                                                                WindowInputPolicy.ClipToWindow,
                                                                HoppingWindowOutputPolicy.ClipToWindowEnd)
                select win.InsideRect();
            return selectedArea;
        }

        private QueryTemplate GetAvgSpeedForSltAreaQT(CepStream<TrafficSensorReading> selectedArea)
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


            return application.CreateQueryTemplate("AverageSpeed", "", AvgSpdForSltArea);
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
            //QueryBinder query3Binder = BindQueryToAnotherQuery2(query2, predictionQuery, passThruQuery,"passThruStream", xmlSensorOutputAdapter1, predictionSpeedsOutputConf,
            //                                               query3Template);
            //QueryBinder query3Binder = BindQueryToAnotherQuery(query2, predictionQuery, xmlSensorOutputAdapter1,
            //                                                   predictionSpeedsOutputConf, query3Template);
            //query3 = application.CreateQuery("PredictionQuery", "Predicted Speeds", query3Binder);
            //query3.Start();
            //RetrieveDiagnostics(predictionQuery);
        }

        //private QueryBinder BindQueryToAnotherQuery2(Query query1, string inputStr1, Query query2, string inputStr2, OutputAdapter outputAdapter, TrafficOutputConfig outputConf, QueryTemplate queryTemplate)
        //{
        //    var queryBinder = new QueryBinder(queryTemplate);

        //    queryBinder.BindProducer(inputStr1, query1);
        //    queryBinder.BindProducer(inputStr2, query2);

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

        public void StartQuery1()
        {
            //var adapterStopSignal = new EventWaitHandle(false, EventResetMode.ManualReset, "StopAdapter");

            query1.Start();
            /*query2.Start();
            
            // Restart and stop the query 
            // Wait 3 minutes then stop and restart query 2
            Console.WriteLine("Sleeping");
            Thread.Sleep(1000 * 1);
            Console.WriteLine("Stopping query2");
            query2.Stop();
            Console.WriteLine("Starting query2");
            adapterStopSignal.Reset();
            query2.Start();
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

        public void CloseSocket()
        {
        }

        private static RIITSInputConfig GetRawStingInputConfig()
        {
            RIITSInputConfig result = GetSensorBaseInputConfig();
            result.InputFieldOrders = new List<string>();
            result.InputFieldOrders.Add("Str");
            result.inputInterpretationType = ReadType.Raw;
            result.inputDataType = RIITSDataTypes.Freeway;
            return result;
        }

        private static RIITSInputConfig GetFreewayInputConfig()
        {
            RIITSInputConfig result = GetSensorBaseInputConfig();
            result.inputDataType = RIITSDataTypes.Freeway;
            result.inputInterpretationType = ReadType.Record;

            return result;
        }

        private static RIITSInputConfig GetArterialInputConfig()
        {
            RIITSInputConfig result = GetSensorBaseInputConfig();
            result.inputDataType = RIITSDataTypes.Arterial;
            result.inputInterpretationType = ReadType.Record;

            return result;
        }

        private static RIITSInputConfig GetSensorBaseInputConfig()
        {
            var sensorInputConf = new RIITSInputConfig
                                      {
                                          Delimiter = ',',
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
                                      };
            return sensorInputConf;
        }

        private static SocketInputConfig GetSensorSocketInputConfig()
        {
            var sensorInputConf = new SocketInputConfig
                                      {
                                          Delimiter = ',',
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
                                          ServerPort = 11111,
                                          inputDataType = RIITSDataTypes.Freeway,
                                          inputInterpretationType = ReadType.Record
                                      };

            return sensorInputConf;
        }

        private static QueryBinder BindQuery(InputAdapter inputAdapter, OutputAdapter outputAdapter,
                                             TrafficOutputConfig outputConf, QueryTemplate queryTemplate)
        {
            var queryBinder = new QueryBinder(queryTemplate);


            var sensorInputConf = new SocketInputConfig
                                      {
                                          Delimiter = ',',
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
            query2.Stop();
        }

        public void StartAverageQuery()
        {
            query2.Start();
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
        private static List<Object> GetFreewayOutputXMLFieldOrders()
        {
            return new List<Object> { "<sp>", "<lat>", "<lon>", "<ost>", "<dir>", "<fst>", "<utm>" };
        }

        private static List<Object> GetFreewayOutputSQLFieldOrders()
        {
            
            return new List<Object> {"sensorID", "speed", "lat", "lng", "readDate", "direction", "onStreet", "fromStreet" };
        }


        private static XMLMessageConfig GetOutputXMLMessageConfigFreeway()
        {
            
            XMLMessageConfig result = new XMLMessageConfig(GetFreewayOutputXMLFieldOrders());

            return result;
        }
        
        private static XMLMessageConfig GetOutputXMLMessageConfigArterial()
        {
            return GetOutputXMLMessageConfigFreeway();
        }
        
        private static SQLMessageConfig GetOutputSqlOutputMessageConfigFreeway()
        {
            return new SQLMessageConfig("DecemberFreeway", GetFreewayOutputSQLFieldOrders());
        }

        private static SQLMessageConfig GetOutputSqlOutputMessageConfigArterial()
        {
            return new SQLMessageConfig("DecemberArterial", GetFreewayOutputSQLFieldOrders());
        }

        

        private static OutputMediaConfig GetOutputFileMediaConfig(string outputFilePath)
        {
            return new FileMediaConfig(outputFilePath);
        }

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