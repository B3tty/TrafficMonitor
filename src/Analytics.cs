using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrafficMonitor
{
    class Analytics
    {
        private readonly int _nbTopSectionsDisplay;
        private readonly int _logDurationSec;
        private List<Log> _currentLogs = new List<Log>();
        private Dictionary<int, int> _responseCodeMap = new Dictionary<int, int>();
        private Dictionary<String, int> _sectionMap = new Dictionary<string, int>();

        public Analytics(int nbTopSectionsDisplay, int logDurationSec)
        {
            _nbTopSectionsDisplay = nbTopSectionsDisplay;
            _logDurationSec = logDurationSec;
        }

        public void Register(Log log)
        {
            _currentLogs.Add(log);
            
            int responseCodeGroup = log.responseCode / 100;
            _responseCodeMap.TryGetValue(responseCodeGroup, out var codeCount); 
            _responseCodeMap[responseCodeGroup] = codeCount + 1;

            String section = log.resource.Split(' ')[1].Split('/')[1];
            _sectionMap.TryGetValue(section, out var sectionCount); 
            _sectionMap[section] = sectionCount + 1;
        }

        public void Flush()
        {
            _currentLogs = new List<Log>();
            _responseCodeMap = new Dictionary<int, int>();
            _sectionMap = new Dictionary<string, int>();
        }

        public void Display()
        {
            Console.WriteLine(new String('-', 50));

            Console.WriteLine($"Total number of calls in the last {_logDurationSec}s: {_currentLogs.Count}");
            Console.WriteLine();

            Console.WriteLine($"Top Sections Called: ");
            var sections = _sectionMap.ToList();
            sections.Sort((kvp1, kvp2) => -kvp1.Value.CompareTo(kvp2.Value));
            foreach(KeyValuePair<String, int> kvp in sections.Take(_nbTopSectionsDisplay))
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value} hits");
            }
            Console.WriteLine();

            Console.WriteLine($"Response Codes:");
            var responseCodes = _responseCodeMap.ToList();
            responseCodes.Sort((kvp1, kvp2) => kvp1.Key.CompareTo(kvp2.Key));
            foreach(KeyValuePair<int, int> kvp in responseCodes)
            {
                Console.WriteLine($"  {kvp.Key}xx: {kvp.Value}");
            }

            Console.WriteLine(new String('-', 50));
        }
    }
}