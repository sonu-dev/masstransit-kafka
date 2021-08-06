using Confluent.Kafka;
using Producer.Contracts;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Producer.Implementations
{
    public class SampleProducer : IProducer
    {
        public async Task PublishAsync(string topic, string message)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "host1:9092,host2:9092",
                ClientId = Dns.GetHostName(),
            };
            using (var p = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                try
                {
                    var messg = new Message<Null, string> { Key = null, Value = message };
                    DeliveryResult<Null, string> reult = await p.ProduceAsync(topic, messg);
                    Console.WriteLine($"Delivered '{reult.Value}' to '{reult.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}
