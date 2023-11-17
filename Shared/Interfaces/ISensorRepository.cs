using SensorMonitoring.Shared.Events;

namespace SensorMonitoring.Shared.Interfaces;
public interface ISensorRepository
{   
    event Action<Sensor> SensorAddedEvent;
    event Action<Sensor> SensorUpdatedEvent;
    event Action<List<SensorReading>> SensorReadingsAddedEvent;

    void AddSensor(Sensor sensor);
    void DeleteSensor(int sensorId);
    void UpdateSensor(Sensor sensor);
    IEnumerable<Sensor> GetAllSensors();
    Sensor GetSensorById(int sensorId);

    void AddSensorReading(SensorReading reading);
    void AddSensorReadings(List<SensorReading> readings);
    IEnumerable<SensorReading> GetAllSensorReadingsForSensor(int sensorId);
    IEnumerable<SensorReading> GetLastNSensorReadingsForSensor(int sensorId, int count);
    IEnumerable<SensorReading> GetLastNSensorReadingsForAllSensors(int count);
}
