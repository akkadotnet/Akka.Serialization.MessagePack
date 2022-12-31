using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Akka.Serialization.MessagePack.Benchmarks
{
    public class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            Add(Job.Default.With(CsProjCoreToolchain.NetCoreApp20).With(Platform.X64).WithGcServer(true).WithId("NETCORE 2.0"));
            this.Options =
                Options | ConfigOptions.DisableOptimizationsValidator;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<SerializationBenchmarks>();

            Console.ReadLine();
        }
    }
}
