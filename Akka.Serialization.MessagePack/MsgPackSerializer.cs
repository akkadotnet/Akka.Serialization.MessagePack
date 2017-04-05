using System;
using System.Runtime.Remoting.Messaging;
using Akka.Actor;
using Akka.Serialization.MessagePack.Resolvers;
using MessagePack;
using MessagePack.Resolvers;

namespace Akka.Serialization.MessagePack
{
    public class MsgPackSerializer : Serializer
    {
        static MsgPackSerializer()
        {
            CompositeResolver.RegisterAndSetAsDefault(
                ActorPathResolver.Instance,
                ActorRefResolver.Instance,
                ConfigResolver.Instance,
                PoisonPillResolver.Instance,
                KillResolver.Instance,
                PrimitiveObjectResolver.Instance,
                ContractlessStandardResolver.Instance);
        }

        public MsgPackSerializer(ExtendedActorSystem system) : base(system)
        {
            // TODO: hack to pass a context to formatters
            CallContext.SetData("ActorSystem", system);
        }

        public override byte[] ToBinary(object obj)
        {
            return MessagePackSerializer.NonGeneric.Serialize(obj.GetType(), obj);
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            return MessagePackSerializer.NonGeneric.Deserialize(type, bytes);
        }

        public override int Identifier => -10;

        public override bool IncludeManifest => false;
    }
}
