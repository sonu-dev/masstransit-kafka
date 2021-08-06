using Confluent.Kafka;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer.Implementations
{
    public class SampleConsumer
    {
        public Task SubscribeAsync(string topic, Action<string> message)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "samplekafka",
                AutoOffsetReset = AutoOffsetReset.Earliest              
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            message(cr.Message.Value);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine(e);
                        }
                }
                catch (Exception)
                {
                    consumer.Close();
                }
            }

            return Task.CompletedTask;
        }
    }
}
