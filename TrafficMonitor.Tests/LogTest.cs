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
            Log logParsed = new Log("127.0.0.1 user-identifier betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            Log logExpected = new Log{
                ip="127.0.0.1",
                ident="user-identifier",
                user="betty",
                timestamp=new DateTime(2018, 05, 09, 16, 0, 42, DateTimeKind.Utc),
                request="POST /api/user HTTP/1.0",
                section="api",
                responseCode=503,
                responseSize=12
            };
            Assert.Equal(logExpected, logParsed);
        }

        [Fact]
        public void Ctor_IdentUnknown()
        {
            Log logParsed = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            Log logExpected = new Log{
                ip="127.0.0.1",
                ident="-",
                user="betty",
                timestamp=new DateTime(2018, 05, 09, 16, 0, 42, DateTimeKind.Utc),
                request="POST /api/user HTTP/1.0",
                section="api",
                responseCode=503,
                responseSize=12
            };
            Assert.Equal(logExpected, logParsed);
        }

        [Fact]
        public void Ctor_UserUnknown()
        {
            Log logParsed = new Log("127.0.0.1 user-identifier - [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            Log logExpected = new Log{
                ip="127.0.0.1",
                ident="user-identifier",
                user="-",
                timestamp=new DateTime(2018, 05, 09, 16, 0, 42, DateTimeKind.Utc),
                request="POST /api/user HTTP/1.0",
                section="api",
                responseCode=503,
                responseSize=12
            };
            Assert.Equal(logExpected, logParsed);
        }

        [Fact]
        public void Ctor_UserAndIdentUnknown()
        {
            Log logParsed = new Log("127.0.0.1 - - [09/May/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
            Log logExpected = new Log{
                ip="127.0.0.1",
                ident="-",
                user="-",
                timestamp=new DateTime(2018, 05, 09, 16, 0, 42, DateTimeKind.Utc),
                request="POST /api/user HTTP/1.0",
                section="api",
                responseCode=503,
                responseSize=12
            };
            Assert.Equal(logExpected, logParsed);
        }

        [Fact]
        public void Ctor_SectionParsing_LotsOfSections()
        {
            Log logParsed = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /a/b/c/d HTTP/1.0\" 503 12");
            Log logExpected = new Log{section="a"};
            Assert.Equal(logExpected.section, logParsed.section);
        }

        [Fact]
        public void Ctor_SectionParsing_NoSubSection()
        {
            Log logParsed = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST /a HTTP/1.0\" 503 12");
            Log logExpected = new Log{section="a"};
            Assert.Equal(logExpected.section, logParsed.section);
        }

        [Fact]
        public void Ctor_SectionParsing_NoSection()
        {
            // Shouldn't happen, we just don't want it to crash
            Log logParsed = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST HTTP/1.0\" 503 12");
        }

        [Fact]
        public void Ctor_SectionParsing_IncompleteResource()
        {
            Log logParsed = new Log("127.0.0.1 - betty [09/May/2018:16:00:42 +0000] \"POST\" 503 12");
            Log logExpected = new Log{section=""};
            Assert.Equal(logExpected.section, logParsed.section);
        }

        [Fact]
        public void Ctor_DifferentDateFormat()
        {
            // We're just checking that it does not crash
            Log logParsed = new Log("127.0.0.1 user-identifier betty [09/05/2018:16:00:42 +0000] \"POST /api/user HTTP/1.0\" 503 12");
        }
    }
}