using System;
using Akka.Configuration;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class ConfigFormatter : IMessagePackFormatter<Config>
    {
        public static readonly IMessagePackFormatter<Config> Instance = new ConfigFormatter();

        ConfigFormatter()
        {

        }

        public int Serialize(ref byte[] bytes, int offset, Config value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteString(ref bytes, offset, value.ToString(true));
            return offset - startOffset;
        }

        public Config Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var hocon = MessagePackBinary.ReadString(bytes, offset, out readSize);
            return ConfigurationFactory.ParseString(hocon);
        }
    }

    public class ConfigResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new ConfigResolver();

        ConfigResolver()
        {

        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return (typeof(T) == typeof(Config))
                ? (IMessagePackFormatter<T>)ConfigFormatter.Instance
                : null;
        }
    }
}
