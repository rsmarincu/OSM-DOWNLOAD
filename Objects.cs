using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadOsm
{
    public class Node
    {
        public string ID { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

    }
    public class Way
    {
        public Dictionary<string,string> Tags { get; set; }
        public List<string> Refs { get; set; }

    }
}
