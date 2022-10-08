using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Venus.Models.JSON
{
    public class Resource
    {

        public class Rootobject
        {
            public Resource_List resource_list { get; set; }
        }

        public class Resource_List
        {
            public Common[] common { get; set; }
            public Low[] low { get; set; }
            public High[] high { get; set; }
            public Exe[] exe { get; set; }
        }

        public class Common
        {
            public int version { get; set; }
            public string directory { get; set; }
            public string file_name { get; set; }
            public long file_size { get; set; }
            public string hash { get; set; }
        }

        public class Low
        {
            public int version { get; set; }
            public string directory { get; set; }
            public string file_name { get; set; }
            public int file_size { get; set; }
            public string hash { get; set; }
        }

        public class High
        {
            public int version { get; set; }
            public string directory { get; set; }
            public string file_name { get; set; }
            public int file_size { get; set; }
            public string hash { get; set; }
        }

        public class Exe
        {
            public int version { get; set; }
            public string directory { get; set; }
            public string file_name { get; set; }
            public int file_size { get; set; }
            public string hash { get; set; }
        }
    }
}
