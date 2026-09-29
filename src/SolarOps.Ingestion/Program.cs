using CsvHelper;
using System.Globalization;
using Microsoft.Data.SqlClient;

//defining the connection string
var connectionString = "Server=localhost,1433;Database=SolarOps;User Id=sa;Password=SolarOps123;TrustServerCertificate=True;";
using var connection = new SqlConnection(connectionString);
connection.Open();
Console.WriteLine("Connected to SQL server successfully");
var csvPath = "../../data/raw/panel_readings.csv";
using var reader = new StreamReader(csvPath);
using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
var records = csv.GetRecords<SolarOps.Ingestion.Models.PanelReading>().ToList();
Console.WriteLine($"Read {records.Count} records from csv");
int inserted=0;
int skipped=0;
foreach(var record in records)
{
    if(string.IsNullOrEmpty(record.SubId) || record.Timestamp == default)
    {
        skipped++;
        continue;
    }
    using var command = new SqlCommand(@"
        INSERT INTO PanelReadings (subID, Timestamp, ReadTime, Wattage, PlantId, ToolId, PassFail, CreatedAt)
        VALUES (@SubId, @Timestamp, @ReadTime, @Wattage, @PlantId, @ToolId, @PassFail, @CreatedAt)", connection);

        command.Parameters.AddWithValue("@SubId", record.SubId);
        command.Parameters.AddWithValue("@Timestamp", record.Timestamp);
        command.Parameters.AddWithValue("@ReadTime", (object?)record.ReadTime?? DBNull.Value);
        command.Parameters.AddWithValue("@Wattage", (object?)record.Wattage??DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", record.CreatedAt);
        command.Parameters.AddWithValue("@PassFail", (object?)record.PassFail??DBNull.Value);
        command.Parameters.AddWithValue("@PlantId", 1);
        command.Parameters.AddWithValue("@ToolId", 1);

        command.ExecuteNonQuery();
        inserted++;
}
Console.WriteLine($"Inserted: {inserted} | Skipped: {skipped}");

