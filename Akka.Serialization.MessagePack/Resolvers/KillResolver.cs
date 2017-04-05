using Akka.Actor;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class KillFormatter : IMessagePackFormatter<Kill>
    {
        public static readonly IMessagePackFormatter<Kill> Instance = new KillFormatter();
        private KillFormatter() { }

        public int Serialize(ref byte[] bytes, int offset, Kill value, IFormatterResolver formatterResolver)
        {
            return offset;
        }

        public Kill Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            readSize = 0;
            return Kill.Instance;
        }
    }

    public class KillResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new KillResolver();
        private KillResolver() {}

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return (typeof(T) == typeof(Kill))
                ? (IMessagePackFormatter<T>)KillFormatter.Instance
                : null;
        }
    }
}
