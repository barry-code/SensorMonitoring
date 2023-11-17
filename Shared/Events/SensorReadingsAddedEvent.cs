namespace SensorMonitoring.Shared.Events;
public class SensorReadingsAddedEvent
{
	public SensorReadingsAddedEvent(List<SensorReading> newSensorReadings)
	{
		NewSensorReadings = newSensorReadings;
	}

	public List<SensorReading> NewSensorReadings { get; }
}
