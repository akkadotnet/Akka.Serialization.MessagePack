Akka.Serialization.MessagePack
===
[![Build status](https://ci.appveyor.com/api/projects/status/nl651507h49ab63k/branch/master?svg=true)](https://ci.appveyor.com/project/akkadotnet-contrib/akka-serialization-messagepack/branch/master)

Akka.NET serialization with [MessagePack](https://github.com/neuecc/MessagePack-CSharp)

## Supported platforms
- .NET Core 2.0 (via .NET Standard 2.0)
- .NET Framework 4.6 and higher (via .NET Standard 2.0)

## It supports
- Primitive types (`int`, `string`, `long`, etc)
- Build-in types (`DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`, `Uri`, `Enum`, etc)
- Collections (`List<T>`, `HashSet<T>`, `Dictionary<TKey, TValue>`, etc)
- Immutable Collections
- Exceptions
- Akka.NET specific types (`ActorPath` and `ActorRef`)
- Object types (polymorphic serialization)
- Generic types
- Version tolerance

## It does not support
- Internal and private classes
- Classes with private or internal constructors
- F# types (`Set`, `Map`, `List`, `FSharpAsync<T>`, discriminated unions)
- Handling circular references
- Preserve object references

## How to setup MessagePack as default serializer
Bind MessagePack serializer using following HOCON configuration in your actor system settings:
```hocon
akka {
  actor {
    serializers {
      messagepack = "Akka.Serialization.MessagePack.MsgPackSerializer, Akka.Serialization.MessagePack"
    }
    serialization-bindings {
      "System.Object" = messagepack
    }
  }
}
```

## Maintainer
- [alexvaluyskiy](https://github.com/alexvaluyskiy)
