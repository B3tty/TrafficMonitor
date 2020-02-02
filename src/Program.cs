namespace TrafficMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "example.log";
            while (true)
            {
                FileReader.ReadFile(path);
            }
        }
    }
}
