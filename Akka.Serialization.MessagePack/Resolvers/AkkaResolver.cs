//-----------------------------------------------------------------------
// <copyright file="AkkaResolver.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Util.Internal;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class AkkaResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new AkkaResolver();
        private AkkaResolver() { }
        public IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.Formatter;

        private static class FormatterCache<T>
        {
            public static IMessagePackFormatter<T> Formatter { get; }
            static FormatterCache() => Formatter = (IMessagePackFormatter<T>)AkkaResolverGetFormatterHelper.GetFormatter(typeof(T));
        }
    }

    internal static class AkkaResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>
        {
            {typeof(ActorPath), new ActorPathFormatter<ActorPath>()},
            {typeof(ChildActorPath), new ActorPathFormatter<ChildActorPath>()},
            {typeof(RootActorPath), new ActorPathFormatter<RootActorPath>()},
            {typeof(IActorRef), new ActorRefFormatter<IActorRef>()},
            {typeof(IInternalActorRef), new ActorRefFormatter<IInternalActorRef>()},
            {typeof(RepointableActorRef), new ActorRefFormatter<RepointableActorRef>()}
        };

        internal static object GetFormatter(Type t) => FormatterMap.TryGetValue(t, out var formatter) ? formatter : null;
    }

    // IActorRef
    public class ActorRefFormatter<T> : IMessagePackFormatter<T> where T : IActorRef
    {
        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteString(ref bytes, offset, Serialization.SerializedActorPath(value));
            return offset - startOffset;
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default(T);
            }

            int startOffset = offset;

            var path = MessagePackBinary.ReadString(bytes, offset, out readSize);

            var system = MsgPackSerializer.LocalSystem.Value.AsInstanceOf<ExtendedActorSystem>();
            if (system == null)
                return default(T);

            readSize = offset - startOffset;
            return (T)system.Provider.ResolveActorRef(path);
        }
    }

    // ActorPath
    public class ActorPathFormatter<T> : IMessagePackFormatter<T> where T : ActorPath
    {
        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.ToSerializationFormat());
            return offset - startOffset;
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            int startOffset = offset;

            var path = MessagePackBinary.ReadString(bytes, offset, out readSize);

            readSize = offset - startOffset;
            return ActorPath.TryParse(path, out var actorPath) ? (T)actorPath : null;
        }
    }
}
