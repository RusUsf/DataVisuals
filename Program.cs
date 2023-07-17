using Npgsql;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Host=localhost;Port=5432;Database=monkey_db;Username=burner;Password=burner";

        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        NpgsqlCommand command = new NpgsqlCommand("select * from monkey_count(5,1,2023,100)", connection);

        List<DayCount> dayCounts = new List<DayCount>();

        try
        {
            connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                dayCounts.Add(new DayCount(reader["dow"].ToString(), int.Parse(reader["counts"].ToString())));
            }
        }
        finally
        {
            connection.Close();
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        // Include formatted date in filename
        string formattedDate = DateTime.Now.Date.ToString("MMMM_d_yyyy"); // e.g., 'January_1_2023'
        string filePath = $@"C:\Users\rus_d\OneDrive\Desktop\Coding\Dotnet\CSharp\DataVisuals\Output\DowCounts_{formattedDate}.xlsx";

        using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
        {
            ExcelWorksheet chartWorksheet = package.Workbook.Worksheets.Add("ChartData"); // Create chart worksheet first
            ExcelWorksheet dataWorksheet = package.Workbook.Worksheets.Add("Data"); // Then create data worksheet

            dataWorksheet.Cells["A1"].Value = "Day of Week";
            dataWorksheet.Cells["B1"].Value = "Count";

            for (int i = 0; i < dayCounts.Count; i++)
            {
                dataWorksheet.Cells[i + 2, 1].Value = dayCounts[i].dow;
                dataWorksheet.Cells[i + 2, 2].Value = dayCounts[i].counts;
            }

            var chart = chartWorksheet.Drawings.AddChart("Sample Chart", eChartType.ColumnClustered);
            var series = chart.Series.Add(dataWorksheet.Cells["B2:B" + (dayCounts.Count + 1)], dataWorksheet.Cells["A2:A" + (dayCounts.Count + 1)]);
            series.Header = "Count";

            chart.Title.Text = "Count by Day of Week";
            chart.SetPosition(1, 0, 15, 0);
            chart.SetSize(400, 300);

            dataWorksheet.View.ShowGridLines = false;
            chartWorksheet.View.ShowGridLines = false;

            for (int i = 1; i <= dayCounts.Count + 1; i++)
            {
                dataWorksheet.Cells[i, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                dataWorksheet.Cells[i, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                dataWorksheet.Cells[i, 2].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                dataWorksheet.Cells[i, 2].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
            }

            package.Save();
        }

        // Open the Excel file
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
    }
}

class DayCount
{
    public string dow { get; set; }
    public int counts { get; set; }

    public DayCount(string dayOfWeek, int count)
    {
        this.dow = dayOfWeek;
        this.counts = count;
    }
}
