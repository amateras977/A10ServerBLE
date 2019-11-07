using System;
using System.Threading.Tasks;

namespace A10ServerBLE
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger.addLogger(new ConsoleLogger());

            var searcher = new TargetDeviceSearcher();
            var dispatcher = new TargetDeviceEventDispatcher();
            dispatcher.init(searcher);

            var server = new A10APIServer(dispatcher);


            searcher.Start();
            server.Start();

            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(3000));
            }
        }
    }

    class ConsoleLogger : ILogger
    {
        public void addLog(string record)
        {
            Console.WriteLine(record);
        }
    }
}
