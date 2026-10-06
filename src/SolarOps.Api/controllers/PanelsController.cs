using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SolarOps.Api.Models;
namespace SolarOps.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PanelsController : ControllerBase
    {
        private readonly string _connectionString = "Server=localhost,1433;Database=SolarOps;User Id=sa;Password=SolarOps123;TrustServerCertificate=True;";
        [HttpGet]
        public IActionResult Get()
        {
            var readings = new List<PanelReading>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT TOP 100 ReadingId, SubId, Timestamp, ReadTime, Wattage, PlantId, ToolId, PassFail, CreatedAt FROM PanelReadings", connection);
            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                readings.Add(new PanelReading
{
    ReadingId = reader.GetInt32(0),
    SubId = reader.GetString(1),
    Timestamp = reader.GetDateTime(2),
    ReadTime = reader.IsDBNull(3) ? null : (float?)reader.GetDouble(3),
    Wattage = reader.IsDBNull(4) ? null : (float?)reader.GetDouble(4),
    PlantId = reader.GetInt32(5),
    ToolId = reader.GetInt32(6),
    PassFail = reader.IsDBNull(7) ? null : reader.GetBoolean(7),
    CreatedAt = reader.GetDateTime(8)
});
            }
            return Ok(readings);
        }

    }
}