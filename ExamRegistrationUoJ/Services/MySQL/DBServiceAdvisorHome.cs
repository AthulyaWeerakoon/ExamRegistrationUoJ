using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections.Specialized;
using System.Text;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceAdvisorHome
    {
        public async Task<DataTable> getExams(int departmentId, int semesterId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select the required fields and apply filters
                string query = @"
                    SELECT 
                        e.id AS id,
                        e.name AS description,
                        e.semester_id AS semester_id,
                        s.name AS semester,
                        d.id AS department_id,
                        d.name AS department,
                        e.coordinator_approval_extension AS approval_opens,
                        e.end_date AS closed
                    FROM 
                        exams e
                    JOIN 
                        semesters s ON e.semester_id = s.id
                    JOIN 
                        courses_in_exam cie ON e.id = cie.exam_id
                    JOIN 
                        departments d ON cie.department_id = d.id
                    WHERE 
                        d.id = @departmentId AND e.semester_id = @semesterId
                    GROUP BY 
                        e.id, e.name, e.semester_id, s.name, d.id, d.name, e.coordinator_approval_extension, e.end_date;
                ";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@departmentId", departmentId);
                    cmd.Parameters.AddWithValue("@semesterId", semesterId);

                    // Execute the query and load the results into a DataTable
                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return dataTable;
        }

    }
}
