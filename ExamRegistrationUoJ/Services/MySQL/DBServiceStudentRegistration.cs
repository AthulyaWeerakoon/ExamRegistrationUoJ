// Ramith's workspace

using ExamRegistrationUoJ.Services.DBInterfaces;
using MySqlConnector;
using System.Data;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceStudentRegistration
    {

        public async Task<DataTable> getCourses()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = "SELECT c.id, " +
                       "cie.department_id AS dep_id, " +
                       "c.name, " +
                       "a.name AS coordinator, " +
                       "c.code " +
                       "FROM courses_in_exam cie " +
                       "JOIN courses c ON cie.course_id = c.id " +
                       "JOIN departments d ON cie.department_id = d.id " +
                       "JOIN coordinators co ON cie.coordinator_id = co.id " +
                       "JOIN accounts a ON co.account_id = a.id " +
                       "WHERE cie.exam_id = @examId";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
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

        Task<DataTable> IDBServiceStudentRegistration.getStudents()
        {
            throw new NotImplementedException();
        }
    }




    
}
