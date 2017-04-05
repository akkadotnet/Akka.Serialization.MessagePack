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

        [Fact]
        public void Can_serialize_EmptyMessage()
        {
            var message = new Messages.EmptyMessage();
            AssertAndReturn(message).Should().BeOfType<Messages.EmptyMessage>();
        }

        [Fact(Skip = "Not implemented yet")]
        public void Can_serialize_EmptySingleton()
        {
            var message = Messages.EmptySingleton.Instance;
            AssertAndReturn(message).Should().BeOfType<Messages.EmptySingleton>();
        }

        [Fact]
        public void Can_serialize_ContractlessSample()
        {
            var message = new Messages.ContractlessSample { MyProperty1 = 99, MyProperty2 = 9999 };
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithSimpleTypes()
        {
            var message = new Messages.ImmutableMessageWithSimpleTypes("Alex", 16);
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithReadonlyFields()
        {
            var message = new Messages.ImmutableMessageWithReadonlyFields("Alex", 16);
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithGenericTypes()
        {
            var message = new Messages.ImmutableMessageWithGenericTypes<int>("Alex", 16);
            AssertEqual(message);
        }

        [Fact]
        public void Can_serialize_ImmutableMessageWithObjectTypes()
        {
            var message = new Messages.ImmutableMessageWithObjectTypes("Alex", 16);
            AssertEqual(message);
        }

        [Fact(Skip = "Not implemented yet")]
        public void Can_serialize_ImmutableMessageWithTwoConstructors()
        {
            var message = new Messages.ImmutableMessageWithTwoConstructors("Alex", 16);
            AssertEqual(message);
        }

        [Fact(Skip = "Not implemented yet")]
        public void Can_serialize_ImmutableMessageWithObjectsInsideObjects()
        {
            var message = new Messages.ImmutableMessageWithObjectsInsideObjects(new Messages.ImmutableMessageWithObjectTypes("Alex", 16));
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
