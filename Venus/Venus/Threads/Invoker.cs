using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venus.Threads
{
    public class Invoker
    {
        public static void Edit(Func<Task> action)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                action();
            });
        }
    }
}
