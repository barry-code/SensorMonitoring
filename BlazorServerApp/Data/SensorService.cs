using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SensorMonitoring.BlazorServerApp.Models;
using SensorMonitoring.Shared.Api;
using SensorMonitoring.Shared.DTO;
using SensorMonitoring.Shared.Models;
using System.Web;

namespace SensorMonitoring.BlazorServerApp.Data;

public class SensorService
{
    private readonly string baseUri;
    private ServerAppOptions _options;

    public SensorService(IOptions<ServerAppOptions> options)
    {
        _options = options.Value;
        baseUri = _options.SensorApiUrl;
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

                    if (_options.FilterSensors)
                    {
                        sensorsFound.RemoveAll(s => !_options.SensorsToShow.Contains(s.Id));
                    }

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

    public async Task<List<SensorSummary>> GetSensorReadingSummary()
    {
        List<SensorSummary> results = new();

        var sensors = await GetSensors();

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"{baseUri}/LastNSensorReadings/1");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var resultStr = await response.Content.ReadAsStringAsync();

                    var readingsFound = JsonConvert.DeserializeObject<List<SensorReading>>(resultStr);

                    foreach (var sensor in sensors)
                    {
                        var newSummary = new SensorSummary()
                        {
                            Sensor = sensor,
                            LastReading = 0f,
                            LastReadingTime = DateTimeOffset.MinValue
                        };

                        var thisSensorReading = readingsFound?.FirstOrDefault(r => r.SensorId == sensor.Id);
                        if (thisSensorReading is not null)
                        {
                            newSummary.LastReading = thisSensorReading.Value;
                            newSummary.LastReadingTime = thisSensorReading.DateTime;
                        }

                        results.Add(newSummary);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

                return results;
            }
        }

        return results;
    }

    public async Task<List<SensorHistory>> GetSensorReadingHistoryForAllSensors(ReadingTimePeriod timePeriod)
    {
        var allSensors = await GetSensors();

        var sensorIds = allSensors.Select(s => s.Id).ToList();

        var result = await GetSensorReadingHistoryForSensors(sensorIds, timePeriod);

        return result;
    }

    public async Task<List<SensorHistory>> GetSensorReadingHistoryForSensors(List<int> sensorIds, ReadingTimePeriod timePeriod)
    {
        List<SensorHistory> results = new();
        DateTimeOffset startTime;
        DateTimeOffset endTime = DateTimeOffset.Now;
        switch (timePeriod)
        {
            case ReadingTimePeriod.Day:
                startTime = DateTimeOffset.Now.AddDays(-1);
                break;
            case ReadingTimePeriod.TwoDays:
                startTime = DateTimeOffset.Now.AddDays(-2);
                break;
            case ReadingTimePeriod.ThreeDays:
                startTime = DateTimeOffset.Now.AddDays(-3);
                break;
            case ReadingTimePeriod.Week:
                startTime = DateTimeOffset.Now.AddDays(-7);
                break;
            case ReadingTimePeriod.TwoWeeks:
                startTime = DateTimeOffset.Now.AddDays(-14);
                break;
            case ReadingTimePeriod.Month:
                startTime = DateTimeOffset.Now.AddMonths(-1);
                break;
            case ReadingTimePeriod.Year:
                startTime = DateTimeOffset.Now.AddYears(-1);
                break;
            case ReadingTimePeriod.AllTime:
            default:
                startTime = DateTimeOffset.MinValue;
                break;
        }

        using (var client = new HttpClient())
        {
            var response = await client.PostAsJsonAsync($"{baseUri}/GetSensorReadingsForSensors/{UrlEncode(startTime)}/{UrlEncode(endTime)}", sensorIds);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var resultStr = await response.Content.ReadAsStringAsync();
                    var sensors = await GetSensors();
                    var filteredSensors = sensors.Where(s => sensorIds.Contains(s.Id)).ToList();
                    var readingsFound = JsonConvert.DeserializeObject<List<SensorReading>>(resultStr);

                    foreach (var sensor in filteredSensors)
                    {
                        var history = new SensorHistory()
                        {
                            Sensor = sensor,
                            Readings = readingsFound?.Where(r => r.SensorId == sensor.Id)?.Select(r => new SensorReadingResult() { SensorId = r.SensorId, SensorName = sensor.Name, ReadingValue = r.Value, DateTime = r.DateTime.LocalDateTime })?.ToList() ?? new List<SensorReadingResult>()
                        };

                        results.Add(history);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }

                return results;
            }
        }

        return results;
    }

    public static string UrlEncode(DateTimeOffset dateTimeOffset)
    {
        return HttpUtility.UrlEncode(dateTimeOffset.ToString("o"));
    }
}
