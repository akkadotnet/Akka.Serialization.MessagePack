//-----------------------------------------------------------------------
// <copyright file="ExceptionsTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using FluentAssertions;
using System;
using System.Runtime.Serialization;
using Akka.Serialization.Testkit.Util;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class ExceptionsTests : TestKit.Xunit2.TestKit
    {
        protected ExceptionsTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public virtual void Can_Serialize_Exception()
        {
            var exception = new SampleExceptions.BasicException();
            AssertAndReturn(exception).Should().BeOfType<SampleExceptions.BasicException>();
        }

        [Fact]
        public virtual void Can_Serialize_ExceptionWithMessage()
        {
            var expected = new SampleExceptions.BasicException("Some message");
            var actual = AssertAndReturn(expected);
            AssertException(expected, actual);
        }

        [Fact]
        public virtual void Can_Serialize_ExceptionWithMessageAndInnerException()
        {
            var expected = new SampleExceptions.BasicException("Some message", new ArgumentNullException());
            var actual = AssertAndReturn(expected);
            AssertException(expected, actual);
        }

        [Fact]
        public virtual void Can_Serialize_ExceptionWithStackTrace()
        {
            try
            {
                throw new SampleExceptions.BasicException();
            }
            catch (SampleExceptions.BasicException ex)
            {
                var actual = AssertAndReturn(ex);
                AssertException(ex, actual);
            }
        }

        [Fact]
        public virtual void Can_Serialize_ExceptionWithCustomFields()
        {
            var exception = new SampleExceptions.ExceptionWithCustomFields("Some message", "John", 16);
            var actual = AssertAndReturn(exception);
            AssertException(exception, actual);
            actual.Name.Should().Be(exception.Name);
            actual.Age.Should().Be(exception.Age);
        }

        private void AssertException(Exception expected, Exception actual)
        {
            if (expected == null && actual == null) return;
            actual.Should().BeOfType(expected.GetType());
            actual.Message.Should().Be(expected.Message);
            actual.StackTrace.Should().Be(expected.StackTrace);
            actual.Source.Should().Be(expected.Source);
            AssertException(expected.InnerException, actual.InnerException);
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

    public static class SampleExceptions
    {
        public class BasicException : Exception
        {
            public BasicException()
            {
            }

            public BasicException(string message) : base(message)
            {
            }

            public BasicException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected BasicException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        public class ExceptionWithCustomFields : Exception
        {
            public ExceptionWithCustomFields()
            {
            }

            public ExceptionWithCustomFields(string message, string name, int age) 
                : this(message, name, age, null)
            {

            }

            public ExceptionWithCustomFields(string message, string name, int age, Exception innerException) 
                : base(message, innerException)
            {
                Name = name;
                Age = age;
            }

            protected ExceptionWithCustomFields(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                Name = info.GetString("Name");
                Age = info.GetInt32("Age");
            }

            public string Name { get; set; }

            public int Age { get; set; }

            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException(nameof(info));
                }

                info.AddValue("Name", Name);
                info.AddValue("Age", Age);
                base.GetObjectData(info, context);
            }
        }
    }
}