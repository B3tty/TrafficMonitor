using System;
using Xunit;
using TrafficMonitor;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TrafficMonitor.Tests
{
    public class LogTest
    {
        [Fact]
        public void Ctor_CorrectBuildFromText()
        {
            Log logParsed = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            Log logExpected = new Log{
                ip="127.0.0.1",
                user="betty",
                //timestamp=DateTime.ParseExact("09/May/2018:16:00:42 +0000", "dd/MMM/yyyy:H:mm:ss zzz", System.Globalization.CultureInfo.InvariantCulture),
                timestamp=new DateTime(2018, 05, 09, 16, 0, 42, DateTimeKind.Utc),
                resource="POST /api/user HTTP/1.0",
                section="api",
                responseCode=503,
                responseSize=12
            };
            Assert.Equal(logExpected.timestamp, logParsed.timestamp);
        }
    }
}