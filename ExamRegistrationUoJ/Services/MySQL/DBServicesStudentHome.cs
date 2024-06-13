// Ramith's workspace
using ExamRegistrationUoJ.Services.DBInterfaces;
using MySqlConnector;
using System.Data;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceStudentHome
    {
        public Task<DataTable> getExams()
        {
            throw new NotImplementedException();
        }

        public async Task<DataTable> getRegisteredExams(string studentId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = @"
                    SELECT 
                        e.id AS id,
                        e.name AS name,
                        e.batch AS batch,
                        e.semester_id AS semester_id,
                        s.name AS semester,
                        d.id AS department_id,
                        d.name AS department,
                        CASE 
                            WHEN e.is_confirmed = 1 THEN 'Confirmed'
                            ELSE 'Not Confirmed'
                        END AS registration_status,
                        e.end_date AS registration_close_date
                    FROM 
                        exams e
                    JOIN 
                        semesters s ON e.semester_id = s.id
                    JOIN 
                        courses_in_exam cie ON e.id = cie.exam_id
                    JOIN 
                        departments d ON cie.department_id = d.id
                    WHERE 
                        (@departmentID IS NULL OR d.id = @departmentID)
                        AND (@semesterID IS NULL OR e.semester_id = @semesterID)
                        AND (@statusID IS NULL OR e.is_confirmed = @statusID);
                ";


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

        Task<DataTable> IDBServiceStudentHome.getFilteredExams(int departmentOpt, int semesterOpt, int statusOpt)
        {
            throw new NotImplementedException();
        }

        Task<DataTable> IDBServiceStudentHome.getRegisteredExams(string studentId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDBServiceStudentHome.registerForExam(int studentId, uint examId)
        {
            throw new NotImplementedException();
        }
    }
}
