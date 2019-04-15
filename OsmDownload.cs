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
        protected override Bitmap Icon { get { return Properties.Resources.DownloadIcon; } }
        //Constructor
        public OsmDownload() : base("Download OSM File", "DWD OSM", "Download the map contained in the input boundary", "OSM Data", "Get Osm") { }

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
                fp = DownloadFile(west, south, east, north);
            }

            //Output
            DA.SetData(0, fp);
            Coordinate corner = new Coordinate(west, south);
            var cornerEasting = double.Parse(corner.UTM.Easting.ToString());
            var cornerNorthing = double.Parse(corner.UTM.Northing.ToString());
            Point2d mapCorner = new Point2d(cornerEasting, cornerNorthing);
            DA.SetData(1, mapCorner);
        }
        public string DownloadFile(double w,double s,double e,double n)
        {
            string BaseUrl = "https://overpass-api.de/api/interpreter";
            string payload = @"(
                                node({0}, {1}, {2}, {3});
                                <;
                             );
                             out meta;";

            payload = String.Format(payload, w, s, e, n);

            WebClient client = new WebClient();
            client.QueryString.Add("data", payload);

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
