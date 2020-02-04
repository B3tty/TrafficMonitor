using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrafficMonitor
{
    class Alerting
    {
        private readonly int _nbCallAlertThreshold = 10;
        private readonly int _durationAlertTimespanSec;
        private int _currentLogCount = 0;
        private Queue<int> _lastIntervalCallsCount = new Queue<int>();
        private bool _inAlert = false;
        private DateTime _alertStart;

        public Alerting(int nbCallAlertThreshold, int durationAlertTimespanSec)
        {
            _nbCallAlertThreshold = nbCallAlertThreshold;
            _durationAlertTimespanSec = durationAlertTimespanSec;
        }

        public void Register(Log log)
        {
            _currentLogCount += 1;
        }

        public void CheckAlertStatus()
        {
            if (_lastIntervalCallsCount.Count < _durationAlertTimespanSec)
            {
                _lastIntervalCallsCount.Enqueue(_currentLogCount);
                return;
            } 
            _lastIntervalCallsCount.Dequeue();
            _lastIntervalCallsCount.Enqueue(_currentLogCount);
            _currentLogCount = 0;
            var avgNbCallsPerSec = _lastIntervalCallsCount.Sum() / _lastIntervalCallsCount.Count;
            if (avgNbCallsPerSec > _nbCallAlertThreshold && !_inAlert)
            {
                _inAlert = true;
                _alertStart = DateTime.Now;
                DisplayAlertStart(avgNbCallsPerSec, _alertStart);
                // “High traffic generated an alert - hits = {value}, triggered at {time}”
            }
            if (avgNbCallsPerSec < _nbCallAlertThreshold && _inAlert)
            {
                _inAlert = false;
                DisplayAlertEnd(_alertStart);
            }
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
    }
}