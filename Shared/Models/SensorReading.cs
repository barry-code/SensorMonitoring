namespace Shared.Models;
public class SensorReading
{
    public Guid SensorId { get; }
    public float Value { get; }
    public DateTimeOffset DateTime { get; }

    private SensorReading(Guid sensorId, float value)
    {
        SensorId = sensorId;
        DateTime = DateTimeOffset.UtcNow;
        Value = value;
    }

    public static Result<SensorReading> Create(Guid sensorId, float value)
    {
        var newSensorReading = new SensorReading(sensorId, value);

        return Result.Ok(newSensorReading);
    }
}
