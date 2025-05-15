namespace Frondendmonitoring.Shared
{

        
    public class DetectionResult
    {
        public string Type { get; set; }
        public float Confidence { get; set; }
        public string Location { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
