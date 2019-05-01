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
        protected override Bitmap Icon { get { return Properties.Resources.osmpoints; } }
        //Constructor
        public ReadOsm() : base("Read OSM File", "OSM", "Read the OSM file.", "OSM Data", "Get Osm") { }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("OSM filepath","Filepath","Output from OSM Download",GH_ParamAccess.item,"");
            pManager.AddVectorParameter("Origin", "O", "South-West corner of the map", GH_ParamAccess.item);

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("OSM Points", "OSM Pt", "Converted Osm Points", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Get Input Data
            string filepath = null;
            Vector3d origin = new Vector3d();
            DA.GetData(0, ref filepath);
            DA.GetData(1, ref origin);

            //Run Scripts
            XmlDocument xmlDoc = ReadOsmFile(filepath);

            List<Geo> geoList = new List<Geo>();
            List<Way> wayList = new List<Way>();
            List<Node> nodeList = new List<Node>();

            ProcessOsm(xmlDoc,wayList,nodeList);

            foreach (Way w in wayList)
            {
                if (w.Refs != null)
                {
                    List<Point3d> points = new List<Point3d>();

                    foreach (string r in w.Refs)
                    {
                        Node matching = nodeList.AsParallel().FirstOrDefault(n => n.ID == r);
                        Point3d matchingUtm = matching.convertToUTM();
                        Point3d moved = matchingUtm + origin;
                        points.Add(moved);
                    }
                    geoList.Add(new Geo { Tag = w.Tags, Points = points });
                }
            }

            DA.SetDataList(0, geoList);
        }

        public XmlDocument ReadOsmFile(string filePath)
        {
            string xmlFile = File.ReadAllText(filePath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile);
            return xmlDoc;

        }

        public void ProcessOsm(XmlDocument xmlDoc, List<Way> wayList, List<Node> nodeList)
        {
            string[] featureKeys = { "aerialway", "aeroway", "amenity", "barrier", "boundary", "building", "craft", "emergency", "geological", "highway",
                                      "historic", "landuse", "leisure", "man_made", "military", "natural", "office", "place", "power", "public_transport",
                                      "railway", "route", "shop", "sport", "tourism", "waterway" };

            foreach (XmlNode node in xmlDoc.GetElementsByTagName("way"))
            {
                var refList = new List<string>();
                Dictionary<string, string> tags = new Dictionary<string, string>();

                foreach (XmlElement nd in node.ChildNodes)
                {

                    if (nd.Name == "nd")
                    {
                        var attribute = nd.GetAttribute("ref");
                        if (attribute != null)
                        {
                            refList.Add(attribute.ToString());
                        }

                    }
                    else if (nd.Name == "tag")
                    {
                        string checkAttribute = nd.GetAttribute("k");

                        if (featureKeys.Contains(checkAttribute, StringComparer.OrdinalIgnoreCase))
                        {
                            tags.Add(nd.GetAttribute("k"), nd.GetAttribute("v"));
                        }

                    }
                }
                wayList.Add(new Way { Refs = refList, Tags = tags });
            }

            foreach (XmlElement node in xmlDoc.GetElementsByTagName("node"))
            {
                string id_ = node.GetAttribute("id");
                double lat_ = double.Parse(node.GetAttribute("lat"));
                double lon_ = double.Parse(node.GetAttribute("lon"));
                nodeList.Add(new Node { ID = id_, Lat = lat_, Lon = lon_ });
            }
        }
    }
}
