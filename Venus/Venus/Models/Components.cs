using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Venus.Models
{
    public class Components
    {
        public Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog { get; set; }
        public Ookii.Dialogs.Wpf.VistaOpenFileDialog fileDialog { get; set; }
        public HttpClient httpClient { get; set; }

        public Components()
        {
            httpClient = new HttpClient();
            folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog()
            {
                Multiselect = true
            };
            fileDialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
        }
    }
}
