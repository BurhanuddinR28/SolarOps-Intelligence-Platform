namespace SolarOps.Api.Models
{
    public class PanelReading
    {
        public int ReadingId {get; set; }
        public string SubId {get; set; } = string.Empty;
        public DateTime Timestamp {get; set; }
        public float? ReadTime{get; set; }
        public float? Wattage{get; set; }
        public int PlantId {get; set; }
        public int ToolId {get; set; }
        public bool? PassFail {get; set; }
        public DateTime CreatedAt {get; set; }

    }
}