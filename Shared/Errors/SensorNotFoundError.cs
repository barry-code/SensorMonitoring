namespace SensorMonitoring.Shared.Errors;
public class SensorNotFoundError : BaseSensorMonitoringError
{
    public SensorNotFoundError(Guid id) : base($"Sensor not found with id {id}")
    {
    }
}
