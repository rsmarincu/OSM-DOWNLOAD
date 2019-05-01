using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;

using CoordinateSharp;

namespace OsmTools
{
    public class Node
    {
        public string ID { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public Point3d convertToUTM()
        {
            Coordinate coordinate = new Coordinate(this.Lat, this.Lon);
            var easting = coordinate.UTM.Easting;
            var northing = coordinate.UTM.Northing;
            Point3d point = new Point3d(easting,northing,0);
            return point;
        }
    }

    public class Way
    {
        public Dictionary<string, string> Tags { get; set; }
        public List<string> Refs { get; set; }
    }

    public class Geo
    {
        public List<Point3d> Points { get; set; }
        public Dictionary<string, string> Tag { get; set; }

    }
}
