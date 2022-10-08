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
using System.Windows.Documents;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

namespace Venus.ViewModels
{
    public class MainViewModel : Base
    {
        private MainWindow window { get; set; }
        private Components components { get; set; }
        private static string api = "https://api01.doaxvv.com/v1";
        private Models.JSON.Maintenance.Rootobject maintenanceRoot { get; set; }
        private Models.JSON.Start.Rootobject startRoot { get; set; }
        private Models.JSON.Resource.Rootobject ResourcesRoot { get; set; }
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

        private async Task Start()
        {
            HttpResponseMessage response = null;

            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Starting game...");

            components.httpClient.DefaultRequestHeaders.Add("Cookie", $"DOAXVVSID={SID}");
            response = await components.httpClient.GetAsync($"{api}/gamestart");

            if (response.IsSuccessStatusCode == false)
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.error, "Game starting failed");
            }
            else
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "Game started");
                startRoot = JsonConvert.DeserializeObject<Models.JSON.Start.Rootobject>(await response.Content.ReadAsStringAsync());
                window.StartStatus.Text = $"{startRoot.gamestart}";
                window.StartStatus.Foreground = (startRoot.gamestart == false) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.LimeGreen;
            }
        }

        private async Task Resources()
        {
            HttpResponseMessage response = null;

            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Retriving resources...");

            components.httpClient.DefaultRequestHeaders.Add("Cookie", $"DOAXVVSID={SID}");
            response = await components.httpClient.GetAsync($"{api}/resource/list");

            if (response.IsSuccessStatusCode == false)
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.error, "Game retriving resources failed");
            }
            else
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "Retriving resources completed");
                ResourcesRoot = JsonConvert.DeserializeObject<Models.JSON.Resource.Rootobject>(await response.Content.ReadAsStringAsync());
                RenderResource<Models.JSON.Resource.Low>(ResourcesRoot.resource_list.low, window.ResourcesListLow);
                RenderResource<Models.JSON.Resource.Common>(ResourcesRoot.resource_list.common, window.ResourcesListCommon);
                RenderResource<Models.JSON.Resource.High>(ResourcesRoot.resource_list.high, window.ResourcesListHigh);
                RenderResource<Models.JSON.Resource.Exe>(ResourcesRoot.resource_list.exe, window.ResourcesListExe);
            }
        }

        private void RenderResource<T>(dynamic content, StackPanel panel)
        {
            foreach (var resource in content)
            {
                panel.Children.Add(new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    FontFamily = new FontFamily("Consolas"),
                    Text = $"file name: {resource.file_name}\nversion:   {resource.version}\nhash:      {resource.hash}"
                });
                panel.Children.Add(new Separator()
                {
                    Background = new BrushConverter().ConvertFromString("#4C4F62") as Brush
                });
            }
        }

        public async Task Load()
        {
            await Maintenance();
            await Start();
            await Resources();
        }

        public async Task EventLoad()
        {
            await Run(Load);
        }
    }
}
