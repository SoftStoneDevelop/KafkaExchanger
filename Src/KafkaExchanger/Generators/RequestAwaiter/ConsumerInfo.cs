﻿using System.Text;

namespace KafkaExchanger.Generators.RequestAwaiter
{
    internal static class ConsumerInfo
    {
        public static void Append(
            StringBuilder builder
            )
        {
            builder.Append($@"
        public class {TypeName()}
        {{
            private {TypeName()}() {{ }}

            public {TypeName()}(
                string topicName,
                int[] partitions
                )
            {{
                {TopicName()} = topicName;
                Partitions = partitions;
            }}

            public string {TopicName()} {{ get; init; }}

            public int[] {Partitions()} {{ get; init; }}
        }}
");
        }

        public static string TypeFullName(KafkaExchanger.Datas.RequestAwaiter requestAwaiter)
        {
            return $"{requestAwaiter.TypeSymbol.Name}.{TypeName()}";
        }

        public static string TypeName()
        {
            return "ConsumerInfo";
        }

        public static string TopicName()
        {
            return "TopicName";
        }

        public static string Partitions()
        {
            return "Partitions";
        }
    }
}