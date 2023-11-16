using SensorMonitoring.Shared.DTO;

namespace SensorMonitoring.BlazorServerApp.Models;

public class SensorSummary
{
    public SensorDTO Sensor { get; set; }
    public float LastReading { get; set; }
    public DateTimeOffset LastReadingTime { get; set; }
    public bool IsCommsLost => LastReadingTime < DateTimeOffset.UtcNow.AddHours(-2);
}
