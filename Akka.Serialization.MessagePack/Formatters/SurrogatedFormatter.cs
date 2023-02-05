using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Text;
using Akka.Actor;
using Akka.Pattern;
using Akka.Util;
using CommunityToolkit.HighPerformance.Buffers;
using MessagePack;
using MessagePack.Formatters;
using Microsoft.NET.StringTools;

namespace Akka.Serialization.MessagePack.Resolvers
{
    class SurrogatedFormatterTools
    {
        internal static readonly ConcurrentDictionary<string, Type> deserTypeCache =
            new ConcurrentDictionary<string, Type>();

        internal static readonly ReadOnlyMemory<byte> baseArraySpan = new byte[]
            { (byte)(MessagePackCode.MinFixArray | (byte)3) };

        internal static readonly Func<string, Type>
            typeLookupFactory = t => Type.GetType(t, true);
    }
    
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
    public class SurrogatedFormatter<T> : IMessagePackFormatter<T>
        where T : ISurrogated
    {
        private readonly ActorSystem _system;
        private static readonly StringPool _pool = new StringPool();

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
                var surrogate = value.ToSurrogate(_system);
                //Rather than relying on TypelessFormatter, it is better for us
                //  to use the exact type formatter alongside manual serialization of type.
                //
                // TODO: See notes in deserialize about future improvements
                //  for well known types.
                if (surrogate is ActorRefBase.Surrogate s)
                {
                    writer.WriteRaw();
                    writer.WriteArrayHeader(3);
                    options.Resolver.GetFormatter<string>()
                        .Serialize(ref writer, surrogate.GetType().TypeQualifiedName(),options);
                    MessagePackSerializer.Serialize(ref writer, s, options);
                    writer.WriteInt32(3);
                }
                else
                {
                    writer.WriteRaw(SurrogatedFormatterTools.baseArraySpan.Span);
                    //writer.WriteArrayHeader(2);
                    var surT = surrogate.GetType();
                    options.Resolver.GetFormatter<string>()
                        .Serialize(ref writer, surT.TypeQualifiedName(), options);
                    MessagePackSerializer.Serialize(surT, ref writer,
                        surrogate, options);    
                }
                
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
                int entries = reader.ReadArrayHeader();
                switch (entries)
                {
                    case 2:
                        return readValueDefault(ref reader, options);
                    default:
                        return readValueFallBack(ref reader, options, entries);
                }

            }
        }

        private T readValueFallBack(ref MessagePackReader reader,
            MessagePackSerializerOptions options, int arraySize)
        {
            if (arraySize == 3)
            {
                var strToken = reader.ReadString();
                var mainData = reader.ReadRaw();
                var typeInt = reader.ReadInt16();
                if (typeInt == 3)
                {
                    return 
                        (T)MessagePackSerializer
                            .Deserialize<ActorRefBase.Surrogate>(mainData,
                                options).FromSurrogate(_system);
                }
                else
                {
                    return deserializeFallback(strToken, mainData, options);
                }
            }
            else
            {
                var v = readValueDefault(ref reader, options);
                while (arraySize > 2)
                {
                    reader.Skip();
                    arraySize--;
                }
                return v;   
            }
        }

        private T deserializeFallback(string strToken, ReadOnlySequence<byte> mainData, MessagePackSerializerOptions options)
        {
            var deserType =
                SurrogatedFormatterTools.deserTypeCache.GetOrAdd(strToken,
                    SurrogatedFormatterTools.typeLookupFactory);
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