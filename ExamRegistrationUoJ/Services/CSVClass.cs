using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using System.Data;
using System.Text;

namespace ExamRegistrationUoJ.Services
{
    public class CSVClass
    {
        private IHttpContextAccessor contextAccessor;

        public CSVClass() { throw new InvalidOperationException("Class cannot be initalized without a HttpContextAccessor"); }
        public CSVClass(IHttpContextAccessor contextAccessor) { this.contextAccessor = contextAccessor; }

        public KeyValuePair<string, DotNetStreamReference> exportAsCSV(DataTable dataTable, string courseCode, string examId)
        {
            var csvContent = ConvertDataTableToCsv(dataTable);
            var fileName = $"{examId}_{courseCode}_data_{DateTime.Now:yyMMddHHmmss}.csv";

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            var memoryStream = new MemoryStream(bytes);

            DotNetStreamReference streamReference = new DotNetStreamReference(stream: memoryStream);
            
            return new KeyValuePair<string, DotNetStreamReference>(fileName, streamReference);
        }

        public async Task<DataTable?> importAsDataTable(IBrowserFile file)
        {
            DataTable? dataTable =  null;
			if (file != null)
			{
				using var stream = file.OpenReadStream();
				using var reader = new StreamReader(stream);
				var csvContent = await reader.ReadToEndAsync();
				dataTable = ConvertCsvToDataTable(csvContent);
			}
			return dataTable;
        }

        private static string ConvertDataTableToCsv(DataTable dataTable)
        {
            var csv = new StringBuilder();
            foreach (DataColumn column in dataTable.Columns)
            {
                csv.Append(column.ColumnName + ",");
            }
            csv.AppendLine();

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var cell in row.ItemArray)
                {
                    csv.Append(cell.ToString().Replace(",", ";") + ",");
                }
                csv.AppendLine();
            }

            return csv.ToString();
        }

        private static DataTable ConvertCsvToDataTable(string csvContent)
        {
            var dataTable = new DataTable();
            using (var reader = new StringReader(csvContent))
            {
                var headers = reader.ReadLine().Split(',');
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    var row = dataTable.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        row[i] = values[i];
                    }
                    dataTable.Rows.Add(row);
                }
            }
            return dataTable;
        }
    }
}
