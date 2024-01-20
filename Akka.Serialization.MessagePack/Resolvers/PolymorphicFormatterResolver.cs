using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Akka.Util;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class PolymorphicFormatterResolver : IFormatterResolver
    {
        private readonly IFormatterResolver _normalResolver;

        private readonly ConcurrentDictionary<Type, IMessagePackFormatter>
            _formatterDict =
                new ConcurrentDictionary<Type, IMessagePackFormatter>();
        public PolymorphicFormatterResolver(IFormatterResolver normalResolver)
        {
            _normalResolver = normalResolver;
        }
        
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (_formatterDict.TryGetValue(typeof(T), out var formatter))
            {
                return (IMessagePackFormatter<T>)formatter;
            }
            else
            {
                return MakeValue<T>();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IMessagePackFormatter<T> MakeValue<T>()
        {
            //We don't have GetOrAdd in NetStandard2.0
            //So instead we suffer and do our own double-checks
            IMessagePackFormatter<T> formatterToUse = _normalResolver.GetFormatter<T>();
            if ((_normalResolver is IDoNotUsePolymorphicFormatter) == false &&
                typeof(T).IsClass)
            {
                if ((typeof(T).IsAbstract || !typeof(T).IsSealed) &&
                    typeof(ISurrogated).IsAssignableFrom(typeof(T))==  false)
                {
                    formatterToUse =
                        new PolymorphicFormatter<T>(formatterToUse);
                }
            }
            
            _formatterDict.TryAdd(typeof(T),formatterToUse);
            return (IMessagePackFormatter<T>)_formatterDict[typeof(T)];
        }
    }
}