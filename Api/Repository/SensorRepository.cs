using Microsoft.Extensions.Options;
using SensorMonitoring.Shared.Errors;
using SensorMonitoring.Shared.Events;
using SensorMonitoring.Shared.Interfaces;
using SensorMonitoring.Shared.Models;

namespace SensorMonitoring.Api.Repository;
public class SensorRepository : ISensorRepository
{
    private SensorContext _context;

    public event Action<Sensor> SensorAddedEvent;
    public event Action<Sensor> SensorUpdatedEvent;
    public event Action<List<SensorReading>> SensorReadingsAddedEvent;

    public SensorRepository(IOptions<ApiOptions> options)
    {
        _context = new SensorContext(options);
    }

    public void AddSensor(Sensor sensor)
    {
        var existingSensor = _context.Sensors.FirstOrDefault(s => s.Name.Equals(sensor.Name));

        if (existingSensor is not null)
        {
            throw new SensorAlreadyExistsException(sensor.Name);
        }

        _context.Sensors.Add(sensor);
        _context.SaveChanges();
        SensorAddedEvent?.Invoke(sensor);
    }

    public void AddSensorReading(SensorReading reading)
    {
        var readingsToAdd = GetValidReadingsToAdd(new List<SensorReading>() { reading });

        SaveSensorReadings(readingsToAdd);
    }

    public void AddSensorReadings(List<SensorReading> readings)
    {
        var readingsToAdd = GetValidReadingsToAdd(readings);

        SaveSensorReadings(readingsToAdd);
    }

    public void DeleteSensor(int sensorId)
    {
        var existingSensor = _context.Sensors.FirstOrDefault(s => s.Id == sensorId);

        if (existingSensor is null)
        {
            throw new SensorNotFoundException(sensorId);
        }

        _context.Sensors.Remove(existingSensor);
        _context.SaveChanges();
    }

    public IEnumerable<SensorReading> GetAllSensorReadingsForSensor(int sensorId)
    {
        return _context.SensorReadings
            .Where(s => s.SensorId.Equals(sensorId))
            .ToArray();
    }

    public IEnumerable<Sensor> GetAllSensors()
    {
        return _context.Sensors.ToArray();
    }

    public IEnumerable<SensorReading> GetLastNSensorReadingsForAllSensors(int count)
    {
        var allSensorReadings = new List<SensorReading>();

        foreach (var sensor in _context.Sensors)
        {
            var thisSensorReading = _context.SensorReadings
                .Where(s => s.SensorId == sensor.Id)
                .OrderByDescending(sr => sr.DateTime)
                .Take(count);

            allSensorReadings.AddRange(thisSensorReading);
        }

        return allSensorReadings;
    }

    public IEnumerable<SensorReading> GetLastNSensorReadingsForSensor(int sensorId, int count)
    {
        return _context.SensorReadings
            .Where(s => s.SensorId == sensorId)
            .OrderByDescending(s => s.DateTime)
            .ToArray()
            .Take(count);
    }

    public Sensor GetSensorById(int sensorId)
    {
        var sensor = _context.Sensors.FirstOrDefault(s => s.Id.Equals(sensorId));

        if (sensor is null)
        {
            throw new SensorNotFoundException(sensorId);
        }

        return sensor;
    }

    public void UpdateSensor(Sensor sensor)
    {
        var existingSensor = _context.Sensors.FirstOrDefault(s => s.Id == sensor.Id);

        if (existingSensor is null)
        {
            throw new SensorNotFoundException(sensor.Id);
        }

        existingSensor.Update(sensor.Name, sensor.Description, sensor.Delta);
        _context.SaveChanges();
        SensorUpdatedEvent?.Invoke(sensor);
    }

    private void SaveSensorReadings(List<SensorReading> readings)
    {
        if (readings.Count == 0)
            return;

        _context.SensorReadings.AddRange(readings);
        _context.SaveChanges();
        SensorReadingsAddedEvent?.Invoke(readings);
    }

    private List<SensorReading> GetValidReadingsToAdd(List<SensorReading> readings)
    {
        var readingsToAdd = new List<SensorReading>();

        foreach (var reading in readings)
        {
            var sensor = _context.Sensors.FirstOrDefault(s => s.Id.Equals(reading.SensorId));

            if (sensor is null)
            {
                throw new SensorNotFoundException(reading.SensorId);
            }

            var lastReading = GetLastNSensorReadingsForSensor(reading.SensorId, 1)?.FirstOrDefault();

            if (lastReading is not null && IsNewValueWithinDelta(reading.Value, (float)lastReading.Value, sensor.Delta))
            {
                continue;
            }

            readingsToAdd.Add(reading);
        }

        return readingsToAdd;
    }

    private bool IsNewValueWithinDelta(float newReading, float lastReading, float delta)
    {
        return Math.Abs(lastReading - newReading) < delta;
    }
}
