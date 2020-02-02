using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrafficMonitor
{
    class Analytics
    {
        private List<Log> currentLogs = new List<Log>();
        private Dictionary<int, int> responseCodeMap = new Dictionary<int, int>();
        private Dictionary<String, int> sectionMap = new Dictionary<string, int>();

        public void Account(Log log)
        {
            currentLogs.Add(log);
            
            int responseCodeGroup = log.responseCode / 100;
            responseCodeMap.TryGetValue(responseCodeGroup, out var codeCount); 
            responseCodeMap[responseCodeGroup] = codeCount + 1;

            String section = log.resource.Split(' ')[1].Split('/')[1];
            sectionMap.TryGetValue(section, out var sectionCount); 
            sectionMap[section] = sectionCount + 1;
        }

        public void Flush()
        {
            currentLogs = new List<Log>();
            responseCodeMap = new Dictionary<int, int>();
            sectionMap = new Dictionary<string, int>();
        }

        public void Display()
        {
            Console.WriteLine(new String('-', 50));

            Console.WriteLine($"Total number of calls in the last 10s: {currentLogs.Count}");
            Console.WriteLine();

            Console.WriteLine($"Sections called: ");
            foreach(String key in sectionMap.Keys)
            {
                Console.WriteLine("  " + key);
            }
            Console.WriteLine();

            Console.WriteLine($"Response Codes:");
            foreach(KeyValuePair<int, int> kvp in responseCodeMap)
            {
                Console.WriteLine($"  {kvp.Key}xx: {kvp.Value}");
            }

            Console.WriteLine(new String('-', 50));
        }
    }
}