/**
 * Created by Seyed Kazemitabar (kazemita@usc.edu) 
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 03/22/2011
 */

/**
 * Updated by Bei(Penny) Pan (beipan@usc.edu) 
 * Purpose: Update Bus, Ramp, Rail, TravelTime Table Name, username, pwd
 * at Integrated Media Systems Center (IMSC), University of Southern California.
 * Date: 04/18/2011
 */


using System;

namespace Utils
{
    public class DBTableStrings
    {
        public static readonly String FreewayTableName = "highway_congestion_data";
        public static readonly String FreewayConfigTableName = "highway_congestion_config";

        public static readonly String ArterialDataTableName = "arterial_congestion_data";
        public static readonly String ArterialConfigTableName = "arterial_congestion_config";
        
        public static readonly String TravelTimeTableName = "FREEWAY_TRAVEL_TIME";
        public static readonly String TravelTimeConfigTableName = "FREEWAY_TRAVEL_TIME_Config";

        public static readonly String RampMeterTableName = "RAMP_METER_DATA";
        public static readonly String RampMeterConfigTableName = "RAMP_METER_CONFIG";

        public static readonly String MetroBusTableName = "METRO_BUS_DATA";
        public static readonly String MetroBusConfigTableName = "METRO_BUS_CONFIG";

        public static readonly String MetroRailTableName = "METRO_RAIL_DATA";
        public static readonly String MetroRailConfigTableName = "METRO_RAIL_CONFIG";

        public static readonly String EventTableName = "EVENT";
        public static readonly String CmsTableName = "CMS";
        public static readonly String CmsConfigTableName = "CMS_CONFIG";

        public static readonly String Freeway_user = "HIGHWAY";
        public static readonly String Freeway_pwd = "hphe106";

        public static readonly String Arterial_user = "ARTERIAL";
        public static readonly String Arterial_pwd = "aphe106";

        public static readonly String Transit_user = "TRANSIT";
        public static readonly String Transit_pwd = "tphe106";

        public static readonly String Event_user = "EVENT";
        public static readonly String Event_pwd = "ephe106";


    }
}
