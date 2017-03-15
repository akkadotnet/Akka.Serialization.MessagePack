using System;
using Akka.Actor;
using MessagePack;
using MessagePack.Resolvers;

namespace Akka.Serialization.MsgPack
{
    public class MsgPackSerializer : Serializer
    {
        public MsgPackSerializer(ExtendedActorSystem system) : base(system)
        {

        }

        public override byte[] ToBinary(object obj)
        {
            return MessagePackSerializer.Serialize(obj, DynamicContractlessObjectResolver.Instance);
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            return MessagePackSerializer.NonGeneric.Deserialize(type, bytes, DynamicContractlessObjectResolver.Instance);
        }

        public override int Identifier => -10;

        public override bool IncludeManifest => false;
    }
}
