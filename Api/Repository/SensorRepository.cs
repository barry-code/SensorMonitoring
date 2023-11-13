using Microsoft.Extensions.Options;
using SensorMonitoring.Shared.Errors;
using SensorMonitoring.Shared.Interfaces;
using SensorMonitoring.Shared.Models;

namespace SensorMonitoring.Api.Repository;
public class SensorRepository : ISensorRepository
{
    private SensorContext _context;
    
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
    }

    public void AddSensorReading(SensorReading reading)
    {
        var sensor = _context.Sensors.FirstOrDefault(s => s.Id.Equals(reading.SensorId));

        if (sensor is null)
        {
            throw new SensorNotFoundException(reading.SensorId);
        }

        var lastReading = GetLastNSensorReadingsForSensor(reading.SensorId, 1)?.FirstOrDefault();

        if (lastReading is null)
        {
            _context.SensorReadings.Add(reading);
            _context.SaveChanges();
            return;
        }

        if (Math.Abs((float)lastReading.Value - reading.Value) < sensor.Delta)
            return;

        _context.SensorReadings.Add(reading);
        _context.SaveChanges();
    }

    public void AddSensorReadings(List<SensorReading> readings)
    {
        foreach (var item in readings)
        {
            AddSensorReading(item);
        }        
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
        var sensorReadings = new List<SensorReading>();

        foreach (var sensor in _context.Sensors)
        {
            sensorReadings
                .AddRange(_context.SensorReadings
                .OrderByDescending(sr => sr.DateTime)
                .Take(count));
        }

        return sensorReadings;
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
    }
}
