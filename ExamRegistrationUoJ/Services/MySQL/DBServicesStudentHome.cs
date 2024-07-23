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

                // SQL query to select the required fields
                string query = @"
                    SELECT DISTINCT
    e.id AS id,
    e.name AS name,
    e.batch AS batch,
    e.semester_id AS semester_id,
    s.name AS semester,
    e.end_date AS deadline,
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
    stu.id = @studentId
    AND e.is_confirmed != 0
    AND e.end_date <= CURDATE();
                ";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add the studentId parameter to the command
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

        public async Task<DataTable> getExams()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = @"
                   SELECT DISTINCT
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
    departments d ON cie.department_id = d.id;
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


        public async Task<DataTable> getFilteredExams(int semesterID)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // Base SQL query
                string query = @"
                SELECT 
                    e.id AS id,
                    e.name AS name,
                    e.batch AS batch,
                    e.semester_id AS semester_id,
                    e.end_date AS deadline,
                    s.name AS semester,
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
                    AND e.is_confirmed = 1
                    AND e.end_date > CURDATE()";


                // List to hold parameters
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                // Apply filters if provided

                if (semesterID != -1)
                {
                    query += " WHERE e.semester_id = @semesterID";
                    parameters.Add(new MySqlParameter("@semesterID", semesterID));
                }


                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameters to the command
                    cmd.Parameters.AddRange(parameters.ToArray());

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

                // SQL query to check if the student is already registered for the exam
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM students_in_exam 
                    WHERE student_id = @studentId AND exam_id = @examId";

                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, _connection))
                {
                    checkCmd.Parameters.AddWithValue("@studentId", studentId);
                    checkCmd.Parameters.AddWithValue("@examId", examId);

                    int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                    if (count > 0)
                    {
                        // Student is already registered for this exam
                        return false;
                    }
                }

                // SQL query to register the student for the exam
                string insertQuery = @"
                    INSERT INTO students_in_exam (student_id, exam_id, is_proper) 
                    VALUES (@studentId, @examId, 1)";  // Assuming 1 means proper registration

                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, _connection))
                {
                    insertCmd.Parameters.AddWithValue("@studentId", studentId);
                    insertCmd.Parameters.AddWithValue("@examId", examId);

                    int rowsAffected = await insertCmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
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

                // SQL query to retrieve the student ID based on email
                string query = @"
                    SELECT stu.id 
                    FROM students stu
                    JOIN accounts acc ON stu.account_id = acc.id
                    WHERE acc.ms_email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add the email parameter to the command
                    cmd.Parameters.AddWithValue("@Email", email);

                    // Execute the query and get the student ID
                    object result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
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


        public async Task<DataTable> getCoursesForExam(int examId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select the required fields
                string query = @"
            SELECT 
                c.code AS course_code,
                c.name AS course_name
            FROM 
                courses c
            JOIN 
                courses_in_exam cie ON c.id = cie.course_id
            WHERE 
                cie.exam_id = @examId";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add the examId parameter to the command
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
    }
}
