namespace SensorMonitoring.Shared.Events;
public class SensorAddedEvent : SensorMonitoringEvent
{
	public SensorAddedEvent(Sensor newSensor)
	{
		NewSensor = newSensor;
	}

    public Sensor NewSensor { get; }
}
