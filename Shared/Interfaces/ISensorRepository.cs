namespace Shared.Interfaces;
public interface ISensorRepository
{
    Result AddSensor(Sensor sensor);
    Result DeleteSensor(Sensor sensor);
    Result UpdateSensor(Sensor sensor);
    Result<IEnumerable<Sensor>> GetAllSensors();
    Result<Sensor> GetSensorById(Guid sensorId);

    Result AddSensorReading(Sensor sensor, SensorReading reading);
    Result<IEnumerable<SensorReading>> GetAllSensorReadingsForSensor(Sensor sensor);
    Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForSensor(Sensor sensor, int count);
    Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForAllSensors(int count);
}
