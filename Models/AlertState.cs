namespace PulseGuard.Models
{
    public class AlertState
    {
        public bool IsAlerting { get; set; }

        public string Message { get; set; } = "All Vitals Normal";

        public string AlertType { get; set; } = "None";
    }
}
