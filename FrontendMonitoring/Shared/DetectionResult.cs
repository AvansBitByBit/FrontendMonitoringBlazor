namespace FrontendMonitoring.Shared
{

        
    public class DetectionResult
    {
        public string Type { get; set; }
        public float Confidence { get; set; }
        public string Location { get; set; }
        public DateTime Time { get; set; }
        public bool Verified { get; set; }
        public bool Cleaned { get; set; }
    }
}
