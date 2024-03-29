// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: responder.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace GrcpService {
  public static partial class RespondService
  {
    static readonly string __ServiceName = "GrcpService.RespondService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrcpService.HelloRequest> __Marshaller_GrcpService_HelloRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrcpService.HelloRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrcpService.HelloResponse> __Marshaller_GrcpService_HelloResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrcpService.HelloResponse.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrcpService.HelloStreamRequest> __Marshaller_GrcpService_HelloStreamRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrcpService.HelloStreamRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::GrcpService.HelloStreamResponse> __Marshaller_GrcpService_HelloStreamResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::GrcpService.HelloStreamResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::GrcpService.HelloRequest, global::GrcpService.HelloResponse> __Method_Hello = new grpc::Method<global::GrcpService.HelloRequest, global::GrcpService.HelloResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "Hello",
        __Marshaller_GrcpService_HelloRequest,
        __Marshaller_GrcpService_HelloResponse);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::GrcpService.HelloStreamRequest, global::GrcpService.HelloStreamResponse> __Method_HelloStream = new grpc::Method<global::GrcpService.HelloStreamRequest, global::GrcpService.HelloStreamResponse>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "HelloStream",
        __Marshaller_GrcpService_HelloStreamRequest,
        __Marshaller_GrcpService_HelloStreamResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::GrcpService.ResponderReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of RespondService</summary>
    [grpc::BindServiceMethod(typeof(RespondService), "BindService")]
    public abstract partial class RespondServiceBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::GrcpService.HelloResponse> Hello(global::GrcpService.HelloRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task HelloStream(grpc::IAsyncStreamReader<global::GrcpService.HelloStreamRequest> requestStream, grpc::IServerStreamWriter<global::GrcpService.HelloStreamResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(RespondServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Hello, serviceImpl.Hello)
          .AddMethod(__Method_HelloStream, serviceImpl.HelloStream).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, RespondServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_Hello, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::GrcpService.HelloRequest, global::GrcpService.HelloResponse>(serviceImpl.Hello));
      serviceBinder.AddMethod(__Method_HelloStream, serviceImpl == null ? null : new grpc::DuplexStreamingServerMethod<global::GrcpService.HelloStreamRequest, global::GrcpService.HelloStreamResponse>(serviceImpl.HelloStream));
    }

  }
}
#endregion
