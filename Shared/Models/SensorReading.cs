namespace SensorMonitoring.Shared.Models;
public class SensorReading
{
    public int Id { get; set; }
    public int SensorId { get; set; }
    public float Value { get; set; }
    public DateTimeOffset DateTime { get; set; }

    public Sensor? Sensor { get; set; }

    public SensorReading(int sensorId, float value)
    {
        SensorId = sensorId;
        DateTime = DateTimeOffset.Now;
        Value = value;
    }
}
