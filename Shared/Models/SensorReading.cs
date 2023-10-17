namespace SensorMonitoring.Shared.Models;
public class SensorReading
{
    public int Id { get; private set; }
    public int SensorId { get; private set; }
    public float Value { get; private set; }
    public DateTimeOffset DateTime { get; private set; }

    public Sensor? Sensor { get; set; }

    public SensorReading(int sensorId, float value)
    {
        Id = -1;
        SensorId = sensorId;
        DateTime = DateTimeOffset.Now;
        Value = value;
    }
}
