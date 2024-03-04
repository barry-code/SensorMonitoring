
using BenchmarkDotNet.Running;
using SensorMonitoring.Api;

Console.WriteLine("Testing!");

var options = new ApiOptions()
{
    SensorRepositoryConnection = "C:\\Users\\B\\AppData\\Local\\SensorMonitoring.db"
};

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
