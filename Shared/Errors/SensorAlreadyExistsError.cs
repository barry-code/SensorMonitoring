namespace Shared.Errors;
public class SensorAlreadyExistsError : BaseSensorMonitoringError
{
    public SensorAlreadyExistsError(string sensorName) : base($"Sensor already exists with the name: {sensorName}")
    {
    }
}
