namespace SolarOps.Ingestion.Models
{
    public class PanelReading
    {
        public int ReadingId {get; set; }
        public string SubId{get; set; } = string.Empty;
        public DateTime Timestamp {get; set;}
        public float? ReadTime {get; set; }
        public float? Wattage {get; set; }
        public string Plant {get; set;} = string.Empty;
        public string ToolName {get; set; } = string.Empty;
        public string ToolLine {get; set; } = string.Empty;
        public bool? PassFail {get; set; }
        public DateTime CreatedAt {get; set; }
    }
}