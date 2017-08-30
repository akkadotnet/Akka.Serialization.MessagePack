//-----------------------------------------------------------------------
// <copyright file="PolymorphismTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Akka.Serialization.Testkit.Util;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class PolymorphismTests : TestKit.Xunit2.TestKit
    {
        protected PolymorphismTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public virtual void Can_Serialize_MessageWithObjectPropertyPrimitive()
        {
            var actual = new PolymorphismMessages.ImmutableMessageWithObjectTypes
            {
                Name = "John",
                Data = 435345345
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_MessageWithObjectPropertyPrimitive_do_not_downcast_to_byte()
        {
            var actual = new PolymorphismMessages.ImmutableMessageWithObjectTypes
            {
                Name = "John",
                Data = 125
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_MessageWithObjectPropertyComplex()
        {
            var data = new CustomMessage.MessageWithPublicSetters
            {
                Name = "John",
                Age = 15
            };

            var actual = new PolymorphismMessages.ImmutableMessageWithObjectTypes
            {
                Name = "John",
                Data = data
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_MessageWithGenericPropertyPrimitive()
        {
            var actual = new PolymorphismMessages.ImmutableMessageWithGenericTypes<int>
            {
                Name = "John",
                Data = 435345345
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_MessageWithGenericPropertyComplex()
        {
            var data = new CustomMessage.MessageWithPublicSetters
            {
                Name = "John",
                Age = 15
            };

            var actual = new PolymorphismMessages.ImmutableMessageWithGenericTypes<CustomMessage.MessageWithPublicSetters>
            {
                Name = "John",
                Data = data
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_CollectionWithObjectTypePrimitive()
        {
            var actual = new List<object> { 5, 7, 856, 34 };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_CollectionWithObjectTypeComplex()
        {
            var actual = new List<object> {
                new CustomMessage.MessageWithPublicSetters { Name = "John", Age = 15 },
                new CustomMessage.MessageWithPublicSetters { Name = "John2", Age = 16 }
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_DictionaryKeyWithObjectTypePrimitive()
        {
            var actual = new Dictionary<object, string>
            {
                [5] = "Name",
                [6] = "Name2",
                [7] = "Name3",
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_DictionaryKeyWithObjectTypeComplex()
        {
            var actual = new Dictionary<object, string>
            {
                [new CustomMessage.MessageWithPublicSetters { Name = "John", Age = 15 }] = "Name",
                [new CustomMessage.MessageWithPublicSetters { Name = "John2", Age = 16 }] = "Name2",
                [new CustomMessage.MessageWithPublicSetters { Name = "John2", Age = 14 }] = "Name3",
            };
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_TupleItemWithObjectTypePrimitive()
        {
            var actual = Tuple.Create<int, object>(25, "John");
            AssertEqual(actual);
        }

        [Fact]
        public virtual void Can_Serialize_TupleItemWithObjectTypeComplex()
        {
            var actual = Tuple.Create<int, object>(25, new CustomMessage.MessageWithPublicSetters { Name = "John", Age = 15 });
            AssertEqual(actual);
        }

        protected T AssertAndReturn<T>(T message)
        {
            var serializer = Sys.Serialization.FindSerializerFor(message);
            var serialized = serializer.ToBinary(message);
            var result = serializer.FromBinary(serialized, typeof(T));
            return (T)result;
        }

        protected void AssertEqual<T>(T message)
        {
            var deserialized = AssertAndReturn(message);
            Assert.Equal(message, deserialized);
        }
    }

    public static class PolymorphismMessages
    {
        public class ImmutableMessageWithObjectTypes
        {
            public string Name { get; set; }

            public object Data { get; set; }

            protected bool Equals(ImmutableMessageWithObjectTypes other)
            {
                return string.Equals(Name, other.Name) && Equals(Data, other.Data);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ImmutableMessageWithObjectTypes)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Data != null ? Data.GetHashCode() : 0);
                }
            }
        }

        public sealed class ImmutableMessageWithGenericTypes<T>
        {
            public string Name { get; set; }

            public T Data { get; set; }

            private bool Equals(ImmutableMessageWithGenericTypes<T> other)
            {
                return string.Equals(Name, other.Name) && EqualityComparer<T>.Default.Equals(Data, other.Data);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithGenericTypes<T> && Equals((ImmutableMessageWithGenericTypes<T>)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ EqualityComparer<T>.Default.GetHashCode(Data);
                }
            }
        }
    }
}
