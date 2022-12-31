using Akka.Util;

namespace Akka.Serialization.MessagePack.Resolvers
{
    static class SurrogateResolvable<T>
    {
        public static readonly bool IsSurrogate = (typeof(ISurrogated).IsAssignableFrom(typeof(T)));
    }
}