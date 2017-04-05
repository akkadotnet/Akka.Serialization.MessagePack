using Akka.Actor;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class PoisonPillFormatter : IMessagePackFormatter<PoisonPill>
    {
        public static readonly IMessagePackFormatter<PoisonPill> Instance = new PoisonPillFormatter();
        private PoisonPillFormatter() { }

        public int Serialize(ref byte[] bytes, int offset, PoisonPill value, IFormatterResolver formatterResolver)
        {
            return offset;
        }

        public PoisonPill Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            readSize = 0;
            return PoisonPill.Instance;
        }
    }

    public class PoisonPillResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new PoisonPillResolver();
        private PoisonPillResolver() {}

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return (typeof(T) == typeof(PoisonPill))
                ? (IMessagePackFormatter<T>)PoisonPillFormatter.Instance
                : null;
        }
    }
}
