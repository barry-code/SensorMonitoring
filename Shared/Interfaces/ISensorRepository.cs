namespace SensorMonitoring.Shared.Interfaces;
public interface ISensorRepository
{
    Result AddSensor(Sensor sensor);
    Result DeleteSensor(Guid sensorId);
    Result UpdateSensor(Sensor sensor);
    Result<IEnumerable<Sensor>> GetAllSensors();
    Result<Sensor> GetSensorById(Guid sensorId);

    Result AddSensorReading(SensorReading reading);
    Result<IEnumerable<SensorReading>> GetAllSensorReadingsForSensor(Guid sensorId);
    Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForSensor(Guid sensorId, int count);
    Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForAllSensors(int count);
}
