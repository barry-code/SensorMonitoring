using SensorMonitoring.Shared.Api;
using SensorMonitoring.Shared.DTO;

namespace SensorMonitoring.BlazorServerApp.Models;

public class SensorHistory
{
    public SensorDTO Sensor { get; set; }
    public List<SensorReadingResult> Readings { get; set; }
    public ReadingTimePeriod TimePeriod { get; set; }
    public SensorType SensorType =>
        Sensor.Description.ToLower().Contains("temperature") ? SensorType.Temperature : (Sensor.Description.ToLower().Contains("humidity") ? SensorType.Humidity : SensorType.Unknown);
}
