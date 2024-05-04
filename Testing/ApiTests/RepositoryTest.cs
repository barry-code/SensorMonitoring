using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SensorMonitoring.Api.Repository;
using SensorMonitoring.Shared.Errors;
using SensorMonitoring.Shared.Interfaces;
using SensorMonitoring.Shared.Models;

namespace SensorMonitoring.ApiTests;

public class RepositoryTest
{
    private readonly ISensorRepository _sensorRepository;

    public RepositoryTest(ISensorRepository sensorRepository)
    {
        _sensorRepository = sensorRepository;
    }

    [Fact]
    public void AddSensor_ShouldAddSuccessfully()
    {
        string newSensorName = "TestSensor";
        string newSensorDescription = "TestSensor1 Description";
        var newSensor = new Sensor(newSensorName, newSensorDescription, 0f);

        _sensorRepository.AddSensor(newSensor);

        var sensorsFound = _sensorRepository.GetAllSensors().Where(s => s.Name == newSensorName).ToList();

        Assert.Single<Sensor>(sensorsFound);
        Assert.Equal(sensorsFound.FirstOrDefault()?.Name ?? "", newSensor.Name);
    }

    [Fact]
    public void AddDuplicateSensor_ShouldThrowError()
    {
        string newSensorName = "TestSensor2";
        string newSensorDescription = "TestSensor2 Description";

        _sensorRepository.AddSensor(new Sensor(newSensorName, newSensorDescription, 0f));

        Assert.Throws<SensorAlreadyExistsException>(() => _sensorRepository.AddSensor(new Sensor(newSensorName, newSensorDescription, 0f)));
    }

    [Fact]
    public void AddBlankSensorName_ShouldThrowError()
    {
        string newSensorName = "";
        string newSensorDescription = "TestSensor3 Description";

        Assert.Throws<InvalidSensorException>(() => _sensorRepository.AddSensor(new Sensor(newSensorName, newSensorDescription, 0f)));
    }

    [Fact]
    public void AddSensorNegativeDelta_ShouldThrowError()
    {
        string newSensorName = "TestSensor4";
        string newSensorDescription = "TestSensor4 Description";
        float negativeDelta = -1f;

        Assert.Throws<InvalidSensorException>(() => _sensorRepository.AddSensor(new Sensor(newSensorName, newSensorDescription, negativeDelta)));
    }

    [Fact]
    public void GetSensorById_ShouldReturnSensor()
    {
        string newSensorName = "TestSensor1a";
        string newSensorDescription = "TestSensor1a Description";
        var newSensor = new Sensor(newSensorName, newSensorDescription, 0f);

        _sensorRepository.AddSensor(newSensor);

        var sensorAdded = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName);

        int id = sensorAdded.Id;

        var foundSensor = _sensorRepository.GetSensorById(id);

        Assert.NotNull(foundSensor);
        Assert.Equal(foundSensor.Name ?? "", newSensorName);
        Assert.Equal(foundSensor.Id, id);
    }

    [Fact]
    public void DeleteSensor_ShouldBeDeleted()
    {
        string newSensorName = "TestSensor1b";
        string newSensorDescription = "TestSensor1b Description";
        var newSensor = new Sensor(newSensorName, newSensorDescription, 0f);

        _sensorRepository.AddSensor(newSensor);

        var sensorAdded = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName);

        int id = sensorAdded.Id;

        _sensorRepository.DeleteSensor(id);

        Assert.Throws<SensorNotFoundException>(() => _sensorRepository.GetSensorById(id));
    }

    [Fact]
    public void UpdateSensor_ShouldBeUpdated()
    {
        string newSensorName = "TestSensor1c";
        string newSensorDescription = "TestSensor1c Description";
        var newSensor = new Sensor(newSensorName, newSensorDescription, 0f);

        _sensorRepository.AddSensor(newSensor);

        var sensorAdded = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName);
        int sensorId = sensorAdded.Id;

        string newName = "NewTestSensor1c";
        string newDesc = "NewTestSensor1c Desc";
        float newDelta = 10.0f;

        sensorAdded.Update(newName, newDesc, newDelta);

        _sensorRepository.UpdateSensor(sensorAdded);

        var updatedSensor = _sensorRepository.GetSensorById(sensorId);

        Assert.Equal(updatedSensor.Name ?? "", newName);
        Assert.Equal(updatedSensor.Description ?? "", newDesc);
        Assert.Equal(updatedSensor.Delta, newDelta);
    }

    [Fact]
    public void AddSingleSensorReading_ShouldAddSuccessfully()
    {
        string newSensorName = "TestSensor5";
        string newSensorDescription = "TestSensor5 Description";
        var newSensor = new Sensor(newSensorName, newSensorDescription, 0f);
        float newReadingValue = 50.1f;
        
        _sensorRepository.AddSensor(newSensor);

        var sensorAdded = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName);

        var newSensorReading = new SensorReading(sensorAdded.Id, newReadingValue);

        _sensorRepository.AddSensorReading(newSensorReading);

        var readingFound = _sensorRepository.GetAllSensorReadingsForSensor(sensorAdded.Id).ToList();

        Assert.Single<SensorReading>(readingFound);
        Assert.Equal(readingFound.FirstOrDefault()?.SensorId, sensorAdded.Id);
        Assert.Equal(readingFound.FirstOrDefault()?.Value, newReadingValue);
    }

    [Fact]
    public void AddMultipleSensorReadings_ShouldAddSuccessfully()
    {
        string newSensorName1 = "TestSensor6";
        string newSensorDescription1 = "TestSensor6 Description";
        string newSensorName2 = "TestSensor7";
        string newSensorDescription2 = "TestSensor7 Description";
        var newSensor1 = new Sensor(newSensorName1, newSensorDescription1, 0f);
        var newSensor2 = new Sensor(newSensorName2, newSensorDescription2, 0f);
        float[] sensor1ReadingValues = new float[] { 40.1f, 40.2f, 40.3f, 40.4f };
        int sensor1ReadingsCount = sensor1ReadingValues.Count();
        float[] sensor2ReadingValues = new float[] { 60.1f, 60.2f, 60.3f};
        int sensor2ReadingsCount = sensor2ReadingValues.Count();

        _sensorRepository.AddSensor(newSensor1);
        _sensorRepository.AddSensor(newSensor2);

        var sensor1Added = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName1);
        var sensor2Added = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName2);

        List<SensorReading> readings = new();

        foreach (var reading in sensor1ReadingValues)
        {
            readings.Add(new SensorReading(sensor1Added.Id, reading));
        }

        foreach (var reading in sensor2ReadingValues)
        {
            readings.Add(new SensorReading(sensor2Added.Id, reading));
        }

        _sensorRepository.AddSensorReadings(readings);

        var s1ReadingsFound = _sensorRepository.GetAllSensorReadingsForSensor(sensor1Added.Id).ToList();
        var s2ReadingsFound = _sensorRepository.GetAllSensorReadingsForSensor(sensor2Added.Id).ToList();

        Assert.Equal(s1ReadingsFound.Count, sensor1ReadingsCount);
        Assert.Equal(s2ReadingsFound.Count, sensor2ReadingsCount);
    }

    [Fact]
    public void AddSensorReadingWithinDelta_ShouldNotAddSuccessfully()
    {
        string newSensorName = "TestSensor8";
        string newSensorDescription = "TestSensor8 Description";
        var newSensor = new Sensor(newSensorName, newSensorDescription, 5f);
        float firstReadingValue = 50f;
        float secondReadingValue = 54f;

        _sensorRepository.AddSensor(newSensor);

        var sensorAdded = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName);

        var firstSensorReading = new SensorReading(sensorAdded.Id, firstReadingValue);
        var secondSensorReading = new SensorReading(sensorAdded.Id, secondReadingValue);

        _sensorRepository.AddSensorReading(firstSensorReading);
        _sensorRepository.AddSensorReading(secondSensorReading);

        var readingsFound = _sensorRepository.GetAllSensorReadingsForSensor(sensorAdded.Id).ToList();

        Assert.Equal(readingsFound?.Count, 1);
    }

    [Fact]
    public void AddSensorReadingOutsideDelta_ShouldAddSuccessfully()
    {
        string newSensorName = "TestSensor9";
        string newSensorDescription = "TestSensor9 Description";
        var newSensor = new Sensor(newSensorName, newSensorDescription, 3f);
        float firstReadingValue = 50f;
        float secondReadingValue = 54f;

        _sensorRepository.AddSensor(newSensor);

        var sensorAdded = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName);

        var firstSensorReading = new SensorReading(sensorAdded.Id, firstReadingValue);
        var secondSensorReading = new SensorReading(sensorAdded.Id, secondReadingValue);

        _sensorRepository.AddSensorReading(firstSensorReading);
        _sensorRepository.AddSensorReading(secondSensorReading);

        var readingsFound = _sensorRepository.GetAllSensorReadingsForSensor(sensorAdded.Id).ToList();

        Assert.Equal(readingsFound?.Count, 2);
    }

    [Fact]
    public void GetLastNSensorReadings_ShouldReturnNReadings()
    {
        string newSensorName1 = "TestSensor10";
        string newSensorDescription1 = "TestSensor10 Description";
        string newSensorName2 = "TestSensor11";
        string newSensorDescription2 = "TestSensor11 Description";
        var newSensor1 = new Sensor(newSensorName1, newSensorDescription1, 0f);
        var newSensor2 = new Sensor(newSensorName2, newSensorDescription2, 0f);
        float[] sensor1ReadingValues = new float[] { 40.1f, 40.2f, 40.3f };
        int sensor1ReadingsCount = sensor1ReadingValues.Count();
        float[] sensor2ReadingValues = new float[] { 60.1f, 60.2f, 60.3f };
        int sensor2ReadingsCount = sensor2ReadingValues.Count();
        int countN = 2;

        _sensorRepository.AddSensor(newSensor1);
        _sensorRepository.AddSensor(newSensor2);

        var sensor1Added = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName1);
        var sensor2Added = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName2);

        List<SensorReading> readings = new();

        foreach (var reading in sensor1ReadingValues)
        {
            readings.Add(new SensorReading(sensor1Added.Id, reading));
        }

        foreach (var reading in sensor2ReadingValues)
        {
            readings.Add(new SensorReading(sensor2Added.Id, reading));
        }

        _sensorRepository.AddSensorReadings(readings);

        var s1ReadingsFound = _sensorRepository.GetLastNSensorReadingsForSensor(sensor1Added.Id, countN).ToList();
        var allReadingsFound = _sensorRepository.GetLastNSensorReadingsForAllSensors(countN).ToList();
        var s2ReadingsFound = allReadingsFound.Where(s => s.SensorId == sensor2Added.Id).ToList();

        Assert.Equal(s1ReadingsFound.Count, countN);
        Assert.Contains<float>(sensor1ReadingValues[2], s1ReadingsFound.Select(sr => sr.Value));
        Assert.Contains<float>(sensor1ReadingValues[1], s1ReadingsFound.Select(sr => sr.Value));
        Assert.Equal(s2ReadingsFound.Count, countN);
        Assert.Contains<float>(sensor2ReadingValues[2], s2ReadingsFound.Select(sr => sr.Value));
        Assert.Contains<float>(sensor2ReadingValues[1], s2ReadingsFound.Select(sr => sr.Value));
    }

    [Fact]
    public void GetSensorReadingsTimestamps_ShouldReturnReadings()
    {
        string newSensorName1 = "TestSensor12";
        string newSensorDescription1 = "TestSensor12 Description";
        string newSensorName2 = "TestSensor13";
        string newSensorDescription2 = "TestSensor13 Description";
        var newSensor1 = new Sensor(newSensorName1, newSensorDescription1, 0f);
        var newSensor2 = new Sensor(newSensorName2, newSensorDescription2, 0f);
        float[] sensor1ReadingValues = new float[] { 40.1f, 40.2f, 40.3f };
        int sensor1ReadingsCount = sensor1ReadingValues.Count();
        float[] sensor2ReadingValues = new float[] { 60.1f, 60.2f };
        int sensor2ReadingsCount = sensor2ReadingValues.Count();

        _sensorRepository.AddSensor(newSensor1);
        _sensorRepository.AddSensor(newSensor2);

        var sensor1Added = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName1);
        var sensor2Added = _sensorRepository.GetAllSensors().FirstOrDefault(s => s.Name == newSensorName2);

        List<SensorReading> readings = new();

        foreach (var reading in sensor1ReadingValues)
        {
            readings.Add(new SensorReading(sensor1Added.Id, reading));
        }

        foreach (var reading in sensor2ReadingValues)
        {
            readings.Add(new SensorReading(sensor2Added.Id, reading));
        }

        DateTimeOffset startingTime = DateTimeOffset.UtcNow.AddSeconds(-5);

        int sentReadings = readings.Count;

        _sensorRepository.AddSensorReadings(readings);

        var readingsFound = _sensorRepository.GetSensorReadingsForSensors(new List<int> { sensor1Added.Id, sensor2Added.Id }, startingTime, DateTimeOffset.UtcNow.AddSeconds(5)).ToList();

        Assert.Equal(readingsFound.Count, sentReadings);
        Assert.Contains<int>(sensor1Added.Id, readingsFound.Select(sr => sr.SensorId));
        Assert.Contains<int>(sensor2Added.Id, readingsFound.Select(sr => sr.SensorId));
    }
}