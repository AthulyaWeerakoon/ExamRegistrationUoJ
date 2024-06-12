using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;

// Arosha's workspace

namespace ExamRegistrationUoJ.Services.MySQL
{
   public partial class DBMySQL : IDBServiceCoordinator1
    {

        public async Task<DataTable> getExams(int coordinatorId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // Get the current date
                DateTime currentDate = DateTime.Now;

                // SQL query to select active exams related to the specific coordinator
                string query = @"
                    SELECT 
                        e.id AS id,
                        e.name AS name,
                        e.batch AS batch,
                        e.semester_id AS semester_id,
                        s.name AS semester,
                        e.is_confirmed AS status,
                        e.end_date AS end_date
                    FROM 
                        exams e
                    JOIN 
                        semesters s ON e.semester_id = s.id
                    WHERE 
                        e.coordinator_id = @coordinatorId AND 
                        (e.is_confirmed = 0 OR
                        DATE_ADD(e.end_date, INTERVAL COALESCE(e.coordinator_approval_extension, 0) + COALESCE(e.advisor_approval_extension, 0) DAY) > CURDATE());
                ";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameters for the current date and coordinator ID
                    cmd.Parameters.AddWithValue("@currentDate", currentDate);
                    cmd.Parameters.AddWithValue("@coordinatorId", coordinatorId);

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






        /*Task<DataTable> IDBServiceCoordinator1.GetExams()
        {
            throw new NotImplementedException();
        }*/

    }

}
