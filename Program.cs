using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Xml;
using CoordinateSharp;

namespace DownloadOsm
{
    class Program
    {
        private static void Main()
        {
            string[] featureKeys = { "aerialway", "aeroway", "amenity", "barrier", "boundary", "building", "craft", "emergency", "geological", "highway",
                                      "historic", "landuse", "leisure", "man_made", "military", "natural", "office", "place", "power", "public_transport",
                                      "railway", "route", "shop", "sport", "tourism", "waterway" };

            string xmlFile = File.ReadAllText(@"C:\\Users\\rmarincu\\Dropbox\\DI Parametric Team\\Tools\\Osm plugin\\osmfile.osm");
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlFile);

            var wayList = new List<Way>();
            var nodeList = new List<Node>();

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
                wayList.Add(new Way { Refs = refList, Tags=tags});
            }

            foreach (XmlElement node in xmlDoc.GetElementsByTagName("node"))
            {
                string id_ = node.GetAttribute("id");
                double lat_ = double.Parse(node.GetAttribute("lat"));
                double lon_ = double.Parse(node.GetAttribute("lon"));
                nodeList.Add(new Node { ID = id_, Lat = lat_, Lon = lon_ });
            }

            foreach (Way w in wayList)
            {
                Console.WriteLine("way");
                foreach (string rf in w.Refs)
                {
                    Console.WriteLine(rf);
                }
                Console.WriteLine("Pairs:"+w.Tags.Count);
                foreach (KeyValuePair<string,string> r in w.Tags)
                {
                    Console.WriteLine(r);
                }
            }
            foreach (Node n in nodeList)
            {
                Console.WriteLine(n.ID + " " + n.Lat + " " + n.Lon);
            }
            Console.WriteLine(wayList.Count + " " + nodeList.Count);
            Console.ReadKey();
        }
        
    }
}
