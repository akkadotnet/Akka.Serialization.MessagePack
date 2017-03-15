using System;
using System.Collections.Generic;
using Akka.Actor;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class ActorPathResolver : IFormatterResolver
    {
        // Resolver should be singleton.
        public static IFormatterResolver Instance = new ActorPathResolver();

        ActorPathResolver()
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
                Formatter = (IMessagePackFormatter<T>)ActorPathResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class ActorPathResolverGetFormatterHelper
    {
        // If type is concrete type, use type-formatter map
        static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>()
        {
            {typeof(ActorPath), new ActorPathFormatter()},
            {typeof(ChildActorPath), new ChildActorPathFormatter()},
            {typeof(RootActorPath), new RootActorPathFormatter()}
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

    public static class ActorPathFormatterHelpers
    {
        public static int Serialize(ref byte[] bytes, int offset, ActorPath value, IFormatterResolver formatterResolver)
        {
            var startOffset = offset;
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.ToSerializationFormat());
            return offset - startOffset;
        }

        public static ActorPath Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var path = MessagePackBinary.ReadString(bytes, offset, out readSize);

            ActorPath actorPath;
            if (ActorPath.TryParse(path, out actorPath))
            {
                return actorPath;
            }

            return null;
        }
    }

    public class ActorPathFormatter : IMessagePackFormatter<ActorPath>
    {
        public int Serialize(ref byte[] bytes, int offset, ActorPath value, IFormatterResolver formatterResolver)
        {
            return ActorPathFormatterHelpers.Serialize(ref bytes, offset, value, formatterResolver);
        }

        public ActorPath Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return ActorPathFormatterHelpers.Deserialize(bytes, offset, formatterResolver, out readSize);
        }
    }

    public class RootActorPathFormatter : IMessagePackFormatter<RootActorPath>
    {
        public int Serialize(ref byte[] bytes, int offset, RootActorPath value, IFormatterResolver formatterResolver)
        {
            return ActorPathFormatterHelpers.Serialize(ref bytes, offset, value, formatterResolver);
        }

        public RootActorPath Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return (RootActorPath)ActorPathFormatterHelpers.Deserialize(bytes, offset, formatterResolver, out readSize);
        }
    }

    public class ChildActorPathFormatter : IMessagePackFormatter<ChildActorPath>
    {
        public int Serialize(ref byte[] bytes, int offset, ChildActorPath value, IFormatterResolver formatterResolver)
        {
            return ActorPathFormatterHelpers.Serialize(ref bytes, offset, value, formatterResolver);
        }

        public ChildActorPath Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return (ChildActorPath)ActorPathFormatterHelpers.Deserialize(bytes, offset, formatterResolver, out readSize);
        }
    }
}
