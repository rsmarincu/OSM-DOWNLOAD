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
    public class BakeOSM : GH_Component
    {
        //Properties
        public override Guid ComponentGuid { get { return new Guid("261f73b0302b478791290a5ba4bf44ab"); } }
        protected override Bitmap Icon { get { return Properties.Resources.roads; } }
        //Constructor
        public BakeOSM() : base("Full map", "Map", "Get the entire map into 3d points", "OSM Data", "Get Osm") { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("OSM Points", "OSM Pt", "Converted Osm Points", GH_ParamAccess.list);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("OSM Geo", "Geo", "Map Points", GH_ParamAccess.tree);
            pManager.AddTextParameter("Layer Names", "Layers", "OSM Features", GH_ParamAccess.tree);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Get inputs
            List<Geo> geoList = new List<Geo>();

            DA.GetDataList<Geo>(0, geoList);

            int i = 0;

            Grasshopper.DataTree<Point3d> GeoLayers = new DataTree<Point3d>();
            Grasshopper.DataTree<string> LayerNames = new DataTree<string>();


            foreach (Geo g in geoList)
            {
                GH_Path pth = new GH_Path(i);
                foreach(Point3d p in g.Points)
                {
                    GeoLayers.Add(p, pth);
                }

                string name = g.Tag.Keys.FirstOrDefault();
                LayerNames.Add(name, pth);
                i++;
            }

            //Outputs
            DA.SetDataTree(0, GeoLayers);
            DA.SetDataTree(1, LayerNames);
        }
       
    }

}
