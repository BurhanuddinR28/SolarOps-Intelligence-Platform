using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SolarOps.Api.Models;
namespace SolarOps.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FaultsController : ControllerBase
    {
        private readonly string _connectionString;
        public FaultsController(IConfiguration config)
            {
                _connectionString = config.GetConnectionString("SolarOps")!;
            }
        [HttpGet]
        public IActionResult Get()
        {
            var readings = new List<FaultEvent>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT TOP 100 FaultId, SubId, Timestamp, FaultReason, ToolName, ToolLine FROM FaultEvents", connection);
            using var reader = command.ExecuteReader();

            while(reader.Read())
            {
                readings.Add(new FaultEvent
                {
                    FaultId = reader.GetInt32(0),
                    SubId = reader.GetString(1),
                    Timestamp = reader.GetDateTime(2),
                    FaultReason = reader.GetString(3),
                    ToolName  = reader.GetString(4),
                    ToolLine = reader.GetString(5)

                });
            }
            return Ok(readings);


        }
    }
}