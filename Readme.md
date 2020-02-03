## Improvements / Weaknesses

The hits are in the 10s time span matching the time they were handled. If we have too many hits for the program to handle everything in that timespan, or if we want to do it asynchronously, then the hits are going to be incorrectly managed in the statistics.
A good way of doing that would be to use the timestamp that's logged.