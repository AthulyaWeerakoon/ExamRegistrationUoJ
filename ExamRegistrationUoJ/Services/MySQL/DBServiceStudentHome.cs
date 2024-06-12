// Ramith's workspace
using ExamRegistrationUoJ.Services.DBInterfaces;
using MySqlConnector;
using System.Data;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceStudentHome
    {
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
                    JOIN 
                        students_in_exam sie ON e.id = sie.exam_id
                    JOIN 
                        students stu ON sie.student_id = stu.id
                    WHERE 
                        stu.id = @studentId;
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
        public async Task<DataTable> getExams()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select department id and name from the departments table
                string query = @"
                        SELECT 
                            e.id AS id, 
                            e.name AS name, 
                            e.batch AS batch, 
                            e.semester_id AS semester_id, 
                            s.name AS semester, 
                            cd.department_id AS department_id, 
                            d.name AS department, 
                            CASE 
                                WHEN NOW() <= e.end_date THEN 'Open' 
                                ELSE 'Closed' 
                            END AS registration_status, 
                            e.end_date AS registration_close_date
                        FROM exams e
                        JOIN semesters s ON e.semester_id = s.id
                        JOIN courses_in_exam cie ON e.id = cie.exam_id
                        JOIN departments d ON cie.department_id = d.id
                        JOIN course_departments cd ON cie.course_id = cd.course_id AND cd.department_id = cie.department_id
                        GROUP BY e.id, cd.department_id;";

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

        Task<DataTable> IDBServiceStudentHome.getRegisteredExams(int studentId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IDBServiceStudentHome.registerForExam(int studentId, int examId)
        {
            throw new NotImplementedException();
        }

        Task<int?> IDBServiceStudentHome.getStudentIdByEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
