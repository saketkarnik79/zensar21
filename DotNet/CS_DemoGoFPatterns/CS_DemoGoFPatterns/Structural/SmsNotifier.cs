using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal class SmsNotifier : NotifierDecorator
    {
        public SmsNotifier(INotifier wrappee) : base(wrappee)
        {
        }
        public override void Send(string message)
        {
            base.Send(message);
            Console.WriteLine($"Sending SMS notification: {message}");
        }
    }
}
