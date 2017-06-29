Akka.Serialization.MessagePack
===
[![Build status](https://ci.appveyor.com/api/projects/status/9uvd8ilxkg8dqxn3/branch/master?svg=true)](https://ci.appveyor.com/project/ravengerUA/akka-serialization-messagepack/branch/master)
Akka.NET serialization with [MessagePack](https://github.com/neuecc/MessagePack-CSharp)

## Supported types
- Primitive types (`int`, `string`, `long`, etc)
- Build-in types (`DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`, `Uri`, `Enum`, etc)
- Collections (`List<T>`, `HashSet<T>`, `Dictionary<TKey, TValue>`, etc)
- Immutable Collections
- Exceptions (only on full .NET Framework)
- Akka.NET specific types (`ActorPath` and `ActorRef`)

## Not supported types
- internal and private classes
- classes with private or internal constructors
