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
        internal List<Log> _currentLogs = new List<Log>();
        internal Dictionary<int, int> _responseCodeMap = new Dictionary<int, int>();
        internal Dictionary<String, int> _sectionMap = new Dictionary<string, int>();
        internal int _sentBytes = 0;

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

            _sectionMap.TryGetValue(log.section, out var sectionCount); 
            _sectionMap[log.section] = sectionCount + 1;
            _sentBytes += log.responseSize;
        }

        public void OnInterval()
        {
            Display();
            Flush();
        }

        public void Flush()
        {
            _currentLogs = new List<Log>();
            _responseCodeMap = new Dictionary<int, int>();
            _sectionMap = new Dictionary<string, int>();
            _sentBytes = 0;
        }

        public void Display()
        {
            Console.WriteLine(new String('-', 50));

            Console.WriteLine($"Total number of calls in the last {_logDurationSec}s: {_currentLogs.Count}");
            Console.WriteLine();

            Console.WriteLine($"Top {_nbTopSectionsDisplay} Sections Called: ");
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

            Console.WriteLine($"Debit - Sent bytes in the last {_logDurationSec}s: {_sentBytes}");

            Console.WriteLine();
            Console.WriteLine(new String('-', 50));
        }
    }
}