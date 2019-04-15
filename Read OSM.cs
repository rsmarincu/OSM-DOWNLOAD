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
    public class ReadOsm: GH_Component
    {
        //Properties
        public override Guid ComponentGuid { get { return new Guid("d664ac6e819e474494b84f128623218a"); } }
        protected override Bitmap Icon { get { return null; } }
        //Constructor
        public ReadOsm() : base("Read OSM File", "OSM", "Read the OSM file.", "OSM Data", "Get Osm") { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("OSM filepath","Filepath","Output from OSM Download",GH_ParamAccess.item,"");
            pManager.AddPointParameter("Origin", "O", "South-West corner of the map", GH_ParamAccess.item);

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("ID","ID","Node ID",GH_ParamAccess.item);
  
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {

        }

        public XmlDocument ReadOsmFile(string filePath)
        {
            string xmlFile = File.ReadAllText(filePath);
            XmlDocument xmlDoc = new XmlDocument();
            return xmlDoc;
        }

    }
}
