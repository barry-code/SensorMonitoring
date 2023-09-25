namespace SensorMonitoring.Shared.Events;
public class SensorUpdatedEvent
{
    public SensorUpdatedEvent(Sensor sensor)
    {
        UpdatedSensor = sensor;
    }

    public Sensor UpdatedSensor { get; set; }
}
