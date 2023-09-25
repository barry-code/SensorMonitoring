namespace SensorMonitoring.Shared.Models;
public class Sensor
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public string IpAddress { get; private set; }
    public string Description { get; private set; }
    public float Delta { get; private set; }

    public Sensor(string name, string description, string ipAddress, float delta)
    {
        PerformValidationChecks(name, description, ipAddress, delta);

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IpAddress = ipAddress;
        Delta = delta;
    }

    public void Update(string name, string description, string ipAddress, float delta)
    {
        PerformValidationChecks(name, description, ipAddress, delta);
        
        Name = name;
        Description = description;
        IpAddress = ipAddress;
        Delta = delta;
    }

    private void PerformValidationChecks(string name, string description, string ipAddress, float delta)
    {   
        IsValidName(name);
        IsValidDelta(delta);
    }

    private bool IsValidName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new InvalidSensorException("Sensor name cannot be blank");
        }

        return true;
    }
    
    private bool IsValidDelta(float delta)
    {
        if (delta < 0)
        {
            throw new InvalidSensorException("Delta cannot be a negative number");
        }

        return true;
    }
}
