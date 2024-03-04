using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SensorMonitoring.Api.Repository;
using SensorMonitoring.Shared.Models;

namespace SensorMonitoring.Api;

public class MyBenchmarkTesting
{
    private IOptions<ApiOptions> _options;

    public MyBenchmarkTesting()
    {
        _options = Options.Create(new ApiOptions()
        {
            SensorRepositoryConnection = "C:\\Users\\B\\AppData\\Local\\SensorMonitoring.db"
        });
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
