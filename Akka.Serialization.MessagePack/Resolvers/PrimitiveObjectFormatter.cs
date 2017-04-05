using System;
using System.Collections;
using MessagePack;
using MessagePack.Formatters;

namespace Akka.Serialization.MessagePack.Resolvers
{
    public class PrimitiveObjectFormatter : IMessagePackFormatter<object>
    {
        public static readonly IMessagePackFormatter<object> Instance = new PrimitiveObjectFormatter();

        PrimitiveObjectFormatter()
        {

        }

        public int Serialize(ref byte[] bytes, int offset, object value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var t = value.GetType();
            var code = Type.GetTypeCode(t);
            switch (code)
            {
                case TypeCode.Boolean:
                    return MessagePackBinary.WriteBoolean(ref bytes, offset, (bool)value);
                case TypeCode.Char:
                    return MessagePackBinary.WriteChar(ref bytes, offset, (char)value);
                case TypeCode.SByte:
                    return MessagePackBinary.WriteSByte(ref bytes, offset, (sbyte)value);
                case TypeCode.Byte:
                    return MessagePackBinary.WriteByte(ref bytes, offset, (byte)value);
                case TypeCode.Int16:
                    return MessagePackBinary.WriteInt16(ref bytes, offset, (Int16)value);
                case TypeCode.UInt16:
                    return MessagePackBinary.WriteUInt16(ref bytes, offset, (UInt16)value);
                case TypeCode.Int32:
                    return MessagePackBinary.WriteInt32(ref bytes, offset, (Int32)value);
                case TypeCode.UInt32:
                    return MessagePackBinary.WriteUInt32(ref bytes, offset, (UInt32)value);
                case TypeCode.Int64:
                    return MessagePackBinary.WriteInt64(ref bytes, offset, (Int64)value);
                case TypeCode.UInt64:
                    return MessagePackBinary.WriteUInt64(ref bytes, offset, (UInt64)value);
                case TypeCode.Single:
                    return MessagePackBinary.WriteSingle(ref bytes, offset, (Single)value);
                case TypeCode.Double:
                    return MessagePackBinary.WriteDouble(ref bytes, offset, (double)value);
                case TypeCode.DateTime:
                    return formatterResolver.GetFormatter<DateTime>().Serialize(ref bytes, offset, (DateTime)value, formatterResolver);
                case TypeCode.String:
                    return formatterResolver.GetFormatter<string>().Serialize(ref bytes, offset, (string)value, formatterResolver);
                default:
                    if (value is byte[])
                    {
                        return formatterResolver.GetFormatter<byte[]>().Serialize(ref bytes, offset, (byte[])value, formatterResolver);
                    }
                    else if (t.IsEnum)
                    {
                        var underlyingType = Enum.GetUnderlyingType(t);
                        var v = Convert.ChangeType(value, underlyingType);
                        return Serialize(ref bytes, offset, v, formatterResolver);
                    }
                    else if (t is ICollection)
                    {
                        var c = t as ICollection;
                        var startOffset = offset;
                        offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, c.Count);
                        foreach (var item in c)
                        {
                            offset += Serialize(ref bytes, offset, item, formatterResolver);
                        }
                        return offset - startOffset;
                    }
                    else if (t is IDictionary)
                    {
                        var d = t as IDictionary;
                        var startOffset = offset;
                        offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, d.Count);
                        foreach (DictionaryEntry item in d)
                        {
                            offset += Serialize(ref bytes, offset, item.Key, formatterResolver);
                            offset += Serialize(ref bytes, offset, item.Value, formatterResolver);
                        }
                        return offset - startOffset;
                    }

                    throw new InvalidOperationException("Not supported primitive object resolver.");
            }
        }

        public object Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var type = MessagePackBinary.GetMessagePackType(bytes, offset);
            switch (type)
            {
                case MessagePackType.Integer:
                    var code = bytes[offset];
                    if (MessagePackCode.MinNegativeFixInt <= code && code <= MessagePackCode.MaxNegativeFixInt) return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
                    else if (MessagePackCode.MinFixInt <= code && code <= MessagePackCode.MaxFixInt) return MessagePackBinary.ReadByte(bytes, offset, out readSize);
                    else if (code == MessagePackCode.Int8) return MessagePackBinary.ReadSByte(bytes, offset, out readSize);
                    else if (code == MessagePackCode.Int16) return MessagePackBinary.ReadInt16(bytes, offset, out readSize);
                    else if (code == MessagePackCode.Int32) return MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                    else if (code == MessagePackCode.Int64) return MessagePackBinary.ReadInt64(bytes, offset, out readSize);
                    else if (code == MessagePackCode.UInt8) return MessagePackBinary.ReadByte(bytes, offset, out readSize);
                    else if (code == MessagePackCode.UInt16) return MessagePackBinary.ReadUInt16(bytes, offset, out readSize);
                    else if (code == MessagePackCode.UInt32) return MessagePackBinary.ReadUInt32(bytes, offset, out readSize);
                    else if (code == MessagePackCode.UInt64) return MessagePackBinary.ReadUInt64(bytes, offset, out readSize);
                    throw new InvalidOperationException("Invalid primitive bytes.");
                case MessagePackType.Boolean:
                    return MessagePackBinary.ReadBoolean(bytes, offset, out readSize);
                case MessagePackType.Float:
                    return MessagePackBinary.ReadDouble(bytes, offset, out readSize);
                case MessagePackType.String:
                    return MessagePackBinary.ReadString(bytes, offset, out readSize);
                case MessagePackType.Binary:
                    return MessagePackBinary.ReadBytes(bytes, offset, out readSize);
                case MessagePackType.Extension:
                    var ext = MessagePackBinary.ReadExtensionFormat(bytes, offset, out readSize);
                    if (ext.TypeCode == ReservedMessagePackExtensionTypeCode.DateTime)
                    {
                        return MessagePackBinary.ReadDateTime(bytes, offset, out readSize);
                    }
                    throw new InvalidOperationException("Invalid primitive bytes.");
                case MessagePackType.Array:
                    {
                        var length = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                        var startOffset = offset;
                        offset += readSize;

                        var array = new object[length];
                        for (int i = 0; i < length; i++)
                        {
                            array[i] = Deserialize(bytes, offset, formatterResolver, out readSize);
                            offset += readSize;
                        }

                        readSize = offset - startOffset;
                        return array;
                    }
                case MessagePackType.Map:
                    {
                        var length = MessagePackBinary.ReadMapHeader(bytes, offset, out readSize);
                        var startOffset = offset;
                        offset += readSize;

                        var hash = new Hashtable(length);
                        for (int i = 0; i < length; i++)
                        {
                            var key = Deserialize(bytes, offset, formatterResolver, out readSize);
                            offset += readSize;

                            var value = Deserialize(bytes, offset, formatterResolver, out readSize);
                            offset += readSize;

                            hash.Add(key, value);
                        }

                        readSize = offset - startOffset;
                        return hash;
                    }
                case MessagePackType.Nil:
                    readSize = 1;
                    return null;
                default:
                    throw new InvalidOperationException("Invalid primitive bytes.");
            }
        }
    }

    public class PrimitiveObjectResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new PrimitiveObjectResolver();

        PrimitiveObjectResolver()
        {

        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return (typeof(T) == typeof(object))
                ? (IMessagePackFormatter<T>)(object)PrimitiveObjectFormatter.Instance
                : null;
        }
    }
}
