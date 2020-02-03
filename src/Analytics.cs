using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrafficMonitor
{
    class Analytics
    {
        private static int nbCallAlertThreshold = 10;
        private static int nbTopSectionsDisplay = 5;
        private List<Log> currentLogs = new List<Log>();
        private Dictionary<int, int> responseCodeMap = new Dictionary<int, int>();
        private Dictionary<String, int> sectionMap = new Dictionary<string, int>();
        private Stack<int> last2mnCallsCount = new Stack<int>();
        private bool inAlert = false;
        private DateTime alertStart;

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

        public void Check2mnSpan()
        {
            if (last2mnCallsCount.Count < 12)
            {
                last2mnCallsCount.Push(currentLogs.Count);
                return;
            } 
            last2mnCallsCount.Pop();
            last2mnCallsCount.Push(currentLogs.Count);
            var avgNbCallsPerSec = last2mnCallsCount.Sum() / last2mnCallsCount.Count;
            if (avgNbCallsPerSec > nbCallAlertThreshold && !inAlert)
            {
                inAlert = true;
                alertStart = DateTime.Now;
                DisplayAlertStart(avgNbCallsPerSec, alertStart);
                // “High traffic generated an alert - hits = {value}, triggered at {time}”
            }
            if (avgNbCallsPerSec < nbCallAlertThreshold && inAlert)
            {
                inAlert = false;
                DisplayAlertEnd(alertStart);
            }
        }

        public void Flush()
        {
            currentLogs = new List<Log>();
            responseCodeMap = new Dictionary<int, int>();
            sectionMap = new Dictionary<string, int>();
        }

        public void DisplayAlertStart(int avgNbHits, DateTime alertStartTime)
        {
            Console.WriteLine(new String('*', 50));
            Console.WriteLine($"High traffic generated an alert - hits per sec = {avgNbHits}, triggered at {alertStartTime}");
            Console.WriteLine(new String('*', 50));
        }

        public void DisplayAlertEnd(DateTime alertStartTime)
        {
            Console.WriteLine(new String('*', 50));
            Console.WriteLine($"Alert started at {alertStartTime} is now over ({DateTime.Now}).");
            Console.WriteLine(new String('*', 50));
        }

        public void Display()
        {
            Console.WriteLine(new String('-', 50));

            Console.WriteLine($"Total number of calls in the last 10s: {currentLogs.Count}");
            Console.WriteLine();

            Console.WriteLine($"Top Sections Called: ");
            var topSections = sectionMap.ToList();
            topSections.Sort((kvp1, kvp2) => kvp1.Value.CompareTo(kvp2.Value));
            foreach(KeyValuePair<String, int> kvp in topSections.Take(nbTopSectionsDisplay))
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value} hits");
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