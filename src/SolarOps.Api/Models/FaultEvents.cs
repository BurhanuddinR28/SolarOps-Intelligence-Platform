namespace SolarOps.Api.Models
{
    public class FaultEvent
    {
        public int FaultId {get; set;}
        public string SubId {get; set; } = string.Empty;
        public DateTime Timestamp {get; set; }
        public string FaultReason {get; set; } = string.Empty;
        public string ToolName {get; set; } = string.Empty;
        public string ToolLine {get; set; } = string.Empty;
    }
}