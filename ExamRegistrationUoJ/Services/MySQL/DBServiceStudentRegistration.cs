// Ramith's workspace

using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Identity.Client;
using MySqlConnector;
using System.Data;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceSR
    {
        public Task<DataTable> getAdvisors()
        {
            throw new NotImplementedException();
        }

        public async Task<DataTable> getCourses(ulong examId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select courses based on the examId

                string query = "SELECT  cie.id as id, c.name AS course_name, c.code AS course_code, a.ms_email AS coordinator_email, cie.department_id AS dep_id " +
                               "FROM courses_in_exam cie " +
                               "JOIN courses c ON cie.course_id = c.id " +
                               "JOIN coordinators co ON cie.coordinator_id = co.id " +
                               "JOIN accounts a ON co.account_id = a.id " +
                               "WHERE cie.exam_id = @examID;";

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


        public async Task<DataTable> getStudent(ulong studentId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = "SELECT a.id, a.name, a.ms_email FROM accounts a WHERE a.id = (SELECT account_id FROM students WHERE id = @studentId); ";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Define the parameter and assign its value
                    cmd.Parameters.AddWithValue("@studentId", studentId);

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
