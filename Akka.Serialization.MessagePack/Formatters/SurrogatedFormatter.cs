using System;
using System.Collections.Concurrent;
using Akka.Actor;
using Akka.Util;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    class SurrogatedFormatterTools
    {
        internal static readonly ConcurrentDictionary<string, Type> deserTypeCache =
            new ConcurrentDictionary<string, Type>();
    }
    public class SurrogatedFormatter<T> : IMessagePackFormatter<T>
        where T : ISurrogated
    {
        private readonly ActorSystem _system;

        public SurrogatedFormatter(ActorSystem system)
        {
            _system = system;
        }

        public void Serialize(ref MessagePackWriter writer, T value,
            MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
            }
            else
            {
             
                //Rather than relying on TypelessFormatter, it is better for us
                //  to use the exact type formatter alongside manual serialization of type.
                //
                // TODO: See notes in deserialize about future improvements
                //  for well known types.
                writer.WriteArrayHeader(2);
                var surrogate = value.ToSurrogate(_system);
                var surT = surrogate.GetType();
                options.Resolver.GetFormatter<string>()
                    .Serialize(ref writer, surT.TypeQualifiedName(), options);
                MessagePackSerializer.Serialize(surT, ref writer,
                    surrogate, options);
            }
        }

        public T Deserialize(ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }
            else
            {
                //TODO: Look into a way to avoid creating a string here.
                //
                //TODO: In future, we can use switching on well-known types,
                //  alongside a 3-slot array with (int,null,realdata)
                // that would let us switch on int,
                // rather than sniff type below.
                reader.ReadArrayHeader();
                var typeStr = options.Resolver.GetFormatter<string>()
                    .Deserialize(ref reader, options);
                var deserType =
                    SurrogatedFormatterTools.deserTypeCache.GetOrAdd(typeStr,
                        t => Type.GetType(t, true));
                var surrogate =
                    (ISurrogate)MessagePackSerializer.Deserialize(deserType,
                        ref reader, options);
                return (T)surrogate.FromSurrogate(_system);
            }
        }
    }
}