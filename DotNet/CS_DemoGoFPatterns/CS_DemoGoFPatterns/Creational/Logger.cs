using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Creational
{
    internal sealed class Logger
    {
        private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());

        private Logger() { }

        public static Logger Instance => instance.Value;

        public void Log(string message)
        {
            Console.WriteLine($"Log: [{DateTime.Now}] {message}");
        }
    }
}
