﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Add the Oracle Output Part (mediaType, messageType)
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */

/**
 * Updated by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Purpose: Add the Azure table storage part
 * Date: 06/09/2011
 */

using System;
using System.Collections.Generic;
using BaseOutputAdapter;
using EventTypes;
using MediaWriters;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Adapters;
using MyMemory;
using OutputTypes;

namespace XMLOutputAdapter
{
    public class TrafficOutputConfig
    {

        public string SourceDataType { get; set; }
        //public IOutputType OutputType { get; set; }
        public string OutputMessageType { get; set; }
        public string OutputMediaType { get; set; }
        public string TableName { get; set; }
        //public OutputMediaConfig OutputMediaConfig { get; set; }
        public string OutputFileName { get; set; }
        public string Agency { get; set; }

        //public OutputMessageConfig OutputMessageConfig { get; set; }
        public string Header { get; set; }
        public string RootName { get; set; }
        public string OtherTopStories { get; set; }
        public List<Object> OutputFieldOrders { get; set; }

        //public TrafficOutputConfig(SourceDataType sourceType, OutputMessageConfig messageConfig, OutputMediaConfig mediaConfig)
        //{
        //    SourceDataType = sourceType;
        //    OutputMessageConfig = messageConfig;
        //    OutputMediaConfig = mediaConfig;
        //}

        //public TrafficOutputConfig(SourceDataType sourceType, OutputMessageType messageType, OutputMediaType mediaType, string fileName, string header, string rootNam, string otherStories, List<string> fieldOrders)
        //{
        //    SourceDataType = sourceType.ToString();
        //    OutputMessageType = messageType.ToString();
        //    OutputMediaType = mediaType.ToString();
        //    OutputFileName = fileName;
        //    Header = header;
        //    RootName = rootNam;
        //    OtherTopStories = otherStories;
        //    OutputFieldOrders = fieldOrders;
        //}
    }






    public class OutputAdapterFactory : IOutputAdapterFactory<TrafficOutputConfig>
    {
        #region IOutputAdapterFactory<TrafficOutputConfig> Members

        public OutputAdapterBase Create(TrafficOutputConfig configInfo, EventShape eventShape, CepEventType cepEventType)
        {
            OutputAdapterBase adapter = default(OutputAdapterBase);
            MediaWriter writer = default(MediaWriter);
            //String outputMediaConfigString = configInfo.OutputMediaConfig.GetType().ToString();
            //String outputMessageConfigString = configInfo.OutputMessageConfig.GetType().ToString();
            string mediaType = configInfo.OutputMediaType;
            string messageType = configInfo.OutputMessageType;


            //if ( outputMediaConfigString== typeof(FileMediaConfig).ToString())
            if (mediaType == OutputMediaType.File.ToString())
            {
                //writer = new FileMediaWriter(((FileMediaConfig)(configInfo.OutputMediaConfig)).OutputFileName);
                writer = new FileMediaWriter(((new FileMediaConfig(configInfo.OutputFileName))).OutputFileName);
            }
            else if (mediaType == OutputMediaType.LocalDB.ToString())
            {
                writer = new SQLMediaWriter();
            }
            else if (mediaType == OutputMediaType.AzureTable.ToString())
                writer = null; // will be defined later in this file based on the type of object we want to write to Azure Table
            //throw new NotImplementedException(configInfo.OutputMediaConfig.GetType().ToString());
            else if (mediaType == OutputMediaType.Oracle.ToString())
            {
                //get user name and password, because it stored in different database
                string user;
                string pwd;
                if (configInfo.SourceDataType == SourceDataType.Freeway.ToString())
                {
                    user = "HIGHWAY";
                    pwd = "hphe106";
                }
                else if (configInfo.SourceDataType == SourceDataType.Arterial.ToString())
                {
                    user = "ARTERIAL";
                    pwd = "aphe106";
                }
                else if (configInfo.SourceDataType == SourceDataType.TravelTime.ToString()
                    || configInfo.SourceDataType == SourceDataType.Ramp.ToString()
                    || configInfo.SourceDataType == SourceDataType.Bus.ToString()
                    || configInfo.SourceDataType == SourceDataType.Rail.ToString())
                {
                    user = "TRANSIT";
                    pwd = "tphe106";
                }
                else if (configInfo.SourceDataType == SourceDataType.Event.ToString())
                {
                    user = "EVENT";
                    pwd = "ephe106";
                }
                else if (configInfo.SourceDataType == SourceDataType.Cms.ToString())
                {
                    user = "EVENT";
                    pwd = "ephe106";
                }
                else
                {
                    user = "";
                    pwd = "";
                }
                writer = new OracleMediaWriter(user, pwd);

            }
            else
                throw new NotImplementedException(configInfo.OutputMediaType.ToString());
            //if ( outputMessageConfigString == typeof(XMLMessageConfig).ToString())
            if (messageType == OutputMessageType.XML.ToString())
            {
                //Type outputType= configInfo.OutputType.GetType();

                //        Type generic = typeof(XMLOutputMessage<> );
                //        Type combined = generic.MakeGenericType(outputType);
                //        var xmlOutputMessage= Activator.CreateInstance(combined,new object[]{configInfo,new LookupFreewayXML()});

                //Type adapterType = typeof (OutputAdapterBase<>);
                //Type combinedAdapter = adapterType.MakeGenericType(outputType);
                //var finalAdapter= Activator.CreateInstance(combinedAdapter,new object[]{configInfo,cepEventType,writer,xmlOutputMessage});
                if (configInfo.SourceDataType == SourceDataType.Freeway.ToString())
                    adapter = new OutputAdapterBase<FreewaySensorSpeedOutputElement>(configInfo, cepEventType, writer,
                                                                                     new XMLOutputMessage
                                                                                         <FreewaySensorSpeedOutputElement>
                                                                                         (new XMLMessageConfig(configInfo.OutputFieldOrders,configInfo.Agency),
                                                                                           new LookupFreewayXML()));
                else
                    if (configInfo.SourceDataType == SourceDataType.Arterial.ToString())
                        adapter = new OutputAdapterBase<ArterialSensorSpeedOutputElement>(configInfo, cepEventType, writer,
                                                                                     new XMLOutputMessage
                                                                                         <
                                                                                         ArterialSensorSpeedOutputElement
                            //     >((XMLMessageConfig)configInfo.OutputMessageConfig, 
                                                                                         >(new XMLMessageConfig(configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                           new LookupArterialXML()));
                    else
                        throw new NotImplementedException();



            }
            else
                if (messageType == OutputMessageType.DB.ToString())
                {
                    if (configInfo.SourceDataType == SourceDataType.Freeway.ToString())
                        adapter = new OutputAdapterBase<FreewaySensorSpeedOutputElement>(configInfo, cepEventType, writer,
                                                                                         new SQLOutputMessage
                                                                                             <FreewaySensorSpeedOutputElement>
                                                                                             (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                             new LookUpFreewayDB()));
                    else
                        if (configInfo.SourceDataType == SourceDataType.Arterial.ToString())
                            adapter = new OutputAdapterBase<ArterialSensorSpeedOutputElement>(configInfo, cepEventType, writer,
                                                                                                     new SQLOutputMessage
                                                                                             <ArterialSensorSpeedOutputElement>
                                                                                             (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                               new LookupArterialDB()));
                        else
                            throw new NotImplementedException();

                }

            // throw new NotImplementedException(configInfo.OutputMediaConfig.GetType().ToString());
                else if (messageType == OutputMessageType.TableStorage.ToString())
                {
                    if (configInfo.SourceDataType == SourceDataType.Freeway.ToString())
                        adapter = new OutputAdapterBase<FreewaySensorSpeedOutputElement>(configInfo, cepEventType, new AzureTableStorageMediaWriter<SensorEntityTuple>(configInfo.TableName),
                                                                                         new AzureTableStorageOutputMessage
                                                                                             <FreewaySensorSpeedOutputElement>(new LookUpFreewayAzureTable(), 
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                    else
                        if (configInfo.SourceDataType == SourceDataType.Arterial.ToString())
                            adapter = new OutputAdapterBase<ArterialSensorSpeedOutputElement>(configInfo, cepEventType, new AzureTableStorageMediaWriter<SensorEntityTuple>(configInfo.TableName),
                                                                                         new AzureTableStorageOutputMessage
                                                                                             <ArterialSensorSpeedOutputElement>(new LookUpArterialAzureTable(),
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                        else
                            if (configInfo.SourceDataType == SourceDataType.Bus.ToString())
                                adapter = new OutputAdapterBase<BusGPSOutputElement>(configInfo, cepEventType, new AzureTableStorageMediaWriter<BusGPSEntityTuple>(configInfo.TableName),
                                                                                             new AzureTableStorageOutputMessage
                                                                                                 <BusGPSOutputElement>(new LookUpBusGPSAzureTable(),
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                            else
                                if (configInfo.SourceDataType == SourceDataType.Rail.ToString())
                                    adapter = new OutputAdapterBase<RailGPSOutputElement>(configInfo, cepEventType, new AzureTableStorageMediaWriter<RailGPSEntityTuple>(configInfo.TableName),
                                                                                                 new AzureTableStorageOutputMessage
                                                                                                     <RailGPSOutputElement>(new LookUpRailGPSAzureTable(),
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                                else
                                    if (configInfo.SourceDataType == SourceDataType.Ramp.ToString())
                                        adapter = new OutputAdapterBase<RampOutputElement>(configInfo, cepEventType, new AzureTableStorageMediaWriter<RampEntityTuple>(configInfo.TableName),
                                                                                                     new AzureTableStorageOutputMessage
                                                                                                         <RampOutputElement>(new LookUpRampMeterAzureTable(),
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                                    else
                                        if (configInfo.SourceDataType == SourceDataType.TravelTime.ToString())
                                            adapter = new OutputAdapterBase<TravelLinksOutputElement>(configInfo, cepEventType, new AzureTableStorageMediaWriter<TravelTimeEntityTuple>(configInfo.TableName),
                                                                                                         new AzureTableStorageOutputMessage
                                                                                                             <TravelLinksOutputElement>(new LookUpTravelLinkAzureTable(),
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                                        else
                                            if (configInfo.SourceDataType == SourceDataType.Event.ToString())
                                                adapter = new OutputAdapterBase<EventEntityTuple>(configInfo, cepEventType, new AzureTableStorageMediaWriter<EventEntityTuple>(configInfo.TableName),
                                                                                                             new AzureTableStorageOutputMessage
                                                                                                                 <EventEntityTuple>(new LookUpEventAzureTable(),
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                                            else
                                                if (configInfo.SourceDataType == SourceDataType.Cms.ToString())
                                                    adapter = new OutputAdapterBase<CmsOutputElement>(configInfo, cepEventType, new AzureTableStorageMediaWriter<CmsEntityTuple>(configInfo.TableName),
                                                                                                                 new AzureTableStorageOutputMessage
                                                                                                                     <CmsOutputElement>(new LookUpCmsAzureTable(),
                                                                                             new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency)));
                                                else
                                                    throw new NotImplementedException();
                }
                else if ((messageType == OutputMessageType.Oracle.ToString()))
                {

                    if (configInfo.SourceDataType == SourceDataType.Freeway.ToString())
                        adapter = new OutputAdapterBase<FreewaySensorSpeedOutputElement>(configInfo, cepEventType, writer,
                                                                                         new OracleOutputMessage
                                                                                         <FreewaySensorSpeedOutputElement>
                                                                                         (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                               new LookUpFreewayDB()));
                    else
                        if (configInfo.SourceDataType == SourceDataType.Arterial.ToString())
                            adapter = new OutputAdapterBase<ArterialSensorSpeedOutputElement>(configInfo, cepEventType, writer,
                                                                                             new OracleOutputMessage
                                                                                             <ArterialSensorSpeedOutputElement>
                                                                                              (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                               new LookupArterialDB()));
                        else
                            if (configInfo.SourceDataType == SourceDataType.Bus.ToString())
                                adapter = new OutputAdapterBase<BusGPSOutputElement>(configInfo, cepEventType, writer,
                                                                                                  new OracleOutputMessage
                                                                                                 <BusGPSOutputElement>
                                                                                                 (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                                   new LookupBusGPSDB()));
                            else
                                if (configInfo.SourceDataType == SourceDataType.Rail.ToString())
                                    adapter = new OutputAdapterBase<RailGPSOutputElement>(configInfo, cepEventType, writer,
                                                                                                      new OracleOutputMessage
                                                                                                     <RailGPSOutputElement>
                                                                                                     (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                                       new LookupRailGPSDB()));
                                else
                                    if (configInfo.SourceDataType == SourceDataType.Ramp.ToString())
                                        adapter = new OutputAdapterBase<RampOutputElement>(configInfo, cepEventType, writer,
                                                                                                          new OracleOutputMessage
                                                                                                         <RampOutputElement>
                                                                                                         (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                                           new LookupRampMeterDB()));
                                    else
                                        if (configInfo.SourceDataType == SourceDataType.TravelTime.ToString())
                                            adapter = new OutputAdapterBase<TravelLinksOutputElement>(configInfo, cepEventType, writer,
                                                                                                              new OracleOutputMessage
                                                                                                             <TravelLinksOutputElement>
                                                                                                             (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                                               new LookupTravelLinkDB()));
                                        else
                                            if (configInfo.SourceDataType == SourceDataType.Event.ToString())
                                                adapter = new OutputAdapterBase<EventEntityTuple>(configInfo, cepEventType, writer,
                                                                                                                  new OracleOutputMessage
                                                                                                                 <EventEntityTuple>
                                                                                                                 (new SQLMessageConfig(configInfo.TableName,
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                                                   new LookupEventDB()));
                                            else
                                                if (configInfo.SourceDataType == SourceDataType.Cms.ToString())
                                                    adapter = new OutputAdapterBase<CmsOutputElement>(configInfo, cepEventType, writer,
                                                                                                                      new OracleOutputMessage
                                                                                                                     <CmsOutputElement>
                                                                                                                     (new SQLMessageConfig(configInfo.TableName, 
                                                                                                                         configInfo.OutputFieldOrders, configInfo.Agency),
                                                                                                                       new LookupCmsDb()));
                                                else
                                                    throw new NotImplementedException();
                }
            //switch (eventShape)
            //{
            //    //case EventShape.Point:
            //    //    if (configInfo.OutputType.Equals(typeof (ArterialSensorSpeedOutputElement).ToString()))
            //    //        adapter = new XmlPointArterialSensorSpeedOutput(configInfo, cepEventType);
            //    //    else if (configInfo.OutputType.Equals(typeof (FreewaySensorSpeedOutputElement).ToString()))
            //    //        adapter = new XmlPointFreewaySensorSpeedOutput(configInfo, cepEventType);
            //    //    else if (configInfo.OutputType.Equals(typeof (AverageSpeedOutputElement).ToString()))
            //    //        adapter = new XmlPointAverageSpeedOutput(configInfo, cepEventType);
            //    //    else if (configInfo.OutputType.Equals(typeof (PredictionSpeedOutputElement).ToString()))
            //    //        adapter = new XmlPointPredictionSpeedOutput(configInfo, cepEventType);

            //    //    break;
            //    case EventShape.Interval:

            //        throw new NotImplementedException();
            //        break;
            //    case EventShape.Edge:
            //        throw new NotImplementedException();
            //        break;
            //    default:
            //        break;
            //}

            return adapter;
        }

        public void Dispose()
        {
        }

        #endregion
    }
}