/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei (Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Add the GetPassThruQuery for Bus, Rail, RMS, TravelTime
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventTypes;
using Microsoft.ComplexEventProcessing;
using Microsoft.ComplexEventProcessing.Linq;
using SocketInputAdapter;

namespace SIServers
{
    internal class QueryUtils
    {
        public static Query GetFreewayPassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<TrafficSensorReading> sensorStream = CepStream<TrafficSensorReading>.Create("freewaySensorInput" + "GetFreewayPassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetFreewayInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                                 StreamEventOrder.FullyOrdered);
        }

        public static Query GetArterialPassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<TrafficSensorReading> sensorStream = CepStream<TrafficSensorReading>.Create("arterialSensorInput" + "GetArterialPassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetArterialInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                                 StreamEventOrder.FullyOrdered);
        }

        public static Query GetEventPassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<EventReading> sensorStream = CepStream<EventReading>.Create("EventInputFrom"+ agency + "GetEventPassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetEventInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                                 StreamEventOrder.FullyOrdered);
        }
        public static Query GetBusPassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<TrafficBusGPSReading> sensorStream = CepStream<TrafficBusGPSReading>.Create("busSensorInput" + "GetBusPassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetBusInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                     StreamEventOrder.FullyOrdered);
        }

        public static Query GetRailPassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<TrafficRailGPSReading> sensorStream = CepStream<TrafficRailGPSReading>.Create("RailSensorInput" + "GetRailPassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetRailInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                     StreamEventOrder.FullyOrdered);
        }

        public static Query GetRampPassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<TrafficRampReading> sensorStream = CepStream<TrafficRampReading>.Create("RampSensorInput" + "GetRailPassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetRampInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                     StreamEventOrder.FullyOrdered);
        }

        public static Query GetTravelTimePassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<TrafficTravelTimeReading> sensorStream = CepStream<TrafficTravelTimeReading>.Create("TravelTimeInput" + "GetTravelTimePassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetTravelTimesInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                     StreamEventOrder.FullyOrdered);
        }

        public static Query GetCmsPassThroughQuery(Application app, String qName, String qDescription, String agency)
        {
            CepStream<TrafficCmsReading> sensorStream = CepStream<TrafficCmsReading>.Create("cmsSensorInput" + "GetCmsPassThroughQuery",
                                                                                                  typeof(WSDLInputFactory),
                                                                                                  InputConfigUtils.GetCmsInputConfig(agency),
                                                                                                  EventShape.Point);
            return sensorStream.ToQuery(app, qName, qDescription, EventShape.Point,
                                     StreamEventOrder.FullyOrdered);
        }


    }
}
