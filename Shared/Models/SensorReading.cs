namespace SensorMonitoring.Shared.Models;
public class SensorReading
{
    public Guid Id { get; private set; }
    public Guid SensorId { get; private set; }
    public float Value { get; private set; }
    public DateTimeOffset DateTime { get; private set; }

    public Sensor Sensor { get; set; }

    private SensorReading(Guid sensorId, float value)
    {
        Id = Guid.NewGuid();
        SensorId = sensorId;
        DateTime = DateTimeOffset.Now;
        Value = value;
    }

    public static Result<SensorReading> Create(Guid sensorId, float value)
    {
        var newSensorReading = new SensorReading(sensorId, value);

        return Result.Ok(newSensorReading);
    }
}
