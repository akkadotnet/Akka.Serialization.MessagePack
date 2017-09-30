//-----------------------------------------------------------------------
// <copyright file="MsgPackSerializer.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Akka.Serialization.MessagePack.Resolvers;
using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;

namespace Akka.Serialization.MessagePack
{
    public sealed class MsgPackSerializer : Serializer
    {
        internal static AsyncLocal<ActorSystem> LocalSystem = new AsyncLocal<ActorSystem>();
        private readonly MsgPackSerializerSettings _settings;

        static MsgPackSerializer()
        {
            CompositeResolver.RegisterAndSetAsDefault(
#if SERIALIZATION
                SerializableResolver.Instance,
#endif
                AkkaResolver.Instance,
                ImmutableCollectionResolver.Instance,
                TypelessContractlessStandardResolver.Instance);
        }

        public MsgPackSerializer(ExtendedActorSystem system) : this(system, MsgPackSerializerSettings.Default)
        {
        }

        public MsgPackSerializer(ExtendedActorSystem system, Config config) 
            : this(system, MsgPackSerializerSettings.Create(config))
        {
        }

        public MsgPackSerializer(ExtendedActorSystem system, MsgPackSerializerSettings settings) : base(system)
        {
            LocalSystem.Value = system;
            _settings = settings;
        }

        public override byte[] ToBinary(object obj)
        {
            if (_settings.EnableLz4Compression)
            {
                return LZ4MessagePackSerializer.NonGeneric.Serialize(obj.GetType(), obj);
            }
            else
            {
                return MessagePackSerializer.NonGeneric.Serialize(obj.GetType(), obj);
            }
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            if (_settings.EnableLz4Compression)
            {
                return LZ4MessagePackSerializer.NonGeneric.Deserialize(type, bytes);
            }
            else
            {
                return MessagePackSerializer.NonGeneric.Deserialize(type, bytes);
            }
        }

        public override int Identifier => 150;

        public override bool IncludeManifest => true;
    }
}
