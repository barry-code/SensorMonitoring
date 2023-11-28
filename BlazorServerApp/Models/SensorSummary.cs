using SensorMonitoring.Shared.DTO;

namespace SensorMonitoring.BlazorServerApp.Models;

public class SensorSummary
{
    public SensorDTO Sensor { get; set; }
    public float LastReading { get; set; }
    public DateTimeOffset LastReadingTime { get; set; }
    public bool HasNoReadings => LastReadingTime == DateTimeOffset.MinValue;
    public bool IsCommsLost => LastReadingTime < DateTimeOffset.UtcNow.AddHours(-2);
    public string LastReadingDisplay => HasNoReadings ? "" : (Sensor.Description.ToLower().Contains("humidity") ? $"{LastReading} %" : $"{LastReading} °C");
    public string LastReadingTimeDisplay => HasNoReadings ? "" : LastReadingTime.ToLocalTime().ToString("ddd-MMM-yyyy HH:mm:ss");
    public string ReadingIconName => Sensor.Description.ToLower().Contains("humidity") ? "water_drop" : "thermostat";
}
