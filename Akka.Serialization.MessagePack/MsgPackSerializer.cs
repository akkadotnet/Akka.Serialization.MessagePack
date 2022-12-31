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
using MessagePack.Formatters;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;

namespace Akka.Serialization.MessagePack
{
    
    public sealed class MsgPackSerializer : Serializer
    {
        //internal static AsyncLocal<ActorSystem> LocalSystem = new AsyncLocal<ActorSystem>();
        private readonly MsgPackSerializerSettings _settings;
        private readonly IFormatterResolver _resolver;
        private readonly MessagePackSerializerOptions _serializerOptions;

        public MsgPackSerializer(ExtendedActorSystem system) : this(system, MsgPackSerializerSettings.Default)
        {
        }

        public MsgPackSerializer(ExtendedActorSystem system, Config config) 
            : this(system, MsgPackSerializerSettings.Create(config))
        {
        }

        public MsgPackSerializer(ExtendedActorSystem system, MsgPackSerializerSettings settings) : base(system)
        {
            _settings = settings;
            
            _resolver = CompositeResolver.Create(SerializableResolver.Instance,
                ImmutableCollectionResolver.Instance,
                new SurrogateFormatterResolver(base.system),
                TypelessContractlessStandardResolver.Instance);
            var opts =
                new MessagePackSerializerOptions(_resolver);
            _serializerOptions = _settings.EnableLz4Compression? opts.WithCompression(MessagePackCompression.Lz4Block): opts;
            
        }

        public override byte[] ToBinary(object obj)
        {
            //if (_settings.EnableLz4Compression)
            //{
            //    return LZ4MessagePackSerializer.NonGeneric.Serialize(obj.GetType(), obj);
            //}
            //else
            {
                return MessagePackSerializer.Serialize(obj.GetType(), obj,_serializerOptions);
            }
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            //if (_settings.EnableLz4Compression)
            //{
            //    return LZ4MessagePackSerializer.NonGeneric.Deserialize(type, bytes);
            //}
            //else
            {
                return MessagePackSerializer.Deserialize(type, bytes,_serializerOptions);
            }
        }

        public override int Identifier => 150;

        public override bool IncludeManifest => true;
    }
}
