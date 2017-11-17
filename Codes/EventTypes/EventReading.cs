using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventTypes
{
    public class EventReading
    {
        public int EventId { get; set; }
        public string Agency { get; set; }
        public string OnStreet { get; set; }
        public string FromStreet { get; set; }
        public string ToStreet { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Direction { get; set; }
        public string AdminCity { get; set; }
        public double AdminPostmile { get; set; }
        public int TypeEvent { get; set; }
        public int Severity { get; set; }
        public string Description { get; set; }
        public int AffectedLaneCnt { get; set; }
        public int AffectedLaneType { get; set; }
        public int VecType9220 { get; set; }
        public int VecType9227 { get; set; }
        public int VecType9228 { get; set; }
        public int VecType9290 { get; set; }
        public int FatalityCnt { get; set; }
        public int PossibleInjCnt { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ClearTime { get; set; }
        public string IssuingUser { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public bool HighwayPatrol { get; set; }
        public bool CountyFire { get; set; }
        public bool CountySheriff { get; set; }
        public bool FireDepartment { get; set; }
        public bool Ambulance { get; set; }
        public bool Coroner { get; set; }
        public bool Mait { get; set; }
        public bool Hazmat { get; set; }
        public bool FreewayServicePatrol { get; set; }
        public bool CaltransMaintenance { get; set; }
        public bool CaltransTMT { get; set; }
        public bool CountySheriffTSB { get; set; }
        public bool Other { get; set; }
        public string OtherText { get; set; }
        public string CommentInternalContent { get; set; }
        public string CommentInternalText { get; set; }
        public string CommentExternalContent { get; set; }
        public string CommentExternalText { get; set; }
        public DateTime ActualStartTime { get; set; }
        public DateTime ActualEndTime { get; set; }
        public int EventStatus { get; set; }
    }
}
