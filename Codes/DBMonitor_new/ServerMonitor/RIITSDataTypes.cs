
namespace ServerMonitor
{
    public enum RIITSDataTypes
    {
        Freeway = 0,
        Arterial,
        //FreewayInventory,
        //ArterialInventory,
        Bus,
        Rail,
        Ramp,
        Traveltimes,
        Event,

        CMS
    }

    public static class RIITSFreewayAgency
    {
        public static readonly string Caltrans_D7 = "Caltrans-D7";
        public static readonly string Caltrans_D8 = "Caltrans-D8";
        public static readonly string Caltrans_D12 = "Caltrans-D12";

    }

    public static class RIITSEventAgency
    {
        public static readonly string CHP_LA = "CHP-LA";
        public static readonly string Caltrans_D7 = "Caltrans-D7";
        public static readonly string CHP_Inland = "CHP-Inland";
        public static readonly string CHP_OC = "CHP-OC";
        public static readonly string Regional_LA = "Regional-LA";
    }

    public static class RIITSArterialAgency
    {
        public static readonly string LADOT = "LADOT";
    }

    public static class RIITSBusAgency
    {
        public static readonly string FHT = "FHT";
        public static readonly string LBT = "LBT";
        public static readonly string MTA_Metro = "MTA-Metro";
    }

    public enum ReadType
    {
        Record = 0,
        Raw
    } ;
}