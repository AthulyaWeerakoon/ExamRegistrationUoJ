using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Identity.Client;
using MySqlConnector;
using System.Data;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceAdvisorViewExam
    {
        public async Task setAdvisorApproval(uint sieId, uint examId, bool isApproved)
        {
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = "UPDATE students_in_exam " +
                               "SET advisor_approved = @isApproved " +
                               "WHERE id = @sieId;";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@sieId", sieId);
                    cmd.Parameters.AddWithValue("@examId", examId);
                    cmd.Parameters.AddWithValue("@isApproved", isApproved ? 1 : 0);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<DataTable> getStudentsInExam(uint examId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = "SELECT a.id as acc_id, a.name, a.ms_email, sie.advisor_approved, sie.id " +
                               "FROM students_in_exam sie " +
                               "JOIN students s ON sie.student_id = s.id " +
                               "JOIN accounts a ON s.account_id = a.id " +
                               "WHERE sie.exam_id = @examId;";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Define the parameter and assign its value
                    cmd.Parameters.AddWithValue("@examId", examId);

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

        /*public async Task<DataTable> test_a(uint examId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = "SELECT * " +
                               "FROM students_in_exam " +
                               "WHERE exam_id=@examId;";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@examId", examId);
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
        }*/
    }
}
