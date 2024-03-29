﻿using Confluent.Kafka;
using KafkaExchanger.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Responder0Console
{
    [Responder(useLogger: false),
        Input(keyType: typeof(Null), valueType: typeof(GrcpService.HelloRequest)),
        Output(keyType: typeof(Null), valueType: typeof(GrcpService.HelloResponse))
        ]
    public partial class ResponderOneToOneSimple
    {

    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bootstrapServers = "localhost:9194, localhost:9294, localhost:9394";
            var inputName = "RAOutputSimple";
            var responderName = "RAResponder1";

            var responder1 = new ResponderOneToOneSimple();
            var partitions = 3;
            var processors = new ResponderOneToOneSimple.ProcessorConfig[partitions];
            for (int i = 0; i < processors.Length; i++)
            {
                processors[i] =
                    new ResponderOneToOneSimple.ProcessorConfig(
                        createAnswer: (input, s) =>
                        {
                            Console.WriteLine($"Input: {input.Input0Message.Value}");
                            var result = new ResponderOneToOneSimple.OutputMessage()
                            {
                                Output0Message = new ResponderOneToOneSimple.Output0Message()
                                {
                                    Value = new GrcpService.HelloResponse { Text = $"0: Answer {input.Input0Message.Value}" }
                                }
                            };

                            return ValueTask.FromResult(result);
                        },
                        input0: new ResponderOneToOneSimple.ConsumerInfo(inputName, new int[] { i })
                        );
            }

            var responder1Config =
                new ResponderOneToOneSimple.Config(
                groupId: responderName,
                serviceName: responderName,
                bootstrapServers: bootstrapServers,
                itemsInBucket: 1000,
                inFlyLimit: 5,
                addNewBucket: static (bucketId, partitions, topicName) => { return ValueTask.CompletedTask; },
                bucketsCount: (partitions, topicName) => { return ValueTask.FromResult(5); },
                processors: processors
                );

            var pool = new KafkaExchanger.ProducerPool<Confluent.Kafka.Null, byte[]>(
                new HashSet<string> { "Responder0Console0", "Responder0Console1" },
                bootstrapServers,
                changeConfig: static (config) =>
                {
                    config.LingerMs = 2;
                    config.SocketKeepaliveEnable = true;
                    config.AllowAutoCreateTopics = false;
                }
                );

            Console.WriteLine("Start Responder");
            await responder1.Setup(config: responder1Config, output0Pool: pool);
            responder1.Start();
            Console.WriteLine("Responder started");

            while (true)
            {
                var read = Console.ReadLine();
                if (read == "exit")
                {
                    break;
                }
            }

            await responder1.StopAsync();
            await pool.DisposeAsync();
        }
    }
}