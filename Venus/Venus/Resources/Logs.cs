using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venus.Resources
{
    public class Logs
    {
        public class Maintenance
        {
            public static string checkLoad = "Checking game maintenance...";
            public static string checkCompleted = "Game maintenance checked";
            public static string setupLoad = "Setting up maintenance";
            public static string setupCompleted = "Maintenance setted up";
        }

        public class SID
        {
            public static string search = "Searching game SID...";
            public static string missing = "Response doesn't contain SID";
            public static string found = "SID found";
            public static string notFound = "SID not found";
        }

        public class Start
        {
            public static string starting = "Starting game...";
            public static string started = "Game started";
        }

        public class Resources
        {
            public static string retriving = "Retriving resources...";
            public static string retrivingCompleted = "Retriving resources completed";
            public static string setupLoad = "Setting up resources...";
            public static string setupCompleted = "Resources setted up";
        }

        public class Client
        {
            public static string refused = "API refused Venus client";
            public static string waiting = "Waiting";
            public static string ready = "Ready";
            public static string allLoaded = "All loaded";
        }
    }
}
