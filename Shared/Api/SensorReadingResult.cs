using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorMonitoring.Shared.Api;
public class SensorReadingResult
{
    public int SensorId { get; set; }
    public string SensorName { get; set; }
    public float ReadingValue { get; set; }
    public DateTime DateTime { get; set; }
}
