using System;
using System.Threading.Tasks;

namespace Consumer.Contracts
{
    public interface IConsumer
    {
        Task SubscribeAsync(string topic, Action<string> message);
    }
}
