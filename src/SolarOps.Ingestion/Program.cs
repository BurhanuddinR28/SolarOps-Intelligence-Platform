using System.Data;
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
using var countCommand = new SqlCommand("SELECT COUNT(*) FROM PanelReadings", connection);
int existingPanels = (int)countCommand.ExecuteScalar()!;
int inserted = 0;
int skipped = 0;
if (existingPanels == 0)
{
    foreach (var record in records)
    {
        if (string.IsNullOrEmpty(record.SubId) || record.Timestamp == default)
        {
            skipped++;
            continue;
        }
        using var command = new SqlCommand(@"
        INSERT INTO PanelReadings (subID, Timestamp, ReadTime, Wattage, PlantId, ToolId, PassFail, CreatedAt)
        VALUES (@SubId, @Timestamp, @ReadTime, @Wattage, @PlantId, @ToolId, @PassFail, @CreatedAt)", connection);

        command.Parameters.AddWithValue("@SubId", record.SubId);
        command.Parameters.AddWithValue("@Timestamp", record.Timestamp);
        command.Parameters.AddWithValue("@ReadTime", (object?)record.ReadTime ?? DBNull.Value);
        command.Parameters.AddWithValue("@Wattage", (object?)record.Wattage ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", record.CreatedAt);
        command.Parameters.AddWithValue("@PassFail", (object?)record.PassFail ?? DBNull.Value);
        command.Parameters.AddWithValue("@PlantId", 1);
        command.Parameters.AddWithValue("@ToolId", 1);

        command.ExecuteNonQuery();
        inserted++;
    }
}
else
{
    Console.WriteLine("PanelReadings already loaded, skipping panel insert");
}
var csvIdToSubId = records.ToDictionary(r => r.ReadingId, r => r.SubId);
Console.WriteLine($"CSV lookup: {csvIdToSubId.Count} entries");

var subIdToSqlId = new Dictionary<string, int>();
using (var lookupCommand = new SqlCommand("SELECT ReadingId, SubId FROM PanelReadings", connection))
using (var lookupReader = lookupCommand.ExecuteReader())
{
    while (lookupReader.Read())
    {
        subIdToSqlId[lookupReader.GetString(1)] = lookupReader.GetInt32(0);
    }
}
Console.WriteLine($"SQL lookup: {subIdToSqlId.Count} entries");
Console.WriteLine($"CSV ID 1 → {csvIdToSubId[1]} → SQL ID {subIdToSqlId[csvIdToSubId[1]]}");
var ivPath = "../../data/raw/iv_curves.csv";
using var ivReader = new StreamReader(ivPath);
using var ivCsv = new CsvReader(ivReader, CultureInfo.InvariantCulture);
var ivRecords = ivCsv.GetRecords<SolarOps.Ingestion.Models.IVCurve>().ToList();
Console.WriteLine($"Read {ivRecords.Count} IV rows from csv");
var ivTable = new DataTable();
ivTable.Columns.Add("ReadingId", typeof(int));
ivTable.Columns.Add("Voltage", typeof(double));
ivTable.Columns.Add("CurrentAmp", typeof(double));

var failures = new List<string>();

foreach (var iv in ivRecords)
{
    if (csvIdToSubId.TryGetValue(iv.ReadingId, out var subId) && subIdToSqlId.TryGetValue(subId, out var sqlId))
    {
        ivTable.Rows.Add(sqlId, (object?)iv.Voltage ?? DBNull.Value, (object?)iv.CurrentAmp ?? DBNull.Value);
    }
    else
    {
        failures.Add($"IVId {iv.IVId}: CSV ReadingId {iv.ReadingId} could not be translated");
    }
}

Console.WriteLine($"Translated {ivTable.Rows.Count} rows, {failures.Count} failures");

if (failures.Count > 0)
{
    Console.WriteLine("Aborting IV load. First failures:");
    foreach (var f in failures.Take(5)) Console.WriteLine(f);
    return;
}
Console.WriteLine($"Inserted: {inserted} | Skipped: {skipped}");


