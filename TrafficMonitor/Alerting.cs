using System;
using System.Collections.Generic;
using System.Linq;

namespace TrafficMonitor
{
    internal class Alerting
    {
        private readonly int _nbCallAlertThreshold = 10;
        private readonly int _durationAlertTimespanSec;
        internal int _currentLogCount = 0;
        internal Queue<int> _lastIntervalCallsCount = new Queue<int>();
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

        public bool inAlert()
        {
            return _inAlert;
        }

        public void OnInterval()
        {
            RegisterInterval();
            CheckAlertStatus();
        }

        private void RegisterInterval()
        {
            if (_lastIntervalCallsCount.Count >= _durationAlertTimespanSec)
            {
                _lastIntervalCallsCount.Dequeue();
            } 
            _lastIntervalCallsCount.Enqueue(_currentLogCount);
            _currentLogCount = 0;
        }

        private void CheckAlertStatus()
        {
            if (_lastIntervalCallsCount.Count >= _durationAlertTimespanSec)
            {
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
        }

        private void DisplayAlertStart(int avgNbHits, DateTime alertStartTime)
        {
            Console.WriteLine(new String('*', 50));
            Console.WriteLine($"High traffic generated an alert - hits per sec = {avgNbHits}, triggered at {alertStartTime}");
            Console.WriteLine(new String('*', 50));
        }

        private void DisplayAlertEnd(DateTime alertStartTime)
        {
            Console.WriteLine(new String('*', 50));
            Console.WriteLine($"Alert started at {alertStartTime} is now over ({DateTime.Now}).");
            Console.WriteLine(new String('*', 50));
        }
    }
}