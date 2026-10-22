namespace SolarOps.Ingestion.Models
{
    public class IVCurve
    {
        public int IVId {get; set;}
        public int ReadingId {get; set;}
        public double? Voltage {get; set; }
        public double? CurrentAmp {get; set; }
    }
}