using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Api.Repository;
using SensorMonitoring.Shared.Models;

namespace SensorMonitoring.Api;

public class MyBenchmarkTesting
{
    private ApiOptions _options;

    public MyBenchmarkTesting()
    {
        _options = new ApiOptions()
        {
            SensorRepositoryConnection = "C:\\Users\\B\\AppData\\Local\\SensorMonitoring.db"
        };
    }

    [Benchmark]
    public List<SensorReading> GetAllHistory()
    {
        List<int> sensorIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        DateTimeOffset from = DateTime.MinValue;
        DateTimeOffset to = DateTime.MaxValue;

        using (var context = new SensorContext(_options))
        {
            var readings = context.SensorReadings
                .Where(s => sensorIds.Contains(s.SensorId) && (s.DateTime >= from && s.DateTime <= to))
                .ToList();

            return readings ?? new List<SensorReading>();
        }
    }

    [Benchmark]
    public List<SensorReading> GetAllHistoryWithNoTracking()
    {
        List<int> sensorIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        DateTimeOffset from = DateTime.MinValue;
        DateTimeOffset to = DateTime.MaxValue;

        using (var context = new SensorContext(_options))
        {
            var readings = context.SensorReadings
                .AsNoTracking()
                .Where(s => sensorIds.Contains(s.SensorId) && (s.DateTime >= from && s.DateTime <= to))
                .ToList();

            return readings ?? new List<SensorReading>();
        }
    }
}
