using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Behavioral
{
    internal class Subscriber : ISubscriber
    {
        public string Name { get; set; }

        public Subscriber(string name)
        {
            this.Name = name;
        }
        public void Update(string videoTitle)
        {
            Console.WriteLine($"{Name} received notification: New video uploaded - {videoTitle}");
        }
    }
}
