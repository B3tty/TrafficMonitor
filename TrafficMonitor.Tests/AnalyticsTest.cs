using System;
using Xunit;
using TrafficMonitor;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TrafficMonitor.Tests
{
    public class AnalyticsTest
    {
        # region Registering
        [Fact]
        public void Register_LogAddedToList_Test()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            analytics.Register(log);
            Assert.Single(analytics._currentLogs);
            Assert.Equal(log, analytics._currentLogs.First());
        }

        [Fact]
        public void Register_SectionMapUpdated_Test()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log{section="api"};
            analytics.Register(log);
            Assert.Single(analytics._sectionMap);
            Assert.Equal("api", analytics._sectionMap.Keys.ToList().First());
        }

        [Fact]
        public void Register_ResponseCodesUpdated_Test()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log{responseCode = 503, section = ""};
            analytics.Register(log);
            Assert.Single(analytics._responseCodeMap);
            Assert.Equal(5, analytics._responseCodeMap.Keys.ToList().First());
        }
        
        [Fact]
        public void Register_EmptySection_Test()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log{section=""};
            analytics.Register(log);
            Assert.Single(analytics._sectionMap);
            Assert.Equal("", analytics._sectionMap.Keys.ToList().First());
        }

        [Fact]
        public void Register_SentSize_Test()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log{section="", responseSize = 42};
            analytics.Register(log);
            Assert.Equal(42, analytics._sentBytes);
        }
        # endregion

        # region Flush

        [Fact]
        public void OnInterval_FlushCurrentQueue()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            analytics.Register(log);
            analytics.OnInterval();
            Assert.Empty(analytics._currentLogs);
        }

        [Fact]
        public void OnInterval_FlushSectionMap()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            analytics.Register(log);
            analytics.OnInterval();
            Assert.Empty(analytics._sectionMap);
        }

        [Fact]
        public void OnInterval_FlushCodeMap()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            analytics.Register(log);
            analytics.OnInterval();
            Assert.Empty(analytics._responseCodeMap);
        }

        [Fact]
        public void OnInterval_FlushSentSize()
        {
            Analytics analytics = new Analytics(1, 10);
            Log log = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            analytics.Register(log);
            analytics.OnInterval();
            Assert.Equal(0, analytics._sentBytes);
        }
        # endregion
    }
}