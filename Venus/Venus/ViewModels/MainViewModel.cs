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
using System.Reflection;

namespace Venus.ViewModels
{
    public class MainViewModel : Base
    {
        private MainWindow window { get; set; }
        private Components components { get; set; }

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

        private Task VenusVersion()
        {
            window.VersionValue.Text = $"{Assembly.GetExecutingAssembly().GetName().Version}";

            return (Task.CompletedTask);
        }

        private async Task Load()
        {
            await VenusVersion();
            await api.FirstPass();

            logger.Record(Plugins.Builtin.Logger.Plug.Type.normal, Resources.Logs.Client.allLoaded);
        }

        public async Task EventLoad()
        {
            await Run(Load);
        }
    }
}
