using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Akka.Actor;
using Akka.Actor.Internal;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class ActorRefResolver : IFormatterResolver
    {
        // Resolver should be singleton.
        public static IFormatterResolver Instance = new ActorRefResolver();

        ActorRefResolver()
        {
        }

        // GetFormatter<T>'s get cost should be minimized so use type cache.
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            // generic's static constructor should be minimized for reduce type generation size!
            // use outer helper method.
            static FormatterCache()
            {
                Formatter = (IMessagePackFormatter<T>)ActorRefResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class ActorRefResolverGetFormatterHelper
    {
        // If type is concrete type, use type-formatter map
        static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>()
        {
            {typeof(IActorRef), new ActorRefFormatter<IActorRef>()},
            {typeof(IInternalActorRef), new ActorRefFormatter<IInternalActorRef>()},
            {typeof(RepointableActorRef), new ActorRefFormatter<RepointableActorRef>()}
        };

        internal static object GetFormatter(Type t)
        {
            object formatter;
            if (FormatterMap.TryGetValue(t, out formatter))
            {
                return formatter;
            }

            // If type can not get, must return null for fallback mecanism.
            return null;
        }
    }

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

            var path = MessagePackBinary.ReadString(bytes, offset, out readSize);

            var system = (ActorSystemImpl)CallContext.GetData("ActorSystem");
            if (system == null)
                return default(T);

            return (T)system.Provider.ResolveActorRef(path);
        }
    }
}
