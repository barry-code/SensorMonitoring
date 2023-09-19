using Microsoft.AspNetCore.Mvc;
using SensorMonitoring.Shared.Interfaces;

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

    [HttpGet(Name = "GetSensors")]
    public List<string> Get()
    {
        return new List<string> { "123", "234", "345" };
    }
}
