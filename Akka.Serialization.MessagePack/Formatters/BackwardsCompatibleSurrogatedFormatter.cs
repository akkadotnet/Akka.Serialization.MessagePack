using System.Buffers;
using System.Runtime.CompilerServices;
using Akka.Actor;
using Akka.Util;
using CommunityToolkit.HighPerformance.Buffers;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    /// <summary>
    /// A Formatter that handles converting Surrogated types to/from their
    /// Surrogate representation 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// We more or less handle things in the current (default) case by the following:
    /// 
    /// When a Surrogated type gets serialized, we get the surrogate, Find it's type,
    /// Then write the unversioned type name as well as the serialized Surrogate.
    /// Thankfully, MessagePack handles recursion of surrogates automagically.
    ///
    /// When we deserialize, we read the short type name and pull the type from a cache.
    /// (Internal to <see cref="SurrogatedFormatterTools"/>)
    /// We then call Deserialize with the type to properly read the data.
    /// </remarks>
    public class BackwardsCompatibleSurrogatedFormatter<T> : IMessagePackFormatter<T>, IDoNotUsePolymorphicFormatter
        where T : ISurrogated
    {
        private readonly ExtendedActorSystem _system;

        public BackwardsCompatibleSurrogatedFormatter(ActorSystem system)
        {
            _system = (ExtendedActorSystem)system;
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
                var surrogate = value.ToSurrogate(_system);
                //Rather than relying on TypelessFormatter, it is better for us
                //  to use the exact type formatter alongside manual serialization of type.
                //
                // TODO: See notes in deserialize about future improvements
                //  for well known types.
                
                writer.WriteArrayHeader(3);
                //writer.WriteRaw(SurrogatedFormatterTools.baseArraySpan.Span);
                writer.WriteInt32(0);
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
                if (typeof(T) == typeof(ActorPath)
                    || typeof(T) == typeof(ChildActorPath)
                    || typeof(T) == typeof(RootActorPath))
                {
                    if (reader.NextMessagePackType == MessagePackType.String)
                    {
                        return deserializeOldPathFormat(reader, options);
                    }
                }
                else if (typeof(T) == typeof(IActorRef)
                         || typeof(T) == typeof(IInternalActorRef)
                         || typeof(T) == typeof(RepointableActorRef))
                {
                    if (reader.NextMessagePackType == MessagePackType.String)
                    {
                        return deserializeOldRefFormat(reader, options);
                    }
                }
                
                //TODO: Look into a way to avoid creating a string here.
                //
                //TODO: In future, we can use switching on well-known types,
                //  alongside a 3-slot array with (int,null,realdata)
                // that would let us switch on int,
                // rather than sniff type below.
                int entries = reader.ReadArrayHeader();
                if (entries == 3)
                {
                    var typeInt = reader.ReadInt32();

                    var strToken = reader.ReadString();
                    var mainData = reader.ReadRaw();
                    return deserializeFallback(strToken, mainData, options);
                }
                else if (entries == 2)
                    return readValueDefault(ref reader, options);
                else if (entries != 1)
                    return readValueFallBack(ref reader, options, entries);
                else
                    return throwInvalidLengthHelper(entries);
            }
        }

        private T deserializeOldPathFormat(MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var pathStr = reader.ReadString();
            return ActorPath.TryParse(pathStr, out var actorPath) ? (T)(object)actorPath : default;
        }

        private T deserializeOldRefFormat(MessagePackReader reader, MessagePackSerializerOptions options)
        {
            var pathStr = reader.ReadString();
            return (T)((ExtendedActorSystem)_system).Provider.ResolveActorRef(
                pathStr);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static T throwInvalidLengthHelper(int entries)
        {
            throw new MalformedPayloadException(
                "Invalid payload length on message!");
        }

        private T readValueFallBack(ref MessagePackReader reader,
            MessagePackSerializerOptions options, int arraySize)
        {
            //TODO: This feels unsafe.
            reader.Skip();
            var v = readValueDefault(ref reader, options);
            while (arraySize > 2)
            {
                reader.Skip();
                arraySize--;
            }

            return v;

        }

        private T deserializeFallback(string strToken, ReadOnlySequence<byte> mainData, MessagePackSerializerOptions options)
        {
            var deserType =
                SurrogatedFormatterTools.GetTypeForShortName(strToken);
            return (T)((ISurrogate)MessagePackSerializer.Deserialize(deserType,
                mainData, options)).FromSurrogate(_system);
        }

        private T readValueDefault(ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            return deserializeFallback(reader.ReadString(), reader.ReadRaw(),
                options);
        }
    }
}