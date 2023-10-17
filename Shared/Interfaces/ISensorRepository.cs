namespace SensorMonitoring.Shared.Interfaces;
public interface ISensorRepository
{
    void AddSensor(Sensor sensor);
    void DeleteSensor(int sensorId);
    void UpdateSensor(Sensor sensor);
    IEnumerable<Sensor> GetAllSensors();
    Sensor GetSensorById(int sensorId);

    void AddSensorReading(SensorReading reading);
    IEnumerable<SensorReading> GetAllSensorReadingsForSensor(int sensorId);
    IEnumerable<SensorReading> GetLastNSensorReadingsForSensor(int sensorId, int count);
    IEnumerable<SensorReading> GetLastNSensorReadingsForAllSensors(int count);
}
