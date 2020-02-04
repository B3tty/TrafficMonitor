# Traffic Monitor

## Goal of the Application

 This application is a simple console program that monitors HTTP traffic on a server.

Consume an actively written-to w3c-formatted HTTP access log (https://www.w3.org/Daemon/User/Config/Logging.html). It should default to reading /tmp/access.log and be overrideable
Example log lines:

```
127.0.0.1 - james [09/May/2018:16:00:39 +0000] "GET /report HTTP/1.0" 200 123
127.0.0.1 - jill [09/May/2018:16:00:41 +0000] "GET /api/user HTTP/1.0" 200 234
127.0.0.1 - frank [09/May/2018:16:00:42 +0000] "POST /api/user HTTP/1.0" 200 34
127.0.0.1 - mary [09/May/2018:16:00:42 +0000] "POST /api/user HTTP/1.0" 503 12
```

Functionalities:

* Display stats every 10s about the traffic during those 10s:
  - the sections of the web site with the most hits (A section is defined as being what's before the second '/' in the resource section of the log line. For example, the section for "/pages/create" is "/pages")
  - interesting summary statistics on the traffic as a whole: response codes proportions
* Make sure a user can keep the app running and monitor the log file continuously
* Whenever total traffic for the past 2 minutes exceeds a certain number on average, display a message saying that “High traffic generated an alert - hits = {value}, triggered at {time}”.
  The default threshold should be 10 requests per second, and is overridable
* Whenever the total traffic drops again below that value on average for the past 2 minutes, display another message detailing when the alert recovered
* Write a test for the alerting logic
* Explain how you’d improve on this application design


## Improvements / Weaknesses

The hits are in the 10s time span matching the time they were handled. If we have too many hits for the program to handle everything in that timespan, or if we want to do it asynchronously, then the hits are going to be incorrectly managed in the statistics.
A good way of doing that would be to use the timestamp that's logged.