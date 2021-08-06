using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Consumer
{
    public  class MassTransitKafkaRiderConsumer
    {
        private const string KafkaBroker = "localhost:9092";
        private const string Topic = "topic-sample";
        public static Task StartAsync()
        {
            var services = new ServiceCollection();
            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context)); // Using RabbitMQ for transport
                x.AddRider(riderConfig =>
                {
                    riderConfig.AddConsumer<KafkaMessageConsumer>();
                    riderConfig.UsingKafka((context, kafkaConfig) =>
                    {
                        var groupId = Guid.NewGuid().ToString(); 
                        kafkaConfig.Host(KafkaBroker);
                        kafkaConfig.TopicEndpoint<KafkaMessage>(Topic, groupId, topicConfig =>
                        {
                            topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                            topicConfig.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });
            return Task.CompletedTask;
        }

        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            public Task Consume(ConsumeContext<KafkaMessage> context)
            {
                Console.WriteLine(context.Message.Text);
                return Task.CompletedTask;
            }
        }

        public interface KafkaMessage
        {
            string Text { get; }
        }
    }
}
