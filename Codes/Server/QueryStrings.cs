﻿/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei (Penny) Pan (beipan@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Add the ThruQueryName for Bus, Rail, RMS, TravelTime
 * Date: 04/18/2011
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIServers
{
    internal class QueryStrings
    {
        public static String ApplicationName = "LA Traffic Application";
        public static String FreewayPassThruQueryName = "PassthroughFreewayQueryName";
        public static String FreewayFilteredQueryName = "FilteredFreewayQueryName";

        public static String ArterialPassThruQueryName = "PassthroughArterialQueryName";
        public static String ArterialFilteredQueryName = "FilteredArterialQueryName";


        public static String AverageSpeedFreewayQTName = "AverageSpeedFreewayQueryTemplate";
        public static String AverageSpeedArterialQTName = "AverageSpeedArterialQueryTemplate";

        public static String BusPassThruQueryName = "PassthroughBusQueryName";
        public static String RailPassThruQueryName = "PassthroughRailQueryName";
        public static String RampPassThruQueryName = "PassthroughRampQueryName";
        public static String TravelTimePassThruQueryName = "PassthroughTravelTimeQueryName";
        public static String D7EventPassThruQueryName = "PassthroughD7EventQueryName";
        public static String CHPEventPassThruQueryName = "PassthroughCHPEventQueryName";
        public static String RegionalLAEventPassThruQueryName = "PassthroughRegionalLAEventQueryName";
        public static String CmsPassThruQueryName = "PassthroughCmsQueryName";
    }
}
