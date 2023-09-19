using FluentResults;
using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Shared.Errors;
using SensorMonitoring.Shared.Interfaces;
using SensorMonitoring.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorMonitoring.Api.Repository;
public class SensorRepository : ISensorRepository
{
    private SensorContext _context;
    
    public SensorRepository()
    {
        _context = new SensorContext();
    }

    public Result AddSensor(Sensor sensor)
    {
        var existingSensor = _context.Sensors.FirstOrDefault(s => s.Name.Equals(sensor.Name, StringComparison.InvariantCultureIgnoreCase));

        if (existingSensor is not null)
        {
            return Result.Fail(new SensorAlreadyExistsError(sensor.Name));
        }

        _context.Sensors.Add(sensor);
        _context.SaveChanges();

        return Result.Ok();

    }

    public Result AddSensorReading(SensorReading reading)
    {
        var sensor = _context.Sensors.FirstOrDefault(s => s.Id.Equals(reading.SensorId));

        if (sensor is null)
        {
            return Result.Fail(new SensorNotFoundError(reading.SensorId));
        }

        var lastReadingResult = GetLastNSensorReadingsForSensor(reading.SensorId, 1);

        if (lastReadingResult.IsFailed)
        {
            return Result.Fail(lastReadingResult.Errors);
        }

        var lastReadingValue = lastReadingResult?.Value?.FirstOrDefault()?.Value;

        if (lastReadingValue is null)
        {
            _context.SensorReadings.Add(reading);
            _context.SaveChanges();
            return Result.Ok();
        }

        if (Math.Abs((float)lastReadingValue - reading.Value) < sensor.Delta)
            return Result.Ok();

        _context.SensorReadings.Add(reading);
        _context.SaveChanges();

        return Result.Ok();
    }

    public Result DeleteSensor(Guid sensorId)
    {
        var existingSensor = _context.Sensors.FirstOrDefault(s => s.Id == sensorId);

        if (existingSensor is null)
        {
            return Result.Fail(new SensorNotFoundError(sensorId));
        }

        _context.Sensors.Remove(existingSensor);
        _context.SaveChanges();

        return Result.Ok();
    }

    public Result<IEnumerable<SensorReading>> GetAllSensorReadingsForSensor(Guid sensorId)
    {
        return _context.SensorReadings.Where(s => s.SensorId.Equals(sensorId)).ToArray();
    }

    public Result<IEnumerable<Sensor>> GetAllSensors()
    {
        return _context.Sensors.ToArray();
    }

    public Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForAllSensors(int count)
    {
        var sensorReadings = new List<SensorReading>();

        foreach (var sensor in _context.Sensors)
        {
            sensorReadings.AddRange(_context.SensorReadings.OrderByDescending(sr => sr.DateTime).Take(count));
        }

        return sensorReadings;
    }

    public Result<IEnumerable<SensorReading>> GetLastNSensorReadingsForSensor(Guid sensorId, int count)
    {
        return Result.Ok(_context.SensorReadings.Where(s => s.SensorId == sensorId).OrderByDescending(s => s.DateTime).ToArray().Take(count));
    }

    public Result<Sensor> GetSensorById(Guid sensorId)
    {
        var sensor = _context.Sensors.FirstOrDefault(s => s.Id.Equals(sensorId));

        if (sensor is null)
        {
            return Result.Fail(new SensorNotFoundError(sensorId));
        }

        return sensor;
    }

    public Result UpdateSensor(Sensor sensor)
    {
        var existingSensor = _context.Sensors.FirstOrDefault(s => s.Id == sensor.Id);

        if (existingSensor is null)
        {
            return Result.Fail(new SensorNotFoundError(sensor.Id));
        }

        existingSensor.Update(sensor.Name, sensor.Description, sensor.IpAddress, sensor.Delta);
        _context.SaveChanges();

        return Result.Ok();
    }
}
