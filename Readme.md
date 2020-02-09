# Traffic Monitor

## Goal of the Application

 This application is a simple console program that monitors HTTP traffic on a server.

Consume an actively written-to HTTP access log. It defaults to reading `/tmp/access.log` and is overrideable.

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

To run the project, execute the command `dotnet run --project TrafficMonitor/TrafficMonitor.csproj` from the solution directory.

In [the settings file](TrafficMonitor/appsettings.json) you can set:
* the timespan for which you want to receive monitoring info (currently 10s)
* the top number sections displayed
* the timespan to watch for alerting (currently 2mn)
* the threshold of hits per second that will trigger the alert (currently triggered at hits > 10/s)

### Running the tests

To run the tests, execute the command `dotnet test TrafficMonitor.Tests/TrafficMonitor.Tests.csproj` from the solution directory.

## Navigating the code

* The source code for the application is in the project `TrafficMonitor`, and the tests are in the project `TrafficMonitor.Tests`.
* `Program.cs` is where the `Main` method is: it opens the file, registers new log lines, and every relevant interval, calls the alerting and statistics mechanism.
* `Log.cs` is the expected `Log` structure, with the parsing mechanism.
* `Statistics.cs` is the class in charge of registering the relevant information and displaying the statistics at the end of the given interval.
* `Alerting.cs` registers the number of log lines (or hits) received every second, and recomputes the average of hits/s over the last 120s every second. It displays alert messages when that average becomes strictly higher than the set-up threshold, and when it becomes lower again.

## Weaknesses

### Time of the log lines

Currently the logs are treated as they come, and we use the moment they are written as an indication to know if they were written in the last 10s. We could use the timestamp of the log to do that.
* that would allow to display analytics for an old log file and see when alerts were raised and the traffic peaks long after it happened
* it can be problematic to do it that way if the logs are not written synchronously. For example if they are written by batch every 4s, this way of monitoring them will be incorrect.

### DateTime Format

When parsing the log lines, we assumed the date would be written in the default strftime format. If that's not the case, the date will not be parsed correctly. Since we don't use the date at the moment, it's not important, but it could be later if we decide to do something with it.


## Improvements

### Statistics

Currently the stats displayed about the logs are quite poor. With more precise specifications, we could display more interesting statistics about them. Need to check with product users what they need in order to enrich them. We register all that should be needed to do so, so it should be a simple modification of the `Analytics.Display` method.

### Display

Obviously for a frequent use, statistics would much easier to use if the display was improved. Some graphs for some interesting metrics could be useful.

### Scale

The scale is linked to the [time of the log lines](#time-of-the-log-lines) issue. If we start getting more log lines than we can handle in the 10s interval, we'll get incorrect metrics. If that starts to happen, either we handle asynchronously by managing them by the date written in them, or we'll need to multithread, and for the multi-threaded solution we need a strategy to regroup and merge the parsed lines and their corresponding stats.

