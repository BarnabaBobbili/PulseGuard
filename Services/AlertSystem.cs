using PulseGuard.Models;

namespace PulseGuard.Services
{
    public class AlertSystem
    {
        private const int HeartRateUpperLimit = 120;
        private const double SpO2LowerLimit = 90.0;

        public AlertState Evaluate(VitalRecord record)
        {
            bool hrAlert = record.HeartRateBpm > HeartRateUpperLimit;
            bool spo2Alert = record.SpO2Value < SpO2LowerLimit;

            if (hrAlert && spo2Alert)
            {
                return new AlertState
                {
                    IsAlerting = true,
                    Message = $"⚠ MULTIPLE ALERTS: HR {record.HeartRateBpm} BPM | SpO2 {record.SpO2Value:F1}%",
                    AlertType = "Multiple"
                };
            }
            else if (hrAlert)
            {
                return new AlertState
                {
                    IsAlerting = true,
                    Message = $"⚠ HEART RATE CRITICAL: {record.HeartRateBpm} BPM",
                    AlertType = "HighHeartRate"
                };
            }
            else if (spo2Alert)
            {
                return new AlertState
                {
                    IsAlerting = true,
                    Message = $"⚠ SpO2 CRITICAL: {record.SpO2Value:F1}%",
                    AlertType = "LowSpO2"
                };
            }
            else
            {
                return new AlertState
                {
                    IsAlerting = false,
                    Message = "✓ All Vitals Normal",
                    AlertType = "None"
                };
            }
        }
    }
}
