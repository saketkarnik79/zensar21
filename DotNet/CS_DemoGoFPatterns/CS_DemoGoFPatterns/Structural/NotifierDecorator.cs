using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal abstract class NotifierDecorator : INotifier
    {
        protected readonly INotifier _wrappee;

        public NotifierDecorator(INotifier wrppee)
        {
            _wrappee = wrppee;
        }
        public virtual void Send(string message)
        {
            _wrappee.Send(message);
        }
    }
}