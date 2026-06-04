using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal class EmailNotifier : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"Email sent: {message}");
        }
    }
}
