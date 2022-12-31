using Akka.Actor;
using Akka.Util;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class SurrogateFormatter<T> : IMessagePackFormatter<T>
        where T : ISurrogated
    {
        private readonly ActorSystem _system;

        public SurrogateFormatter(ActorSystem system)
        {
            _system = system;
        }

        public void Serialize(ref MessagePackWriter writer, T value,
            MessagePackSerializerOptions options)
        {
            options.Resolver.GetFormatter<ISurrogate>()
                .Serialize(ref writer, value.ToSurrogate(_system), options);
        }

        public T Deserialize(ref MessagePackReader reader,
            MessagePackSerializerOptions options)
        {
            var surrogate = options.Resolver.GetFormatter<ISurrogate>()
                .Deserialize(ref reader, options);
            return (T)surrogate.FromSurrogate(_system);
        }
    }
}