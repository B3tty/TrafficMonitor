using System;
using System.Text.RegularExpressions;

namespace TrafficMonitor
{
    public class Log
    {
        public String ip;
        public String user;
        public DateTime timestamp;
        public String resource;
        public int responseCode;
        public int responseSize;

        public Log(){}

        public Log(String line)
        {
            // 127.0.0.1 - betty [09/May/2018:16:00:43 +0000] "GET /report HTTP/1.0" 200 124
            // ip - user [timestamp] "resource" responseCode responseSize
            var r = new Regex(@"([0-9.]+)\s-\s(.+)\s\[(.+)\]\s""(.+)""\s([0-9]+)\s([0-9]+)", RegexOptions.IgnoreCase);
            var match = r.Match(line);
            if(match.Success)
            {
                ip = match.Groups[1].Value;
                user = match.Groups[2].Value;
                timestamp = DateTime.ParseExact(match.Groups[3].Value, "dd/MMM/yyyy:H:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture);
                resource = match.Groups[4].Value;
                responseCode = int.Parse(match.Groups[5].Value);
                responseSize = int.Parse(match.Groups[6].Value);
            }
        }
    }
}