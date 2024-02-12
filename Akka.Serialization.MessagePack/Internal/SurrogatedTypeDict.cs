namespace Akka.Serialization.MessagePack.Resolvers;

/// <summary>
/// Used to provide indexes into a fast array lookup for resolvers.
/// </summary>
internal static class SurrogatedTypeDict<T>
{
    //WARNING, Read Notes!
    //It's ok that we return -1 here,
    //Since it's never going to actually be used on a path
    public static readonly int TypeVal = SurrogateResolvable<T>.IsSurrogated ? SurrogateTypeDictCtr.doNotCallExternally() : -1;
}