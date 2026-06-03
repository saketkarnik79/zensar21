using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoEvents
{
    internal class Process
    {
        public event Notify ProcessCompleted;

        public void StartProcess()
        {
            Console.WriteLine("Process Started!");
          
            // Some processing logic here
            Console.WriteLine("Process Completed!");
            
            // Raise the event
            ProcessCompleted?.Invoke();
        }
    }
}
