namespace Akka.Serialization.MessagePack.Resolvers;

internal static class SurrogateTypeDictCtr
{
    private static int _typeCtr;

    internal static int doNotCallExternally()
    {
        return Interlocked.Increment(ref _typeCtr);
    }
}