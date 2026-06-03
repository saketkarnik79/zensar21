using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoEvents
{
    internal class OrderEventArgs: EventArgs
    {
        public string OrderID { get; set; }
        public decimal Amount { get; set; }
    }
}
