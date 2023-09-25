namespace SensorMonitoring.Shared.Errors;
public class InvalidSensorException : BaseSensorMonitoringException
{
    public InvalidSensorException(string message) : base(message)
    {
    }
}
