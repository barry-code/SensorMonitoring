using Newtonsoft.Json;
using SensorMonitoring.Shared.Models;
//using System.Text.Json;

namespace SensorMonitoring.BlazorServerApp.Data;

public class SensorService
{
    private readonly string baseUri = @"https://localhost:44388/Sensor";
    public async Task<List<Sensor>> GetSensors()
    {
        var sensors = new List<Sensor>();

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(baseUri);

            if (response.IsSuccessStatusCode)
            {
                var resultStr = await response.Content.ReadAsStringAsync();

                var sensorsFound = JsonConvert.DeserializeObject<List<Sensor>>(resultStr);
                
                if (sensorsFound is null)
                    return new List<Sensor>();

                sensors.AddRange(sensorsFound);
            }         
        }

        return sensors;
    }
}
