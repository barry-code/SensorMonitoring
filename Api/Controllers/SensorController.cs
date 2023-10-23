using Microsoft.AspNetCore.Mvc;
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

    [HttpPost(Name ="AddSensor")]
    public IActionResult AddSensor([FromForm] string name, [FromForm] string description, [FromForm] float delta)
    {
        var sensor = new Sensor(name, description, delta);

        _sensorRepository.AddSensor(sensor);

        return Ok();
    }
}
