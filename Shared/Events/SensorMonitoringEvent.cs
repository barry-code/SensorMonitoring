namespace SensorMonitoring.Shared.Events;
public abstract class SensorMonitoringEvent : EventArgs
{
	public SensorMonitoringEvent()
	{
        TimeStamp = DateTimeOffset.Now;
	}

    public DateTimeOffset TimeStamp { get; }
}
