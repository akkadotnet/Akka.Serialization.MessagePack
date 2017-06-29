using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.Serialization.Testkit
{
    public class ConfigFactory
    {
        public static string GetConfig(Type serializerType)
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
    }
}
