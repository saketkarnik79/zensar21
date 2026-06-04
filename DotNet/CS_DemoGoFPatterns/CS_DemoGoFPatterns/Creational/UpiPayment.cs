using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Creational
{
    internal class UpiPayment : IPayment
    {
        public void Process()
        {
            Console.WriteLine("Processing UPI payment...");
        }
    }
}
