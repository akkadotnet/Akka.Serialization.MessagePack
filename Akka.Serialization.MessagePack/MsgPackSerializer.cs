using System;
using Akka.Actor;
using Akka.Serialization.MessagePack.Resolvers;
using Akka.Util;
using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;

namespace Akka.Serialization.MsgPack
{
    public class MsgPackSerializer : Serializer
    {
        static MsgPackSerializer()
        {
            CompositeResolver.RegisterAndSetAsDefault(
                // resolver custom types first
                ActorPathResolver.Instance,
                ActorRefResolver.Instance,
                ImmutableCollectionResolver.Instance,

                // finaly use standard resolver
                ContractlessStandardResolver.Instance);
        }

        public MsgPackSerializer(ExtendedActorSystem system) : base(system)
        {

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
