using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TrafficMonitor
{
    class Program
    {
        private static readonly String defaultPath = "tmp/access.log";
        static void Main(string[] args)
        {

            # region config
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var path = configuration["Log:Path"];
            if (String.IsNullOrEmpty(path))
            {
                path = defaultPath;
            }

            int intervalDurationSec;
            try
            {
                intervalDurationSec = int.Parse(configuration["Analytics:TimeSpanSec"]);
            }
            catch
            {
                intervalDurationSec = 10;
            }

            var analytics = new Analytics(
                int.Parse(configuration["Analytics:TopSections"]),
                intervalDurationSec);

            var alerting = new Alerting(
                int.Parse(configuration["Alerting:HitsThreshold"]),
                int.Parse(configuration["Alerting:TimeSpanSec"]));

            # endregion config

            Stopwatch logStopwatch = new Stopwatch();
            logStopwatch.Start();
            Stopwatch alertStopwatch = new Stopwatch();
            alertStopwatch.Start();

            try
            {   
                // Open the text file using a stream reader.
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        // Get to the end of the file
                        // Doing RT monitoring, we don't want to start at the beginning of a
                        //   potentially huge log file.
                        if (sr.BaseStream.Length > 10)
                        {
                            sr.BaseStream.Seek(-10, SeekOrigin.End);
                            sr.ReadLine(); // flush probably partial last line bcs of offset 10
                        }
                        while (true)
                        {
                            var line = sr.ReadLine();
                            if (!String.IsNullOrEmpty(line))
                            {
                                var log = new Log(line);
                                // TODO Remove console output
                                Console.WriteLine(line);
                                analytics.Register(log);
                                alerting.Register(log);
                            }
                            if (logStopwatch.ElapsedMilliseconds > intervalDurationSec * 1000)
                            {
                                analytics.Display();
                                analytics.Flush();
                                logStopwatch.Restart();
                            }
                            // we check the alert state every second
                            if (alertStopwatch.ElapsedMilliseconds > 1000)
                            {
                                alerting.CheckAlertStatus();
                                alertStopwatch.Restart();
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
       }
    }
}
