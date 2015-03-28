using Microsoft.Owin.Hosting;
using System;
using System.Net.Http;

namespace fingerprints_service
{
    public class Program
    {
        private static string serverUrl;

        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:3000/";
            if (args.Length > 0) {
                serverUrl = args[0];
            }
            else
            {
                serverUrl = "http://october-user.codio.io:3000";
            }
            

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Server started on " + baseAddress);
                Console.WriteLine("Using REST server: " + serverUrl);
                Console.WriteLine("Press Enter to Exit.");
                Console.ReadLine(); 
            }
        }

        public static string ServerUrl
        {
            get
            {
                return serverUrl;
            }
        }
    }
}
