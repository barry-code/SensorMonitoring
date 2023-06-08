namespace Shared.Errors;
public class InvalidSensorError : BaseSensorMonitoringError
{
    public InvalidSensorError(string message) : base(message)
    {
    }
}
