//-----------------------------------------------------------------------
// <copyright file="IncapsulationTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using Akka.Serialization.Testkit.Util;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class IncapsulationTests : TestKit.Xunit2.TestKit
    {
        protected IncapsulationTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public virtual void Can_Serialize_internal_class()
        {
            var internalClass = new IncapsulationMessages.InternalClass(55);
            AssertEqual(internalClass);
        }

        [Fact]
        public virtual void Can_Serialize_a_class_with_internal_constructor()
        {
            var classWithInternalConstructor = new IncapsulationMessages.ClassWithInternalConstructor(55);
            AssertEqual(classWithInternalConstructor);
        }

        [Fact]
        public virtual void Can_Serialize_a_class_with_private_constructor()
        {
            var privateConstructor = IncapsulationMessages.ClassWithPrivateConstructor.Instance;
            AssertEqual(privateConstructor);
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

    public static class IncapsulationMessages
    {
        internal class InternalClass
        {
            public InternalClass(int number)
            {
                Number = number;
            }

            public int Number { get; }

            protected bool Equals(InternalClass other) => Number == other.Number;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((InternalClass)obj);
            }

            public override int GetHashCode() => Number;
        }

        public class ClassWithPrivateConstructor
        {
            public static ClassWithPrivateConstructor Instance { get; } = new ClassWithPrivateConstructor();
            private ClassWithPrivateConstructor() { }
            public override bool Equals(object obj) => obj is ClassWithPrivateConstructor;
        }

        public class ClassWithInternalConstructor
        {
            internal ClassWithInternalConstructor(int number)
            {
                Number = number;
            }

            public int Number { get; }

            protected bool Equals(ClassWithInternalConstructor other) => Number == other.Number;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ClassWithInternalConstructor)obj);
            }

            public override int GetHashCode() => Number;
        }
    }
}
