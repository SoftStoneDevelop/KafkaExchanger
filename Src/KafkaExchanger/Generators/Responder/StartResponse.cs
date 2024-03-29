﻿using System.Text;

namespace KafkaExchanger.Generators.Responder
{
    internal static class StartResponse
    {
        public static void Append(
            StringBuilder builder,
            string assemblyName,
            Datas.Responder responder
            )
        {
            builder.Append($@"
        public class {TypeName()} : {ChannelInfo.TypeFullName(responder)}
        {{
            public {KafkaExchanger.Generators.Responder.ResponseProcess.TypeFullName(responder)} ResponseProcess {{ get; set; }}
        }}
");
        }

        public static string TypeFullName(KafkaExchanger.Datas.Responder responder)
        {
            return $"{responder.TypeSymbol.Name}.{TypeName()}";
        }

        public static string TypeName()
        {
            return "StartResponse";
        }

        public static string ResponseProcess()
        {
            return "ResponseProcess";
        }
    }
}