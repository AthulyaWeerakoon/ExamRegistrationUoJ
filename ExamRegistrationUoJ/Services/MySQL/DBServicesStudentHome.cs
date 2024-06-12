// Ramith's workspace 
using ExamRegistrationUoJ.Services.DBInterfaces;
using MySqlConnector;
using System.Data;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceStudentHome
    {
        public async Task<DataTable> getRegisteredExams(int studentId)
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


        public async Task<DataTable> getFilteredExams(int departmentID, int semesterID, int statusID)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select exams filtered by the specified parameters
                string query = @"
                    SELECT 
                        exams.id, 
                        exams.name, 
                        exams.batch, 
                        exams.semester_id, 
                        semesters.name AS semester, 
                        exams.department_id, 
                        departments.name AS department, 
                        exams.registration_status, 
                        exams.registration_close_date 
                    FROM exams
                    INNER JOIN semesters ON exams.semester_id = semesters.id
                    INNER JOIN departments ON exams.department_id = departments.id
                    WHERE 
                        (@DepartmentID IS NULL OR exams.department_id = @DepartmentID)
                        AND (@SemesterID IS NULL OR exams.semester_id = @SemesterID)
                        AND (@StatusID IS NULL OR exams.registration_status = @StatusID)";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Adding parameters to the query
                    cmd.Parameters.AddWithValue("@DepartmentID", departmentID != 0 ? (object)departmentID : DBNull.Value);
                    cmd.Parameters.AddWithValue("@SemesterID", semesterID != 0 ? (object)semesterID : DBNull.Value);
                    cmd.Parameters.AddWithValue("@StatusID", statusID != 0 ? (object)statusID : DBNull.Value);

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

        public async Task<bool> registerForExam(int studentId, int examId)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                using (var transaction = await _connection.BeginTransactionAsync())
                {
                    // Check if the student is already registered for the exam
                    string checkQuery = "SELECT COUNT(*) FROM exam_registrations WHERE student_id = @studentId AND exam_id = @examId";
                    using (var checkCommand = new MySqlCommand(checkQuery, _connection, transaction))
                    {
                        checkCommand.Parameters.AddWithValue("@studentId", studentId);
                        checkCommand.Parameters.AddWithValue("@examId", examId);

                        var count = (long)await checkCommand.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            // Student is already registered
                            return false;
                        }
                    }

                    // Register the student for the exam
                    string insertQuery = "INSERT INTO exam_registrations (student_id, exam_id) VALUES (@studentId, @examId)";
                    using (var insertCommand = new MySqlCommand(insertQuery, _connection, transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@studentId", studentId);
                        insertCommand.Parameters.AddWithValue("@examId", examId);

                        await insertCommand.ExecuteNonQueryAsync();
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }


        public async Task<int?> getStudentIdByEmail(string email)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to get the student ID based on email
                string query = "SELECT id FROM students WHERE email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    var result = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out int studentId))
                    {
                        return studentId;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
 
    }
}