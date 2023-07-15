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

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // set the license context for your application

        using (ExcelPackage package = new ExcelPackage(new FileInfo(@"C:\Users\rus_d\OneDrive\Desktop\Coding\Dotnet\CSharp\DataVisuals\Output\DowCounts.xlsx")))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ChartData");
            worksheet.Cells["A1"].Value = "Day of Week";
            worksheet.Cells["B1"].Value = "Count";

            for (int i = 0; i < dayCounts.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = dayCounts[i].dow;
                worksheet.Cells[i + 2, 2].Value = dayCounts[i].counts;
            }

            var chart = worksheet.Drawings.AddChart("Sample Chart", eChartType.ColumnClustered);

            var series = chart.Series.Add(worksheet.Cells["B2:B" + (dayCounts.Count + 1)], worksheet.Cells["A2:A" + (dayCounts.Count + 1)]);
            series.Header = "Count";

            chart.Title.Text = "Count by Day of Week";
            chart.SetPosition(1, 0, 3, 0);
            chart.SetSize(800, 600);

            package.Save();
        }
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
