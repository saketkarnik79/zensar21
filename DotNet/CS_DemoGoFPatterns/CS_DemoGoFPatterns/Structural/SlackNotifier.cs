using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal class SlackNotifier : NotifierDecorator
    {
        public SlackNotifier(INotifier wrappee) : base(wrappee)
        {
        }
        public override void Send(string message)
        {
            base.Send(message);
            Console.WriteLine($"Sending Slack notification: {message}");
        }
    }
}
