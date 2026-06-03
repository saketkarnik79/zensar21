using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoDelegates
{
    internal class NotificationService
    {
        public void SendEmailNotification(string message)
        {
            Console.WriteLine($"Email Notification: {message}");
        }

        public void SendSmsNotification(string message) 
        {
            Console.WriteLine("SMS Notification: " + message);
        }
    }
}