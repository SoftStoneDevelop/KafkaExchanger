﻿using Confluent.Kafka;
using KafkaExchanger.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestAwaiterConsole
{
    [RequestAwaiter(useLogger: false),
        Input(keyType: typeof(Null), valueType: typeof(string), new string[] { "RAResponder1" }),
        Input(keyType: typeof(Null), valueType: typeof(string), new string[] { "RAResponder2" }),
        Output(keyType: typeof(Null), valueType: typeof(string))
        ]
    public partial class RequestAwaiter
    {

    }

    [RequestAwaiter(useLogger: false),
        Input(keyType: typeof(Null), valueType: typeof(string), new string[] { "RAResponder1", "RAResponder2" }),
        Output(keyType: typeof(Null), valueType: typeof(string))
        ]
    public partial class RequestAwaiter2
    {

    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bootstrapServers = "localhost:9194, localhost:9294, localhost:9394";
            var input0Name = "RAInputSimple1";
            var input1Name = "RAInputSimple2";
            var outputName = "RAOutputSimple";

            var pool = new KafkaExchanger.Common.ProducerPoolNullString(
                3,
                bootstrapServers,
                static (config) =>
                {
                    config.LingerMs = 5;
                    config.SocketKeepaliveEnable = true;
                    config.AllowAutoCreateTopics = false;
                }
                );

            await using var reqAwaiter = await Scenario1(bootstrapServers, input0Name, input1Name, outputName, pool);
            //await using var reqAwaiter = Scenario2(bootstrapServers, input0Name, outputName, pool);

            int requests = 0;
            while ( true )
            {
                Console.WriteLine($"Write 'exit' for exit or press write 'requests count' for new pack");
                var read = Console.ReadLine();
                if(read == "exit")
                {
                    break;
                }

                requests = int.Parse(read);
                Console.WriteLine($"Start {requests} reqests");
                var iterationTimes = new long[20];
                for (int iteration = 0; iteration < 20; iteration++)
                {
                    Console.WriteLine($"Iteration {iteration}");
                    Stopwatch sw = Stopwatch.StartNew();
                    var tasks = new Task<long>[requests];

                    Parallel.For(0, requests, (index) => 
                    {
                        tasks[index] = Produce(reqAwaiter);
                        //tasks[i] = Produce2(reqAwaiter);
                    });

                    Console.WriteLine($"Create tasks: {sw.ElapsedMilliseconds} ms");
                    Task.WaitAll(tasks);
                    sw.Stop();
                    iterationTimes[iteration] = sw.ElapsedMilliseconds;
                    Console.WriteLine($"Requests sended: {requests}");
                    Console.WriteLine($"First pack Time: {sw.ElapsedMilliseconds} ms");
                    Console.WriteLine($"Per request: {sw.ElapsedMilliseconds / requests} ms");

                    var hashSet = new Dictionary<long, int>();
                    foreach (var task in tasks)
                    {
                        var executedTime = task.Result;
                        if (hashSet.TryGetValue(executedTime, out var internalCount))
                        {
                            hashSet[executedTime] = ++internalCount;
                        }
                        else
                        {
                            hashSet[executedTime] = 1;
                        }
                    }
                    Console.WriteLine($"Times:");

                    var pairs = hashSet.OrderBy(or => or.Key).ToList();
                    long startRange = pairs.First().Key;
                    long last = pairs.First().Key;
                    int count = pairs.First().Value;
                    var sb = new StringBuilder();

                    for (int i = 0; i < pairs.Count; i++)
                    {
                        KeyValuePair<long, int> item = pairs[i];
                        if (item.Key > startRange + 100)
                        {
                            sb.AppendLine($"({startRange} - {last}) ms: count {count}");
                            count = item.Value;
                            startRange = item.Key;
                            last = item.Key;
                        }
                        else if (i != 0)
                        {
                            count += item.Value;
                            last = item.Key;
                        }

                        if (i == pairs.Count - 1)
                        {
                            sb.AppendLine($"({startRange} - {last}) ms: count {count}");
                        }
                    }

                    Console.Write(sb.ToString());
                }

                Console.WriteLine("Iterations:");
                for (int i = 0; i < iterationTimes.Length; i++)
                {
                    var number = i + 1;
                    var time = iterationTimes[i];
                    Console.WriteLine($"Iteration {number}: {time}");
                }
            }
        }

        private static async Task<RequestAwaiter> Scenario1(
            string bootstrapServers,
            string input0Name,
            string input1Name,
            string outputName,
            KafkaExchanger.Common.ProducerPoolNullString pool
            )
        {
            var reqAwaiter = new RequestAwaiter();
            var reqAwaiterConfitg =
                new RequestAwaiter.Config(
                    groupId: "SimpleProduce",
                    bootstrapServers: bootstrapServers,
                    itemsInBucket: 1000,
                    inFlyBucketsLimit: 5,
                    addNewBucket: (bucketId, partitions0, topic0Name, partitions1, topic1Name) =>
                    {
                        return Task.CompletedTask;
                    },
                    bucketsCount: async (partitions0, topic0Name, partitions1, topic1Name) =>
                    {
                        return await Task.FromResult(2);
                    },
                    processors: new RequestAwaiter.ProcessorConfig[]
                    {
                        //From _inputSimpleTopic1
                        new RequestAwaiter.ProcessorConfig(
                            input0: new RequestAwaiter.ConsumerInfo(
                                topicName: input0Name,
                                partitions: new int[] { 0 }
                                ),
                            input1: new RequestAwaiter.ConsumerInfo(
                                topicName: input1Name,
                                partitions: new int[] { 0 }
                                ),
                            output0: new RequestAwaiter.ProducerInfo(outputName)
                            ),
                        new RequestAwaiter.ProcessorConfig(
                            input0: new RequestAwaiter.ConsumerInfo(
                                topicName: input0Name,
                                partitions: new int[] { 1 }
                                ),
                            input1: new RequestAwaiter.ConsumerInfo(
                                topicName: input1Name,
                                partitions: new int[] { 1 }
                                ),
                            output0:new RequestAwaiter.ProducerInfo(outputName)
                            ),
                        new RequestAwaiter.ProcessorConfig(
                            input0: new RequestAwaiter.ConsumerInfo(
                                topicName: input0Name,
                                partitions: new int[] { 2 }
                                ),
                            input1: new RequestAwaiter.ConsumerInfo(
                                topicName: input1Name,
                                partitions: new int[] { 2 }
                                ),
                            output0: new RequestAwaiter.ProducerInfo(outputName)
                            )
                    }
                    );
            Console.WriteLine("Start ReqAwaiter");
            await reqAwaiter.Setup(
                reqAwaiterConfitg,
                producerPool0: pool,
                currentBucketsCount: reqAwaiterConfitg.BucketsCount
                );

            reqAwaiter.Start(
                static (config) =>
                {
                    //config.MaxPollIntervalMs = 5_000;
                    //config.SessionTimeoutMs = 2_000;
                    config.SocketKeepaliveEnable = true;
                    config.AllowAutoCreateTopics = false;
                }
                );
            Console.WriteLine("ReqAwaiter started");

            return reqAwaiter;
        }

        private static async Task<RequestAwaiter2> Scenario2(
            string bootstrapServers,
            string input0Name,
            string outputName,
            KafkaExchanger.Common.ProducerPoolNullString pool
            )
        {
            var reqAwaiter = new RequestAwaiter2();
            var reqAwaiterConfitg =
                new RequestAwaiter2.Config(
                    groupId: "SimpleProduce",
                    bootstrapServers: bootstrapServers,
                    itemsInBucket: 20000,
                    inFlyBucketsLimit: 2,
                    addNewBucket: (bucketId, partitions0, topic1Name) =>
                    {
                        return Task.CompletedTask;
                    },
                    bucketsCount: async (partitions0, topic0Name) =>
                    {
                        return await Task.FromResult(2);
                    },
                    processors: new RequestAwaiter2.ProcessorConfig[]
                    {
                        //From _inputSimpleTopic1
                        new RequestAwaiter2.ProcessorConfig(
                            input0: new RequestAwaiter2.ConsumerInfo(
                                topicName: input0Name,
                                partitions: new int[] { 0 }
                                ),
                            output0: new RequestAwaiter2.ProducerInfo(outputName)
                            ),
                        new RequestAwaiter2.ProcessorConfig(
                            input0: new RequestAwaiter2.ConsumerInfo(
                                topicName: input0Name,
                                partitions: new int[] { 1 }
                                ),
                            output0:new RequestAwaiter2.ProducerInfo(outputName)
                            ),
                        new RequestAwaiter2.ProcessorConfig(
                            input0: new RequestAwaiter2.ConsumerInfo(
                                topicName: input0Name,
                                partitions: new int[] { 2 }
                                ),
                            output0: new RequestAwaiter2.ProducerInfo(outputName)
                            )
                    }
                    );
            Console.WriteLine("Start ReqAwaiter");
            await reqAwaiter.Setup(
                reqAwaiterConfitg,
                producerPool0: pool,
                currentBucketsCount: reqAwaiterConfitg.BucketsCount
                );

            reqAwaiter.Start(
                static (config) =>
                {
                    //config.MaxPollIntervalMs = 5_000;
                    //config.SessionTimeoutMs = 2_000;
                    config.SocketKeepaliveEnable = true;
                    config.AllowAutoCreateTopics = false;
                }
                );
            Console.WriteLine("ReqAwaiter started");

            return reqAwaiter;
        }

        private static async Task<long> Produce(RequestAwaiter reqAwaiter)
        {
            Stopwatch sb = Stopwatch.StartNew();
            using var result = await reqAwaiter.Produce("Hello").ConfigureAwait(false);
            return sb.ElapsedMilliseconds;
        }

        private static async Task<long> Produce2(RequestAwaiter2 reqAwaiter)
        {
            Stopwatch sb = Stopwatch.StartNew();
            using var result = await reqAwaiter.Produce("Hello").ConfigureAwait(false);
            return sb.ElapsedMilliseconds;
        }
    }
}