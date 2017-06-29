using System;
using System.Collections.Generic;
using System.Reflection;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MsgPack.Resolvers
{
    public class ExceptionFallbackResolver : IFormatterResolver
    {
        public static readonly IFormatterResolver Instance = new ExceptionFallbackResolver();
        ExceptionFallbackResolver() { }

        public IMessagePackFormatter<T> GetFormatter<T>() => FormatterCache<T>.Formatter;

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;
            static FormatterCache() => Formatter = (IMessagePackFormatter<T>)ExceptionFallbackFormatterHelper.GetFormatter<T>();
        }
    }

    internal static class ExceptionFallbackFormatterHelper
    {
        internal static object GetFormatter<T>()
        {
            return typeof(Exception).IsAssignableFrom(typeof(T)) ? new ExceptionFallbackFormatter<T>() : null;
        }
    }

    public class ExceptionFallbackFormatter<T> : IMessagePackFormatter<T>
    {
        private const BindingFlags All = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        private TypeInfo ExceptionTypeInfo = typeof(Exception).GetTypeInfo();
        private static readonly IMessagePackFormatter<object> ObjectFormatter = TypelessFormatter.Instance;
        private HashSet<string> exclude = new HashSet<string>
        {
            "ClassName",
            "Message",
            "StackTraceString",
            "Source",
            "InnerException",
            "HelpURL",
            "RemoteStackTraceString",
            "RemoteStackIndex",
            "ExceptionMethod",
            "HResult",
            "Data",
            "TargetSite",
            "HelpLink",
            "StackTrace"
        };

        public int Serialize(ref byte[] bytes, int offset, T value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var exception = value as Exception;

            var startOffset = offset;
            offset += MessagePackBinary.WriteString(ref bytes, offset, exception?.Message);
            offset += MessagePackBinary.WriteString(ref bytes, offset, exception?.StackTrace);
            offset += MessagePackBinary.WriteString(ref bytes, offset, exception?.Source);

            // store properties
            var elements = new Dictionary<string, object>();
            foreach (var property in exception.GetType().GetTypeInfo().DeclaredProperties)
            {
                if (exclude.Contains(property.Name)) continue;
                elements.Add(property.Name, property.GetValue(exception));
            }

            offset += MessagePackBinary.WriteMapHeader(ref bytes, offset, elements.Count);
            foreach (var element in elements)
            {
                offset += MessagePackBinary.WriteString(ref bytes, offset, element.Key);
                offset += ObjectFormatter.Serialize(ref bytes, offset, element.Value, formatterResolver);
            }

            return offset - startOffset;
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return default(T);
            }

            int startOffset = offset;

            var obj = Activator.CreateInstance(typeof(T));
            var message = MessagePackBinary.ReadString(bytes, offset, out readSize);
            offset += readSize;
            var stackTrace = MessagePackBinary.ReadString(bytes, offset, out readSize);
            offset += readSize;
            var source = MessagePackBinary.ReadString(bytes, offset, out readSize);
            offset += readSize;
            
            ExceptionTypeInfo?.GetField("_message", All)?.SetValue(obj, message);
            if (!string.IsNullOrEmpty(stackTrace)) ExceptionTypeInfo?.GetField("_stackTraceString", All)?.SetValue(obj, stackTrace);
            if (!string.IsNullOrEmpty(source)) ExceptionTypeInfo?.GetField("_source", All)?.SetValue(obj, source);

            // read properties
            var propertiesCount = MessagePackBinary.ReadMapHeader(bytes, offset, out readSize);
            offset += readSize;
            for (int i = 0; i < propertiesCount; i++)
            {
                var key = MessagePackBinary.ReadString(bytes, offset, out readSize);
                offset += readSize;
                var val = ObjectFormatter.Deserialize(bytes, offset, formatterResolver, out readSize);
                offset += readSize;

                typeof(T).GetProperty(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty).SetValue(obj, val);
            }

            readSize = offset - startOffset;
            return (T)obj;
        }
    }
}
