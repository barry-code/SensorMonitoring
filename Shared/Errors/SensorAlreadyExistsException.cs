namespace SensorMonitoring.Shared.Errors;
public class SensorAlreadyExistsException : BaseSensorMonitoringException
{
    public SensorAlreadyExistsException(string sensorName) : base($"Sensor already exists with the name: {sensorName}")
    {
    }
}
