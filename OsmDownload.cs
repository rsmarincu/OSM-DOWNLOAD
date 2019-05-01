using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Xml;
using System.IO;
using System.Net;

using Rhino;
using Rhino.Geometry;
using Rhino.DocObjects;
using Rhino.Collections;
using Rhino.ApplicationSettings;

using GH_IO;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using CoordinateSharp;

namespace OsmTools
{
    public class OsmDownload: GH_Component
    {
        //Properties
        public override Guid ComponentGuid { get { return new Guid ("a8d43ae11c974d6fb3789feb6955ebba"); } }
        protected override Bitmap Icon { get { return Properties.Resources.map; } }
        //Constructor
        public OsmDownload() : base("Download OSM File", "Download", "Download the map contained in the input boundary", "OSM Data", "Get Osm") { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("West","W","Minimum Latitude",GH_ParamAccess.item,0.0);
            pManager.AddNumberParameter("South","S","Minimum Longitude",GH_ParamAccess.item,0.0);
            pManager.AddNumberParameter("East", "E", "Maximum Latitude", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("North", "N", "Maximum Longitude", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Run", "Run", "Download the osm file", GH_ParamAccess.item, false);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("OSM FilePath","Path","OSM file location",GH_ParamAccess.item);
            pManager.AddGenericParameter("Origin", "O", "South-West corner of the map", GH_ParamAccess.item);

        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Get numbers

            double west = 0.0;
            double south = 0.0;
            double east = 0.0;
            double north = 0.0;
            bool run_ = false;
            string fp = null;
            DA.GetData(0, ref west);
            DA.GetData(1, ref south);
            DA.GetData(2, ref east);
            DA.GetData(3, ref north);
            DA.GetData(4, ref run_);
            if (run_)
            {
                try
                {
                    fp = DownloadFile(west, south, east, north);
                }
                catch
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error,"Save your Rhino File.");
                }
            }

            //Output
            DA.SetData(0, fp);

            Coordinate corner = new Coordinate(south, west);
            Coordinate topCorner = new Coordinate(north, east);

            var cornerEasting = corner.UTM.Easting;
            var cornerNorthing = corner.UTM.Northing;
            var topCornerEasting = topCorner.UTM.Easting;
            var topCornerNorthing = topCorner.UTM.Northing;

            Point3d bottom = new Point3d(cornerEasting, cornerNorthing, 0);
            Point3d top = new Point3d(topCornerEasting, topCornerNorthing, 0);


            var moveVector = new Point3d(0, 0, 0)- bottom;

            DA.SetData(1, moveVector);
        }
        public string DownloadFile(double w,double s,double e,double n)
        {
            string BaseUrl = "http://www.overpass-api.de/api/xapi?map?bbox={0},{1},{2},{3}";

            BaseUrl= String.Format(BaseUrl, w, s, e, n);

            WebClient client = new WebClient();

            Stream data = client.OpenRead(BaseUrl);
            StreamReader reader = new StreamReader(data);

            string text = reader.ReadToEnd();
            XmlDocument osmfile = new XmlDocument();

            string filepath = Rhino.RhinoDoc.ActiveDoc.Path;
            filepath = System.IO.Path.GetDirectoryName(filepath);

            filepath = filepath + "\\osmfile.osm";
            osmfile.LoadXml(text);
            osmfile.Save(filepath);

            data.Close();
            reader.Close();
            return filepath;
        }
    }

}
