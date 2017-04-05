using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Akka.Serialization.MessagePack.Tests.TestKit
{
    public abstract class BaseMessagesSerializationSpec : Akka.TestKit.Xunit2.TestKit
    {
        protected BaseMessagesSerializationSpec(Type serializerType) : base(GetConfig(serializerType))
        {
        }

        private static string GetConfig(Type serializerType)
        {
            return @"
                akka.actor {
                    serializers {
                        testserializer = """ + serializerType.AssemblyQualifiedName + @"""
                    }

                    serialization-bindings {
                      ""System.Object"" = testserializer
                    }
                }";
        }

        #region Messages
        public class EmptyMessage { }

        public class EmptySingleton
        {
            public static EmptySingleton Instance { get; } = new EmptySingleton();

            private EmptySingleton() { }
        }

        public sealed class ContractlessSample
        {
            public int MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }
        }

        public sealed class ImmutableMessageWithSimpleTypes
        {
            public ImmutableMessageWithSimpleTypes(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }

            public int Age { get; }

            private bool Equals(ImmutableMessageWithSimpleTypes other)
            {
                return string.Equals(Name, other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithSimpleTypes && Equals((ImmutableMessageWithSimpleTypes)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        public sealed class ImmutableMessageWithGenericTypes<T>
        {
            public ImmutableMessageWithGenericTypes(string name, T data)
            {
                Name = name;
                Data = data;
            }

            public string Name { get; }

            public T Data { get; }

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

        public sealed class ImmutableMessageWithObjectTypes
        {
            public ImmutableMessageWithObjectTypes(string name, object data)
            {
                Name = name;
                Data = data;
            }

            public string Name { get; }

            public object Data { get; }

            private bool Equals(ImmutableMessageWithObjectTypes other)
            {
                return string.Equals(Name, other.Name) && Equals(Data, other.Data);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithObjectTypes && Equals((ImmutableMessageWithObjectTypes)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Data != null ? Data.GetHashCode() : 0);
                }
            }
        }

        public sealed class ImmutableMessageWithTwoConstructors
        {
            public ImmutableMessageWithTwoConstructors(int age, string name)
            {
                Name = name;
                Age = age;
            }

            public ImmutableMessageWithTwoConstructors(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }

            public int Age { get; }

            private bool Equals(ImmutableMessageWithSimpleTypes other)
            {
                return string.Equals(Name, other.Name) && Age == other.Age;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj is ImmutableMessageWithSimpleTypes && Equals((ImmutableMessageWithSimpleTypes)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Age;
                }
            }
        }

        #endregion

        [Fact]
        public void Can_serialize_EmptyMessage()
        {
            var message = new EmptyMessage();
            AssertAndReturn(message).Should().BeOfType<EmptyMessage>();
        }

        [Fact]
        public void Can_serialize_EmptySingleton()
        {
            var message = EmptySingleton.Instance;
            AssertAndReturn(message).Should().BeOfType<EmptySingleton>();
        }

        [Fact]
        public void Can_serialize_ContractlessSample()
        {
            var message = new ContractlessSample { MyProperty1 = 99, MyProperty2 = 9999 };
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithSimpleTypes()
        {
            var message = new ImmutableMessageWithSimpleTypes("Alex", 16);
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithGenericTypes()
        {
            var message = new ImmutableMessageWithGenericTypes<int>("Alex", 16);
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithObjectTypes()
        {
            var message = new ImmutableMessageWithObjectTypes("Alex", 16);
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithTwoConstructors()
        {
            var message = new ImmutableMessageWithTwoConstructors("Alex", 16);
            AssertEqual(message);
        }

        private T AssertAndReturn<T>(T message)
        {
            var serializer = Sys.Serialization.FindSerializerFor(message);
            var serialized = serializer.ToBinary(message);
            var result = serializer.FromBinary(serialized, typeof(T));
            return (T)result;
        }

        private void AssertEqual<T>(T message)
        {
            var deserialized = AssertAndReturn(message);
            Assert.Equal(message, deserialized);
        }
    }
}
