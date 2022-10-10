using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venus.ViewModels
{
    public abstract class Base
    {
        public MainWindow mainWindow { get; set; }
        public List<Func<Task>> queue { get; set; }
        public Dictionary<object, Func<Task>> actions { get; set; }
        public Plugins.Builtin.Logger.Plug logger { get; set; }
        public Plugins.Builtin.API.Plug api { get; set; }
        public Ookii.Dialogs.Wpf.VistaFolderBrowserDialog folderDialog { get; set; }
        public Ookii.Dialogs.Wpf.VistaOpenFileDialog fileDialog { get; set; }
        public string name { get; set; }

        public Base(MainWindow mainWindow, string name)
        {
            this.mainWindow = mainWindow;

            queue = new List<Func<Task>>();
            actions = new Dictionary<object, Func<Task>>();
            logger = new Plugins.Builtin.Logger.Plug(this.mainWindow, name);
            api = new Plugins.Builtin.API.Plug(this.mainWindow, name);
            folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            fileDialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog();
        }

        public async Task Run(Func<Task> action)
        {
            logger.Record(Plugins.Builtin.Logger.Plug.Type.wait, $"Running action {action.Method.Name}");

            queue.Add(action);
            await Wait();

            logger.Force();
        }

        private async Task Wait()
        {
            foreach (Func<Task> job in queue)
            {
                await Task.Run(() => Threads.Invoker.Edit(job));
            }
            queue.Clear();
        }
    }
}
