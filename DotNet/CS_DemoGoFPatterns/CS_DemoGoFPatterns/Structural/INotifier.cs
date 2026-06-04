using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Structural
{
    internal interface INotifier
    {
        void Send(string message);
    }
}
