using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Venus.Plugins.Builtin.Logger
{
    public class Plug : Plugin
    {
        private int limit { get; set; }
        private List<string> logs { get; set; }
        private string session { get; set; }
        private string folder { get; set; }
        private string message { get; set; }
        private Type type { get; set; }
        private string name { get; set; }
        private List<Action> queue { get; set; }

        private Dictionary<Type, Brush> fore = new Dictionary<Type, Brush>()
        {
            { Type.normal, Brushes.Violet },
            { Type.error, Brushes.Red },
            { Type.canceled, Brushes.Yellow },
            { Type.success, Brushes.LimeGreen },
            { Type.fail, Brushes.Red },
            { Type.wait, Brushes.Orange }
        };

        private Dictionary<Type, string> Log = new Dictionary<Type, string>()
        {
            { Type.normal, "INFO" },
            { Type.canceled, "CANCEL" },
            { Type.success, "DONE" },
            { Type.error, "FAIL" },
            { Type.wait, "WAIT" }
        };

        public Plug(MainWindow window, string name) : base(window, name, false)
        {
            this.window = window;
            this.name = name;

            Initialize();
        }

        private void Initialize()
        {
            limit = 10;
            session = $"{DateTime.Now.ToString("ddMMyyhh")}.log";
            folder = "Logs";

            logs = new List<string>();
            queue = new List<Action>();
        }

        private Task Write()
        {
            TextBlock block = new TextBlock();

            block.TextWrapping = TextWrapping.Wrap;
            block.FontFamily = new FontFamily("Consolas");
            block.Inlines.Add(new Run($"[ {Log[type]} ] ") { Foreground = fore[type] });
            block.Inlines.Add(message);

            window.Log.Children.Add(block);
            window.LogsFooter.Text = $"{message}";

            return (Task.CompletedTask);
        }

        public enum Type
        {
            normal,
            error,
            canceled,
            success,
            fail,
            wait
        }

        public void Record(Type type, string message)
        {
            string full = $"[{DateTime.Now.ToString("hh:mm:ss")}] | {message}";

            this.message = message;
            this.type = type;

            if (logs.Count >= limit)
            {
                Dump();
                logs.Clear();
            }
            logs.Add(full);
            Threads.Invoker.Edit(Write);
        }

        public void Force()
        {
            Dump();
        }

        private void Dump()
        {
            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
            }
            if (Directory.Exists($"{folder}/{name}") == false)
            {
                Directory.CreateDirectory($"{folder}/{name}");
            }
            File.AppendAllLines($"{folder}/{name}/{session}", logs);
            logs.Clear();
        }
    }
}
