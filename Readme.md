# Traffic Monitor

## Goal of the Application

 This application is a simple console program that monitors HTTP traffic on a server.

Consume an actively written-to HTTP access log. It defaults to reading /tmp/access.log and is overrideable
Example log lines:

```
127.1.1.1 - jack [23/May/2018:16:00:39 +0000] "GET /experiment HTTP/1.0" 200 134
127.1.1.1 user-identifier lucy [23/May/2018:16:00:41 +0000] "GET /api/variation HTTP/1.0" 200 245
127.1.1.1 - betty [23/May/2018:16:00:42 +0000] "POST /api/variation HTTP/1.0" 503 21
```

Functionalities:

* Display stats every 10s about the traffic during those 10s:
  - the sections of the web site with the most hits (A section is defined as being what's before the second '/' in the resource section of the log line. For example, the section for "/pages/create" is "/pages")
  - interesting summary statistics on the traffic as a whole: response codes proportions
* Make sure a user can keep the app running and monitor the log file continuously
* Whenever total traffic for the past 2 minutes exceeds a certain number on average, display a message saying that “High traffic generated an alert - hits = {value}, triggered at {time}”.
  The default threshold should be 10 requests per second, and is overridable
* Whenever the total traffic drops again below that value on average for the past 2 minutes, display another message detailing when the alert recovered

## Execution



## Weaknesses

Currently the logs are treated as they come, and we use the moment they are written as an indication to know if they were written in the last 10s. We could use the timestamp of the log to do that.
* that would allow to display analytics for an old log file and see where alerts were raised and the traffics peaks long after it happened
* it can be problematic to do it that way if the logs are not written synchronously. For example if they are written by batch every 4s, this way of monitoring them will be incorrect.

When parsing the log lines, we assumed the date would be written in the default strftime format. If that's not the case, the date will not be parsed correctly. Since we don't use the date at the moment, it's not important, but it could be later if we decide to do something with it.


## Improvements


