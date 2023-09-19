namespace SensorMonitoring.Shared.Models;
public class Sensor
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public IPAddress IpAddress { get; private set; }
    public string Description { get; private set; }
    public float Delta { get; private set; }

    private Sensor(string name, string description, IPAddress ipAddress, float delta)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IpAddress = ipAddress;
        Delta = delta;
    }

    public Result Update(string name, string description, IPAddress ipAddress, float delta)
    {
        var validationCheck = ValidationChecks(name, description, ipAddress, delta);
        if (validationCheck.IsFailed)
        {
            return Result.Fail(validationCheck.Errors);
        }

        Name = name;
        Delta = delta;
        Description = description;
        IpAddress = ipAddress;

        return Result.Ok();
    }

    public static Result<Sensor> Create(string name, string description, IPAddress ipAddress, float delta)
    {
        var validationCheck = ValidationChecks(name, description, ipAddress, delta);
        if (validationCheck.IsFailed)
        {
            return Result.Fail(validationCheck.Errors);
        }

        var newSensor = new Sensor(name, description, ipAddress, delta);

        return Result.Ok(newSensor);
    }

    private static Result ValidationChecks(string name, string description, IPAddress ipAddress, float delta)
    {   
        var nameCheck = IsValidName(name);
        var deltaCheck = IsValidDelta(delta);

        return Result.Merge(nameCheck, deltaCheck);
    }

    private static Result IsValidName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Fail(new InvalidSensorError("Sensor name cannot be blank"));
        }

        return Result.Ok();
    }
    
    private static Result IsValidDelta(float delta)
    {
        if (delta < 0)
        {
            return Result.Fail(new InvalidSensorError("Delta cannot be a negative number"));
        }

        return Result.Ok();
    }
}
