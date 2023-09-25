namespace SensorMonitoring.Shared.Events;
public class SensorReadingAddedEvent
{
	public SensorReadingAddedEvent(SensorReading newSensorReading)
	{
		NewSensorReading = newSensorReading;
	}

	public SensorReading NewSensorReading { get; }
}
