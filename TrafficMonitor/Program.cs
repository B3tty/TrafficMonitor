using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TrafficMonitor
{
    class Program
    {
        private static readonly String _defaultPath = "/tmp/access.log";
        static void Main(string[] args)
        {
            # region configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var path = configuration["Log:Path"];
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Console.WriteLine($"File is not set or incorrectly set, defaulting to {_defaultPath}");
                path = _defaultPath;
            }

            int intervalDurationSec = int.Parse(configuration["Analytics:TimeSpanSec"]);

            Analytics analytics = new Analytics(
                int.Parse(configuration["Analytics:TopSections"]),
                intervalDurationSec);

            Alerting alerting = new Alerting(
                int.Parse(configuration["Alerting:HitsThreshold"]),
                int.Parse(configuration["Alerting:TimeSpanSec"]));
            # endregion

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
                        Console.WriteLine($"Now monitoring file {path}");
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
                                Log log = new Log(line);
                                analytics.Register(log);
                                alerting.Register(log);
                            }
                            if (logStopwatch.ElapsedMilliseconds > intervalDurationSec * 1000)
                            {
                                analytics.OnInterval();
                                logStopwatch.Restart();
                            }
                            // we check the alert state every second
                            if (alertStopwatch.ElapsedMilliseconds > 1000)
                            {
                                alerting.OnInterval();
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
