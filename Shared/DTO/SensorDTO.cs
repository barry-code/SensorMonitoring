namespace SensorMonitoring.Shared.DTO;
public class SensorDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public float Delta { get; set; }
}
