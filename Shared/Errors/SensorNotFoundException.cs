namespace SensorMonitoring.Shared.Errors;
public class SensorNotFoundException : BaseSensorMonitoringException
{
    public SensorNotFoundException(int id) : base($"Sensor not found with id {id}")
    {
    }
}
