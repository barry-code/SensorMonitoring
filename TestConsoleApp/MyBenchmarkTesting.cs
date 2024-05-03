using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SensorMonitoring.Api.Repository;
using SensorMonitoring.Shared.Models;

namespace SensorMonitoring.Api;

public class MyBenchmarkTesting
{
    private IOptions<ApiOptions> _apiOptions;
    private DbContextOptionsBuilder<SensorContext> _contextBuilder;

    public MyBenchmarkTesting()
    {
        _apiOptions = Options.Create(new ApiOptions()
        {
            SensorRepositoryConnection = "DataSource=C:\\Users\\B\\AppData\\Local\\SensorMonitoring.db"
        });

        _contextBuilder = new DbContextOptionsBuilder<SensorContext>();
        _contextBuilder.UseSqlite(_apiOptions.Value.SensorRepositoryConnection);
    }

    [Benchmark]
    public List<SensorReading> GetAllHistory()
    {
        List<int> sensorIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        DateTimeOffset from = DateTime.MinValue;
        DateTimeOffset to = DateTime.MaxValue;

        using (var context = new SensorContext(_contextBuilder.Options))
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

        using (var context = new SensorContext(_contextBuilder.Options))
        {
            var readings = context.SensorReadings
                .AsNoTracking()
                .Where(s => sensorIds.Contains(s.SensorId) && (s.DateTime >= from && s.DateTime <= to))
                .ToList();

            return readings ?? new List<SensorReading>();
        }
    }
}
