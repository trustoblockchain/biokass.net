using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;
using Mono.Options;
using System.Collections.Generic;
using System.Threading;

namespace fingerprints_service
{
    public class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        private static string serverUrl;
        private static string waitdeviceScript;
        private static float treshold;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                Console.WriteLine("Exit...");
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            string baseAddress = "http://localhost:3000/";
            serverUrl = "http://www.winstonkw.com";
            waitdeviceScript = "/assets/js/scan-page.js";
            treshold = 25;
            bool showHelp = false;

            var p = new OptionSet()
            {
                {"t|treshold=", "the biometric match treshold (default is " + treshold + ")",
                    (float v) => treshold = v},
                {"b|base=", "the base url to listen on for commands (default is " + baseAddress + ")",
                    v => baseAddress = v},
                {"s|script=", "relative location of wait devices script. Default is /js/scan-page.js",
                    v => waitdeviceScript = v},
                {"h|help", "show this help message and exit",
                    v => showHelp = (v != null)}
            };

            try
            {
                List<string> extra = p.Parse(args);
                if (extra.Count > 0)
                {
                    serverUrl = extra[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parsing command line arguments. Try \n\tfingerprints_service.exe --help");
                return;
            }
            


            if (showHelp)
            {
                Console.WriteLine("Usage: \n\tfingerprints_service.exe [options...] [server_url]");
                Console.WriteLine();
                Console.WriteLine("The server_url is the http or https url of the main web application server.");
                Console.WriteLine("If no server_url is specified, it will be used the default one: " + serverUrl);
                Console.WriteLine();
                Console.WriteLine("Options");
                p.WriteOptionDescriptions(Console.Out);
                Console.WriteLine();
                Console.WriteLine("Examples");
                Console.WriteLine("\tfingerprints_service.exe -t 110 https:////october-user.codio.io:9500");
                return;
            }

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Server started on " + baseAddress);
                Console.WriteLine("Web application server: " + serverUrl);
                Console.WriteLine("Using biometric match treshold: " + treshold);
                Console.WriteLine("Waitdevice script: " + waitdeviceScript);
                Console.WriteLine("Press Ctrl+C to Exit.");

                _quitEvent.WaitOne();
            }
        }

        public static string WaitdeviceScript
        {
            get
            {
                //"http://october-user.codio.io:3000/js/scan-page.js"
                return serverUrl + waitdeviceScript;
            }
        }
        public static string ServerUrl
        {
            get
            {
                return serverUrl;
            }
        }

        public static float Treshold
        {
            get
            {
                return treshold;
            }
        }
    }
}
