﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaExchanger.Generators.Responder
{
    internal static class Config
    {
        public static void Append(
            StringBuilder builder,
            Datas.Responder responder
            )
        {
            builder.Append($@"
        public class {TypeName()}
        {{
            private {TypeName()}()
            {{
            }}

            public {TypeName()}(
                string groupId,
                string serviceName,
                string bootstrapServers,
                int itemsInBucket,
                int inFlyLimit,
                {responder.AddNewBucketFuncType()} addNewBucket,
                {ProcessorConfig.TypeFullName(responder)}[] processors
                )
            {{
                {GroupId()} = groupId;
                {ServiceName()} = serviceName;
                {BootstrapServers()} = bootstrapServers;
                {ItemsInBucket()} = itemsInBucket;
                {InFlyLimit()} = inFlyLimit;
                {AddNewBucket()} = addNewBucket;
                {Processors()} = processors;
            }}

            public string {GroupId()} {{ get; init; }}

            public string {ServiceName()} {{ get; init; }}

            public string {BootstrapServers()} {{ get; init; }}

            public int {ItemsInBucket()} {{ get; init; }}

            public int {InFlyLimit()} {{ get; init; }}

            public {responder.AddNewBucketFuncType()} {AddNewBucket()} {{ get; init; }}

            public {ProcessorConfig.TypeFullName(responder)}[] {Processors()} {{ get; init; }}
        }}
");
        }

        public static string ItemsInBucket()
        {
            return "ItemsInBucket";
        }

        public static string InFlyLimit()
        {
            return "InFlyLimit";
        }

        public static string AddNewBucket()
        {
            return "AddNewBucket";
        }

        public static string TypeFullName(KafkaExchanger.Datas.Responder responder)
        {
            return $"{responder.TypeSymbol.Name}.{TypeName()}";
        }

        public static string TypeName()
        {
            return "Config";
        }

        public static string Processors()
        {
            return "Processors";
        }

        public static string BootstrapServers()
        {
            return "BootstrapServers";
        }

        public static string ServiceName()
        {
            return "ServiceName";
        }

        public static string GroupId()
        {
            return "GroupId";
        }
    }
}