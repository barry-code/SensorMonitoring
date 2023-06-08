using FluentResults;
using Shared.Errors;
using Shared.Interfaces;
using Shared.Models;
using System.Linq;

namespace TestConsoleApp;
internal class SensorRepository : ISensorRepository
{
    private List<Sensor> _sensors = new();
    private List<SensorReading> _sensorReadings = new();

    public Result AddSensor(Sensor sensor)
    {
        var existingSensor = _sensors.FirstOrDefault(s => s.Name.Equals(sensor.Name, StringComparison.InvariantCultureIgnoreCase));

        if (existingSensor is not null)
        {
            return Result.Fail(new SensorAlreadyExistsError(sensor.Name));
        }

        _sensors.Add(sensor);

        return Result.Ok();
    }

    public Result DeleteSensor(Sensor sensor)
    {
        var existingSensor = _sensors.FirstOrDefault(s => s.Id == sensor.Id);

        if (existingSensor is null)
        {
            return Result.Fail(new SensorNotFoundError(sensor.Id));
        }

        _sensors.Remove(existingSensor);

        return Result.Ok();
    }

    public Result UpdateSensor(Sensor sensor)
    {
        var existingSensor = _sensors.FirstOrDefault(s => s.Id == sensor.Id);

        if (existingSensor is null)
        {
            return Result.Fail(new SensorNotFoundError(sensor.Id));
        }

        existingSensor.Update(sensor.Name, sensor.Description, sensor.IpAddress, sensor.Delta);
        
        return Result.Ok();
    }

    public Result<IEnumerable<Sensor>> GetAllSensors()
    {
        return _sensors;
    }

    public Result<Sensor> GetSensorById(Guid sensorId)
    {
        var sensor = _sensors.FirstOrDefault(s => s.Id.Equals(sensorId));

        if (sensor is null)
        {
            return Result.Fail(new SensorNotFoundError(sensorId));
        }

        return sensor;
    }


    public Result AddSensorReading(Sensor sensor, SensorReading reading)
    {
        var lastReadingResult = GetLastNSensorReadingsForSensor(sensor, 1);

        if (lastReadingResult.IsFailed)
        {
            return Result.Fail(lastReadingResult.Errors);
        }

        var lastReadingValue = lastReadingResult?.Value?.FirstOrDefault()?.Value;

        if (lastReadingValue is null)
        {
            _sensorReadings.Add(reading);
            return Result.Ok();
        }

        if (Math.Abs((float)lastReadingValue - reading.Value) < sensor.Delta)
            return Result.Ok();

        _sensorReadings.Add(reading);

        return Result.Ok();
    }

    public Result<IEnumerable<SensorReading>> GetAllSensorReadingsForSensor(Sensor sensor)
    {
        return _sensorReadings.Where(s => s.SensorId.Equals(sensor.Id)).ToArray();
    }

    public Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForSensor(Sensor sensor, int count)
    {
        return Result.Ok(_sensorReadings.OrderByDescending(s => s.DateTime).Take(count));
    }
    
    public Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForAllSensors(int count)
    {
        var sensorReadings = new List<SensorReading>();

        foreach (var sensor in _sensors)
        {
            sensorReadings.AddRange(_sensorReadings.OrderByDescending(sr => sr.DateTime).Take(count));
        }

        return sensorReadings;
    }
}
