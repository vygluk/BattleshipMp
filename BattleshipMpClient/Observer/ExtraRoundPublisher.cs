using System.Collections.Generic;

namespace BattleshipMpClient.Observer
{
    public class ExtraRoundPublisher
    {
        private readonly List<ExtraRoundSubscriber> _subscribers;

        public ExtraRoundPublisher()
        {
            _subscribers = new List<ExtraRoundSubscriber>();
        }

        public void Subscribe(ExtraRoundSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void Unsubscribe(ExtraRoundSubscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public void NotifySubscribers()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.UpdateRoundsInARow();
            }
        }
    }
}
