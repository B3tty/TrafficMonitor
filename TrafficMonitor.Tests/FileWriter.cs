using System;
using System.Collections.Generic;

namespace TrafficMonitor.Tests
{
    public class FileWriter
    {
            private readonly String _path = "test.log";
            /*
            127.0.0.1 - james [09/May/2018:16:00:39 +0000] "GET /report HTTP/1.0" 200 123
            127.0.0.1 - jill [09/May/2018:16:00:41 +0000] "GET /api/user HTTP/1.0" 200 234
            127.0.0.1 - frank [09/May/2018:16:00:42 +0000] "POST /api/user HTTP/1.0" 200 34
            127.0.0.1 - mary [09/May/2018:16:00:42 +0000] "POST /api/user HTTP/1.0" 503 12
            127.0.0.1 - betty [09/May/2018:16:00:43 +0000] "GET /report HTTP/1.0" 200 124
            */

            public FileWriter(String path)
            {
                _path = path;
            }

        public void WriteInFile(List<String> listLogs)
        {
            System.IO.File.WriteAllLines(_path, listLogs);
        }
    }
}