using Microsoft.AspNetCore.Mvc;
using SensorMonitoring.Shared.DTO;
using SensorMonitoring.Shared.Interfaces;
using SensorMonitoring.Shared.Models;

namespace SensorMonitoring.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SensorController : ControllerBase
{
	private readonly ISensorRepository _sensorRepository;

    public SensorController(ISensorRepository sensorRepository)
	{
        _sensorRepository = sensorRepository;
    }

    [HttpGet(Name = "Sensors")]
    public IActionResult ListAllSensors()
    {
        var sensors = _sensorRepository.GetAllSensors();

        if (sensors is null)
        {
            return NoContent();
        }

        return Ok(sensors);
    }

    [Route("AddSensor")]
    [HttpPost()]
    public IActionResult AddSensor([FromForm] string name, [FromForm] string description, [FromForm] float delta)
    {
        var sensor = new Sensor(name, description, delta);

        _sensorRepository.AddSensor(sensor);

        return Ok();
    }

    [Route("AddSensorReading")]
    [HttpPost()]
    public IActionResult AddSensorReading([FromBody] SensorReadingDTO sensorReading)
    {
        var sensorReadingValue = new SensorReading(sensorReading.SensorId, sensorReading.Reading);

        _sensorRepository.AddSensorReading(sensorReadingValue);

        return Ok();
    }

    [Route("AddSensorReadings")]
    [HttpPost()]
    public IActionResult AddSensorReadings([FromBody] List<SensorReadingDTO> sensorReadings)
    {
        var sensorReadingValues = sensorReadings.Select(s => new SensorReading(s.SensorId, s.Reading)).ToList();

        _sensorRepository.AddSensorReadings(sensorReadingValues);

        return Ok();
    }

    [Route("LastNSensorReadings/{count}")]
    [HttpGet()]
    public IActionResult GetLastNSensorReadings([FromRoute]int count = 1)
    {
        var readings = _sensorRepository.GetLastNSensorReadingsForAllSensors(count);

        return Ok(readings);
    }
}
