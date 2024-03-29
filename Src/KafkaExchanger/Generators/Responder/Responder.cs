﻿using KafkaExchanger.Datas;
using KafkaExchanger.Extensions;
using KafkaExchanger.Generators.RequestAwaiter;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaExchanger.Generators.Responder
{
    internal static class Responder
    {
        public static void Append(
            string assemblyName,
            KafkaExchanger.Datas.Responder responder,
            StringBuilder builder
            )
        {
            Start(builder, responder);

            Config.Append(builder, responder);
            ProcessorConfig.Append(builder, assemblyName, responder);
            ConsumerInfo.Append(builder, responder);

            InputMessage.Append(builder, assemblyName, responder);
            BaseInputMessage.Append(builder, assemblyName, responder);
            InputMessages.Append(builder, assemblyName, responder);

            OutputMessage.Append(builder, assemblyName, responder);
            OutputMessages.Append(builder, assemblyName, responder);

            ChannelInfo.Append(builder, assemblyName, responder);
            StartResponse.Append(builder, assemblyName, responder);
            SetOffsetResponse.Append(builder, assemblyName, responder);
            EndResponse.Append(builder, assemblyName, responder);

            ResponseProcess.Append(builder, assemblyName, responder);
            PartitionItem.Append(builder, assemblyName, responder);

            StartMethod(builder, responder);
            Setup(builder, assemblyName, responder);
            Push(builder, responder);
            StopAsync(builder);

            End(builder);
        }

        private static string _loggerFactory()
        {
            return "_loggerFactory";
        }

        private static string _items()
        {
            return "_items";
        }

        private static string _bootstrapServers()
        {
            return "_bootstrapServers";
        }

        private static string _groupId()
        {
            return "_groupId";
        }

        private static void Start(
            StringBuilder builder,
            KafkaExchanger.Datas.Responder responder
            )
        {
            builder.Append($@"
    {responder.TypeSymbol.DeclaredAccessibility.ToName()} partial class {responder.TypeSymbol.Name} : I{responder.TypeSymbol.Name}Responder
    {{
        {(responder.UseLogger ? $@"private readonly ILoggerFactory {_loggerFactory()};" : "")}
        private {PartitionItem.TypeFullName(responder)}[] {_items()};
        private string {_bootstrapServers()};
        private string {_groupId()};
        
        public {responder.TypeSymbol.Name}({(responder.UseLogger ? @"ILoggerFactory loggerFactory" : "")})
        {{
            {(responder.UseLogger ? $@"{_loggerFactory()} = loggerFactory;" : "")}
        }}
");
        }

        private static void StartMethod(
            StringBuilder builder,
            KafkaExchanger.Datas.Responder responder
            )
        {
            builder.Append($@"
        public void Start(
            Action<Confluent.Kafka.ConsumerConfig> changeConfig = null
            )
        {{
            for (int i = 0; i < {_items()}.Length; i++)
            {{
                {_items()}[i].Start(
                    {_bootstrapServers()},
                    {_groupId()},
                    changeConfig
                    );
            }}
        }}
");
        }

        private static void Setup(
            StringBuilder builder,
            string assemblyName,
            KafkaExchanger.Datas.Responder responder
            )
        {
            builder.Append($@"
        public async Task Setup(
            {Config.TypeFullName(responder)} config");

            for (int i = 0; i < responder.OutputDatas.Count; i++)
            {
                var outputData = responder.OutputDatas[i];
                builder.Append($@",
            KafkaExchanger.IProducerPool<{outputData.TypesPair}> {outputData.NameCamelCase}Pool");
            }
            builder.Append($@"
            )
        {{
            config.{Config.Validate()}();
            {_bootstrapServers()} = config.{Config.BootstrapServers()};
            {_groupId()} = config.{Config.GroupId()};
            {_items()} = new {PartitionItem.TypeFullName(responder)}[config.{Config.Processors()}.Length];
            for (int i = 0; i < config.{Config.Processors()}.Length; i++)
            {{
                var processorConfig = config.{Config.Processors()}[i];
                {_items()}[i] =
                    new {PartitionItem.TypeFullName(responder)}(
                        config.{Config.ServiceName()},
                        config.{Config.ItemsInBucket()},
                        config.{Config.InFlyLimit()},
                        config.{Config.AddNewBucket()},
");
            if (responder.UseLogger)
            {
                builder.Append($@"
                        {_loggerFactory()}.CreateLogger(config.{Config.GroupId()}),");
            }

            if (responder.AfterCommit)
            {
                builder.Append($@"
                        processorConfig.{ProcessorConfig.AfterCommit()},");
            }

            if (responder.CheckCurrentState)
            {
                builder.Append($@"
                        processorConfig.{ProcessorConfig.CheckCurrentState()},");
            }

            if (responder.AfterSend)
            {
                builder.Append($@"
                        processorConfig.{ProcessorConfig.AfterSend()},");
            }

            for (int i = 0; i < responder.InputDatas.Count; i++)
            {
                var inputData = responder.InputDatas[i];
                builder.Append($@"
                        processorConfig.{ProcessorConfig.ConsumerInfoName(inputData)}.{ConsumerInfo.TopicName()},
                        processorConfig.{ProcessorConfig.ConsumerInfoName(inputData)}.{ConsumerInfo.Partitions()},");
            }

            for (int i = 0; i < responder.OutputDatas.Count; i++)
            {
                var outputData = responder.OutputDatas[i];
                builder.Append($@"
                        {outputData.NameCamelCase}Pool,");
            }

            builder.Append($@"
                        processorConfig.{ProcessorConfig.CreateAnswer()}
                        );

                await {_items()}[i].Setup(config.{Config.BucketsCount()});
            }}
        }}
");
        }

        private static void Push(
            StringBuilder builder,
            Datas.Responder responder
            )
        {
            builder.Append($@"
        public void Push(
            int bucketId,
            string guid,
            int configId
            )
        {{
            var partitionItem = {_items()}[configId];
            partitionItem.PushMessage(bucketId, guid);
        }}
");
        }

        private static void StopAsync(
            StringBuilder builder
            )
        {
            builder.Append($@"
        public async Task StopAsync()
        {{
            if ({_items()} == null)
            {{
                return;
            }}

            foreach (var item in {_items()})
            {{
                await item.Stop();
            }}

            {_items()} = null;
        }}
");
        }

        private static void End(StringBuilder builder)
        {
            builder.Append($@"
}}
");
        }
    }
}
