namespace FrontendMonitoring.Models
{
    public class AfvalModel
    {
        public Guid? Id { get; set; }
        
        public DateTime? Time { get; set; }
        public string? TrashType { get; set; }
        
        public string? Location { get; set; }
        public bool Verified { get; set; }
        public bool Cleaned { get; set; }
        public double Confidence { get; set; }
    }
}