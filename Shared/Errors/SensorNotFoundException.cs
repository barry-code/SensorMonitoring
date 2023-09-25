namespace SensorMonitoring.Shared.Errors;
public class SensorNotFoundException : BaseSensorMonitoringException
{
    public SensorNotFoundException(Guid id) : base($"Sensor not found with id {id}")
    {
    }
}
