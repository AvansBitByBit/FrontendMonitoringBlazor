namespace FrontendMonitoring.Shared
{

        
    public class DetectionResult
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public float Confidence { get; set; }
        public string Location { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsVerified { get; set; }
    }
}
