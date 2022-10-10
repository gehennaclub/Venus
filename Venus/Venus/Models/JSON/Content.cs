using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venus.Models.JSON
{
    public class Content
    {
        public Models.JSON.Maintenance.Rootobject maintenanceRoot { get; set; }
        public Models.JSON.Start.Rootobject startRoot { get; set; }
        public Models.JSON.Resource.Rootobject ResourcesRoot { get; set; }
    }
}
