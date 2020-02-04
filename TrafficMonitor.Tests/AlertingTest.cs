using System;
using Xunit;
using TrafficMonitor;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace TrafficMonitor.Tests
{
    public class AlertingTest
    {
        # region Registering
        [Fact]
        public void Register_LogRegistered_Test()
        {
            Alerting alerting = new Alerting(nbCallAlertThreshold:1, durationAlertTimespanSec:3);
            Log log = new Log();
            alerting.Register(log);
            Assert.Equal(1, alerting._currentLogCount);
        }

        [Fact]
        public void OnInterval_IntervalRegistered_Test()
        {
            Alerting alerting = new Alerting(nbCallAlertThreshold:1, durationAlertTimespanSec:3);
            Log log = new Log();
            alerting.Register(log);
            alerting.OnInterval();
            Assert.Single(alerting._lastIntervalCallsCount);
        }

        [Fact]
        public void OnInterval_CurrentQueueFlushed_Test()
        {
            Alerting alerting = new Alerting(nbCallAlertThreshold:1, durationAlertTimespanSec:3);
            Log log = new Log();
            alerting.Register(log);
            alerting.OnInterval();
            Assert.Equal(0, alerting._currentLogCount);
        }
        # endregion

        # region Alert Raising
        [Fact]
        public void OnInterval_alertRaisedOnTime_Test()
        {
            Alerting alerting = new Alerting(nbCallAlertThreshold:1, durationAlertTimespanSec:3);
            Log log = new Log();
            for (int i=1; i<4; i++)
            {
                alerting.Register(log);
                alerting.Register(log);
                alerting.OnInterval();
            }
            Assert.True(alerting.inAlert());
        }

        [Fact]
        public void OnInterval_alertNotRaisedTooSoon_Test()
        {
            Alerting alerting = new Alerting(nbCallAlertThreshold:1, durationAlertTimespanSec:3);
            Log log = new Log();
            for (int i=1; i<3; i++)
            {
                alerting.Register(log);
                alerting.Register(log);
                alerting.OnInterval();
            }
            // average calls/sec = 4/3 but we haven't reach the time yet
            Assert.False(alerting.inAlert());
        }

        [Fact]
        public void OnInterval_alertUnraised_Test()
        {
            Alerting alerting = new Alerting(nbCallAlertThreshold:1, durationAlertTimespanSec:3);
            Log log = new Log();
            for (int i=1; i<4; i++)
            {
                alerting.Register(log);
                alerting.Register(log);
                alerting.OnInterval();
            }
            alerting.OnInterval();
            alerting.OnInterval();
            Assert.False(alerting.inAlert());
        }
        # endregion
    }
}