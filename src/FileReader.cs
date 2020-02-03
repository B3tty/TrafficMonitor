using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TrafficMonitor
{
    class FileReader
    {
        public static void ReadFile(String path)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Analytics analytics = new Analytics();

            try
            {   
                // Open the text file using a stream reader.
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs))
                    {
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
                                Console.WriteLine(line);
                                analytics.Account(log);
                            }
                            if (stopwatch.ElapsedMilliseconds > 10000)
                            {
                                analytics.Display();
                                analytics.Check2mnSpan();
                                analytics.Flush();
                                stopwatch.Restart();
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
