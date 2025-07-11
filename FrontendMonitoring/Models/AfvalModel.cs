﻿namespace FrontendMonitoring.Models
{
    public class AfvalModel
    {
        public Guid? Id { get; set; }

        public DateTime? Time { get; set; }
        public string? TrashType { get; set; }    
        
        public string? Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool Verified { get; set; }
        public bool Cleaned { get; set; }
        public double Confidence { get; set; }
        public DateTime? CleanedTime { get; set; } // Timestamp when cleaned

        public double? Temperature { get; set; }
    }
}