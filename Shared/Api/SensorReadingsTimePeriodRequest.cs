namespace SensorMonitoring.Shared.Api;
public class SensorReadingsTimePeriodRequest
{
    public List<int> SensorIds { get; set; } = new List<int>();
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
}
