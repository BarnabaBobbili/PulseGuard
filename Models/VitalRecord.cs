namespace PulseGuard.Models
{
    public class VitalRecord
    {
        public DateTime Timestamp { get; set; }

        public double EcgValue { get; set; }

        public double SpO2Value { get; set; }

        public int HeartRateBpm { get; set; }
    }
}
