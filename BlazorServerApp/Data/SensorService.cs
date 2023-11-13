using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SensorMonitoring.BlazorServerApp.Pages;
using SensorMonitoring.Shared.DTO;
using SensorMonitoring.Shared.Models;
//using System.Text.Json;

namespace SensorMonitoring.BlazorServerApp.Data;

public class SensorService
{
    private readonly string baseUri;

    public SensorService(IOptions<ServerAppOptions> options)
    {
        baseUri = options.Value.SensorApiUrl;
    }

    public async Task<List<SensorDTO>> GetSensors()
    {
        var sensors = new List<SensorDTO>();

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(baseUri);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var resultStr = await response.Content.ReadAsStringAsync();

                    var sensorsFound = JsonConvert.DeserializeObject<List<SensorDTO>>(resultStr);

                    if (sensorsFound is null)
                        return new List<SensorDTO>();

                    sensors.AddRange(sensorsFound);
                }
                catch (Exception ex)
                {

                    throw;
                }
                
            }         
        }

        return sensors;
    }

    public async Task AddSensor()
    {
        await Task.Delay(1000);
        //using (var client = new HttpClient())
        //{
        //    var response = await client.PostAsJsonAsync(baseUri);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var resultStr = await response.Content.ReadAsStringAsync();

        //        var sensorsFound = JsonConvert.DeserializeObject<List<Sensor>>(resultStr);

        //        if (sensorsFound is null)
        //            return new List<Sensor>();

        //        sensors.AddRange(sensorsFound);
        //    }
        //}
    }
}
