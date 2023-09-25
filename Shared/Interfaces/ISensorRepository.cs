namespace SensorMonitoring.Shared.Interfaces;
public interface ISensorRepository
{
    void AddSensor(Sensor sensor);
    void DeleteSensor(Guid sensorId);
    void UpdateSensor(Sensor sensor);
    IEnumerable<Sensor> GetAllSensors();
    Sensor GetSensorById(Guid sensorId);

    void AddSensorReading(SensorReading reading);
    IEnumerable<SensorReading> GetAllSensorReadingsForSensor(Guid sensorId);
    IEnumerable<SensorReading> GetLastNSensorReadingsForSensor(Guid sensorId, int count);
    IEnumerable<SensorReading> GetLastNSensorReadingsForAllSensors(int count);
}
