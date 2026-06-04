using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Behavioral
{
    internal class Channel
    {
        private List<ISubscriber> subscribers = new();

        public void Subscribe(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void UploadVideo(string videoTitle)
        {
            Console.WriteLine($"Channel: New video uploaded - {videoTitle}");
            NotifySubscribers(videoTitle);
        }

        private void NotifySubscribers(string videoTitle)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.Update(videoTitle);
            }
        }
    }
}
