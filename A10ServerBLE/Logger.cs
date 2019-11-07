using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A10ServerBLE
{
    public class Logger
    {
        private static List<ILogger> loggers = new List<ILogger>();

        public static void addLogger(ILogger logger)
        {
            loggers.Add(logger);
        }

        public static void log(string record)
        {
            foreach(var logger in  loggers){
                logger.addLog(record);
            }
        }
    }


    public interface ILogger
    {
        void addLog(string record);
    }
}
