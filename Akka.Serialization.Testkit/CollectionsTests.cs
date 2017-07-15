//-----------------------------------------------------------------------
// <copyright file="CollectionsTests.cs" company="Akka.NET Project">
//     Copyright (C) 2017 Akka.NET Contrib <https://github.com/AkkaNetContrib/Akka.Serialization.MessagePack>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Akka.Serialization.Testkit.Util;
using Xunit;

namespace Akka.Serialization.Testkit
{
    public abstract class CollectionsTests : TestKit.Xunit2.TestKit
    {
        protected CollectionsTests(Type serializerType) : base(ConfigFactory.GetConfig(serializerType))
        {
        }

        [Fact]
        public void Can_Serialize_Array()
        {
            var actual = new[] { 5, 7, 856, 34 };
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_List()
        {
            var actual = new List<int>() { 5, 7, 856, 34 };
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_LinkedList()
        {
            var actual = new LinkedList<int>();
            actual.AddLast(5);
            actual.AddLast(122);
            actual.AddLast(25);
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_Stack()
        {
            var actual = new Stack<int>();
            actual.Push(5);
            actual.Push(122);
            actual.Push(25);
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_Queue()
        {
            var actual = new Queue<int>();
            actual.Enqueue(5);
            actual.Enqueue(122);
            actual.Enqueue(25);
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_HashSet()
        {
            var actual = new HashSet<int>() { 5, 7, 856, 34 };
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_Dictionary()
        {
            var actual = new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two"
            };
            AssertEqual(actual);
        }

        // Immutable
        [Fact]
        public void Can_Serialize_ImmutableArray()
        {
            ImmutableArray<int> expected = new[] { 5, 7, 856, 34 }.ToImmutableArray();
            AssertAndReturn(expected).SequenceEqual(expected);
        }

        [Fact]
        public void Can_Serialize_ImmutableList()
        {
            var actual = new List<int>() { 5, 7, 856, 34 }.ToImmutableList();
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_ImmutableHashSet()
        {
            var actual = new HashSet<int>() { 5, 7, 856, 34 }.ToImmutableHashSet();
            AssertEqual(actual);
        }

        [Fact]
        public void Can_Serialize_ImmutableDictionary()
        {
            var actual = new Dictionary<int, string>()
            {
                [1] = "one",
                [2] = "two"
            }.ToImmutableDictionary();
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
}
