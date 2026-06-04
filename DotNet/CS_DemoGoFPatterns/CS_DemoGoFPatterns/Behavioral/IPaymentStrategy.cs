using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Behavioral
{
    internal interface IPaymentStrategy
    {
        void Pay(decimal amount);
    }
}
