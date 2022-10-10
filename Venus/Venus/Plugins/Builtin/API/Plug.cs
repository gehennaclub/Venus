using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Venus.Models;
using Venus.Models.JSON;

namespace Venus.Plugins.Builtin.API
{
    public class Plug : Plugin
    {
        private static string api = "https://api01.doaxvv.com/v1";
        private Models.JSON.Maintenance.Rootobject maintenanceRoot { get; set; }
        private Models.JSON.Start.Rootobject startRoot { get; set; }
        private Models.JSON.Resource.Rootobject ResourcesRoot { get; set; }
        private HttpClient httpClient { get; set; }
        private string SID { get; set; }
        private bool cookieAdded { get; set; }

        public Plug(MainWindow window, string name) : base(window, name, true)
        {
            httpClient = new HttpClient();

            cookieAdded = false;
        }

        private async Task Contact(string path, Func<HttpResponseMessage, Task> function, bool cookie)
        {
            HttpResponseMessage response = null;

            if (cookie == true && cookieAdded == false)
            {
                httpClient.DefaultRequestHeaders.Add("Cookie", $"DOAXVVSID={SID}");
                cookieAdded = true;
            }
            response = await httpClient.GetAsync($"{api}/{path}");

            if (response.IsSuccessStatusCode == false)
            {
                logger.Record(Logger.Plug.Type.error, $"API refused Venus client {path}");
            }
            else
            {
                await function(response);
            }
        }

        private async Task LoadMaintenance(HttpResponseMessage response)
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "Maintenance checked");
            
            maintenanceRoot = JsonConvert.DeserializeObject<Models.JSON.Maintenance.Rootobject>(await response.Content.ReadAsStringAsync());
            window.MaintenanceStatus.Text = $"{maintenanceRoot.maintenance}";

            await Cookie(response.Headers);
        }

        private async Task Maintenance()
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Checking game maintenance...");
            await Contact("maintenance", LoadMaintenance, false);
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
                }
                else
                {
                    logger.Record(Plugins.Builtin.Logger.Plug.Type.error, "SID not found");
                }
                window.SID.Text = $"{SID}";
            }
            else
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.error, "Response doesn't contains SID");
            }

            return (Task.CompletedTask);
        }

        private async Task LoadStart(HttpResponseMessage response)
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "Game started");
            startRoot = JsonConvert.DeserializeObject<Models.JSON.Start.Rootobject>(await response.Content.ReadAsStringAsync());
            window.StartStatus.Text = $"{startRoot.gamestart}";
        }

        private async Task Start()
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Starting game...");

            await Contact($"gamestart", LoadStart, true);
        }

        private async Task LoadResources(HttpResponseMessage response)
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, "Retriving resources completed");
            
            ResourcesRoot = JsonConvert.DeserializeObject<Models.JSON.Resource.Rootobject>(await response.Content.ReadAsStringAsync());
            RenderResource<Models.JSON.Resource.Low>(ResourcesRoot.resource_list.low, window.ResourcesListLow);
            RenderResource<Models.JSON.Resource.Common>(ResourcesRoot.resource_list.common, window.ResourcesListCommon);
            RenderResource<Models.JSON.Resource.High>(ResourcesRoot.resource_list.high, window.ResourcesListHigh);
            RenderResource<Models.JSON.Resource.Exe>(ResourcesRoot.resource_list.exe, window.ResourcesListExe);
        }

        private async Task Resources()
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, "Retriving resources...");

            await Contact($"resource/list", LoadResources, true);
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

        public async Task FirstPass()
        {
            await Maintenance();
            await Start();
            await Resources();
        }
    }
}
