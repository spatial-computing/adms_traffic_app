using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;

namespace Memory
{
    public class FreewaySensorInfo
    {
        const int SRID = 4326;
        public FreewaySensorInfo(double latitude, double longitude, String OST, int direction, String FST)
        {
            //Location = SqlGeography.Point(latitude, longitude, SRID);
            //UTMConverter.LLtoUTM(UTMConverter.RefEllipsoid, latitude, longitude);
            //double x = UTMConverter.UTMEasting;
            //double y = UTMConverter.UTMNorthing;

            //GeomLocation = SqlGeometry.Point(x, y, SRID);
            GeogLocation = SqlGeography.Point(latitude, longitude, SRID);
            OnStreet = OST;
            FromStreet = FST;
            Direction = direction;
        }

        
        public SqlGeography GeogLocation { get; set; }
        public String OnStreet { get; set; }
        public int Direction { get; set; }
        public String FromStreet { get; set; }
    }
}
