using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using Venus.Models;
using Venus.Tools;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;

using Newtonsoft.Json;
using AdonisUI;

namespace Venus.ViewModels
{
    public class MainViewModel : Base
    {
        private MainWindow window { get; set; }
        private Components components { get; set; }
        private static string api = "https://api01.doaxvv.com/v1";
        private Models.JSON.Maintenance.Rootobject maintenanceRoot { get; set; }
        private string SID { get; set; }

        public MainViewModel(MainWindow window, string name) : base(window, name)
        {
            this.window = window;

            Initialize();
        }

        private void Initialize()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Loading components...");

            components = new Components();

            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "Components loaded");
        }

        private async Task Maintenance()
        {
            HttpResponseMessage maintenance = null;

            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Checking game maintenance...");
            maintenance = await components.httpClient.GetAsync($"{api}/maintenance");

            if (maintenance.IsSuccessStatusCode == false)
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.error, "Maintenance check refused");
            } else
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "Maintenance checked");
                maintenanceRoot = JsonConvert.DeserializeObject<Models.JSON.Maintenance.Rootobject>(await maintenance.Content.ReadAsStringAsync());
                window.MaintenanceStatus.Text = $"{maintenanceRoot.maintenance}";
                window.MaintenanceStatus.Foreground = (maintenanceRoot.maintenance == true) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.LimeGreen;
                await Cookie(maintenance.Headers);
            }
        }

        private Task Cookie(HttpResponseHeaders headers)
        {
            IEnumerable<string> results = null;

            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Trying to find DOAXVV SID...");
            results = headers.GetValues("Set-Cookie");
            if (results != null)
            {
                if (results.Count() > 0)
                {
                    SID = results.First().Remove(0, 10).Remove(32, 8);
                    logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "SID found");
                } else
                {
                    logger.Record(Plugins.Builtin.Logger.Plug.Type.error, "SID not found");
                }
                window.SID.Text = $"{SID}";
            } else
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.error, "Response doesn't contains SID");
            }

            return (Task.CompletedTask);
        }
        
        public async Task Load()
        {
            await Maintenance();
        }

        public async Task EventLoad()
        {
            await Run(Load);
        }
    }
}
