namespace SensorMonitoring.BlazorServerApp;

public class ServerAppOptions
{
    public string SensorApiUrl { get; set; } = string.Empty;
    public bool FilterSensors { get; set; }
    public List<int> SensorsToShow { get; set; } = new();
}
