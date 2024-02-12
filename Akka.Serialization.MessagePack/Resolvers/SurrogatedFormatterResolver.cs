//-----------------------------------------------------------------------
// <copyright file="AkkaResolver.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Akka.Actor;
using Akka.Util.Internal;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class SurrogatedFormatterResolver : IFormatterResolver, IDoNotUsePolymorphicFormatter
    {

        private readonly IntIndexedMessagePackFormatterDict _formatterDict =
            new IntIndexedMessagePackFormatterDict(16);
        private readonly ConcurrentDictionary<Type, IMessagePackFormatter>
            _formatterCache =
                new ConcurrentDictionary<Type, IMessagePackFormatter>();

        private readonly Func<Type, IMessagePackFormatter> _formatterCreateFunc;
        public SurrogatedFormatterResolver(ExtendedActorSystem system)
        {
            //Cast in the func since we'll have to cache anyway.
            //The alternative is making a 'nullable' func in another static class,
            //But that may result in too much garbage for other types.
            _formatterCreateFunc = t =>
                (IMessagePackFormatter)typeof(SurrogatedFormatter<>)
                    .MakeGenericType(t)
                    .GetConstructor(new[] { typeof(ActorSystem) })
                    .Invoke(new[] { system });

        }
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (SurrogateResolvable<T>.IsSurrogated)
            {
                if (_formatterDict.TryGet(SurrogatedTypeDict<T>.TypeVal,
                        out var formatter))
                {
                    //IMPORTANT!
                    // This assumes that MakeValue<T> is doing a proper
                    // sanity check on the formatter
                    // produced by underlying resolver.
                    return Unsafe.As<IMessagePackFormatter<T>>(formatter);
                }
                else
                {
                    return MakeValue<T>();
                }
            }
            else
            {
                return null!;
            }
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private IMessagePackFormatter<T> MakeValue<T>()
        {
            // IMPORTANT!!!
            // For type safety make sure to read comments!!!
            // If you break below assumptions,
            // GetFormatter may require refactor!
            //
            // The explicit-casting here,
            // is to ensure we do a type check on returned value.
            // This helps guarantee our Unsafe calls are safe in context.
            // Since this is not at all the hot path,
            // easy cost to justify for retaining type safety.
            // 
            // Minor Note:
            // It's possible that we wastefully alloc here,
            // However our 'wrapped' formatter should be caching,
            // and the Polymorphic formatter itself is just a ref,
            // so it's a cheap temporary cost to pay.
            var f = (IMessagePackFormatter<T>)(object)_formatterCreateFunc(typeof(T));
            return (IMessagePackFormatter<T>)_formatterDict.TryAdd(
                TypeDict<T>.TypeVal, f);
        }
        
    }
}