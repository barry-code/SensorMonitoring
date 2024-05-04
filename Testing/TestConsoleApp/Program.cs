
using BenchmarkDotNet.Running;
using SensorMonitoring.Api;

Console.WriteLine("Testing!");

try
{
    BenchmarkRunner.Run<MyBenchmarkTesting>();
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR RUNNING BENCHMARK: {ex.Message}");    
}

Console.WriteLine("ALL DONE");

Console.ReadLine();
