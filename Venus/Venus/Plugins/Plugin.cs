using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venus.Plugins
{
    public abstract class Plugin
    {
        public string Name { get; set; }
        public MainWindow MainWindow { get; set; }
        public Plugins.Builtin.Logger.Plug logger { get; set; }

        public Plugin(MainWindow mainWindow, string name, bool trace)
        {
            Name = name;
            MainWindow = mainWindow;
            if (trace == true)
            {
                logger = new Builtin.Logger.Plug(mainWindow, name);
            }
        }
    }
}
