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
using Venus.Resources;

namespace Venus.Plugins.Builtin.API
{
    public class Plug : Plugin
    {
        private static string api = "https://api01.doaxvv.com/v1";
        private Content content { get; set; }
        private HttpClient httpClient { get; set; }
        private string SID { get; set; }
        private bool cookieAdded { get; set; }

        public Plug(MainWindow window, string name) : base(window, name, true)
        {
            httpClient = new HttpClient();
            content = new Content();

            cookieAdded = false;
            SID = "failed";
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
                logger.Record(Logger.Plug.Type.error, Logs.Client.refused);
            }
            else
            {
                await function(response);
            }
        }

        private async Task LoadMaintenance(HttpResponseMessage response)
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, Logs.Maintenance.checkCompleted);

            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, Logs.Maintenance.setupLoad);
            content.maintenanceRoot = JsonConvert.DeserializeObject<Models.JSON.Maintenance.Rootobject>(await response.Content.ReadAsStringAsync());
            window.MaintenanceStatus.Text = $"{content.maintenanceRoot.maintenance}";
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, Logs.Maintenance.setupCompleted);

            await Cookie(response.Headers);
        }

        private async Task Maintenance()
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, Logs.Maintenance.checkLoad);
            
            await Contact("maintenance", LoadMaintenance, false);
        }

        private Task Cookie(HttpResponseHeaders headers)
        {
            IEnumerable<string> results = null;

            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, Logs.SID.search);
            results = headers.GetValues("Set-Cookie");
            if (results != null)
            {
                if (results.Count() > 0)
                {
                    logger.Record(Plugins.Builtin.Logger.Plug.Type.success, Logs.SID.found);
                    SID = results.First().Remove(0, 10).Remove(32, 8);
                }
                else
                {
                    logger.Record(Plugins.Builtin.Logger.Plug.Type.error, Logs.SID.notFound);
                }
                window.SID.Text = $"{SID}";
            }
            else
            {
                logger.Record(Plugins.Builtin.Logger.Plug.Type.error, Logs.SID.missing);
            }

            return (Task.CompletedTask);
        }

        private async Task LoadStart(HttpResponseMessage response)
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, Logs.Start.starting);

            content.startRoot = JsonConvert.DeserializeObject<Models.JSON.Start.Rootobject>(await response.Content.ReadAsStringAsync());
            window.StartStatus.Text = $"{content.startRoot.gamestart}";
        }

        private async Task Start()
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, Logs.Start.started);

            await Contact($"gamestart", LoadStart, true);
        }

        private async Task LoadResources(HttpResponseMessage response)
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, Logs.Resources.retrivingCompleted);

            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, Logs.Resources.setupLoad);
            content.ResourcesRoot = JsonConvert.DeserializeObject<Models.JSON.Resource.Rootobject>(await response.Content.ReadAsStringAsync());
            RenderResource<Models.JSON.Resource.Low>(content.ResourcesRoot.resource_list.low, window.ResourcesListLow);
            RenderResource<Models.JSON.Resource.Common>(content.ResourcesRoot.resource_list.common, window.ResourcesListCommon);
            RenderResource<Models.JSON.Resource.High>(content.ResourcesRoot.resource_list.high, window.ResourcesListHigh);
            RenderResource<Models.JSON.Resource.Exe>(content.ResourcesRoot.resource_list.exe, window.ResourcesListExe);
            logger.Record(Plugins.Builtin.Logger.Plug.Type.success, Logs.Resources.setupCompleted);
        }

        private async Task Resources()
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, Logs.Resources.retriving);

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
