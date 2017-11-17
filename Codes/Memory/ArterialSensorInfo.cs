using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Memory
{
    public class ArterialSensorInfo :FreewaySensorInfo
    {
        public float PostMile { get; set; }
        public int AffectedLaneCnt { get; set; }
        public String AffectedLaneType { get; set; }

        public ArterialSensorInfo(double latitude, double longitude, string OST, int direction, string FST, float postmile,int afLaneCnt, string afLaneType) : base(latitude, longitude, OST, direction, FST)
        {
            PostMile = postmile;
            AffectedLaneCnt = afLaneCnt;
            AffectedLaneType = afLaneType;
        }
    }
}
