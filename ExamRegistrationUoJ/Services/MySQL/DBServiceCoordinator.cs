using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;

// Arosha's workspace

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceCoordinator1
    {

        public async Task<int> getCoordinatorID(string email)
        {
            int coordinatorID = -1;

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = "SELECT id FROM accounts WHERE ms_email = @Email";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    object result = await cmd.ExecuteScalarAsync();

                    if (result != null && int.TryParse(result.ToString(), out coordinatorID))
                    {
                        // ID extracted successfully
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return coordinatorID;
        }


        public async Task<DataTable> getexam_id_details_coordinator(string email)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                    SELECT ce.course_id, ce.exam_id , ce.department_id, et.end_date, et.coordinator_approval_extension, et.name, et.semester_id 
                    FROM courses_in_exam ce 
                    JOIN coordinators c ON c.id = ce.coordinator_id 
                    JOIN accounts a ON a.id = c.account_id 
                    JOIN exams et ON et.id = ce.exam_id 
                    WHERE a.ms_email = @Email and et.is_confirmed!=0
                    GROUP BY  ce.exam_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

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

        public async Task<DataTable> get_exam_all_details_coordinator(string email)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                            SELECT ce.exam_id, 
                            et.semester_id, 
                            ce.department_id, 
                            ce.course_id, 
                            co.name AS course_name, 
                            co.code, 
                            et.end_date, 
                            et.coordinator_approval_extension, 
                            et.name AS exam_name, 
                            d.name AS department_name
                            FROM courses_in_exam ce
                            JOIN coordinators c ON c.id = ce.coordinator_id
                            JOIN accounts a ON a.id = c.account_id
                            JOIN exams et ON et.id = ce.exam_id
                            JOIN courses co ON co.id = ce.course_id
                            JOIN departments d ON d.id = ce.department_id
                            WHERE a.ms_email = @Email and et.is_confirmed!=0";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

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

        public async Task<DataTable> getExamDetails_student(int exam_id, string CourseCode)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                             SELECT 
                             sr.exam_course_id AS id,
                             s.account_id AS student_id, 
                             a.account_id AS advisor_id, 
                             sa.ms_email AS student_email,
                             sa.name AS student_name, 
                             aa.name AS advisor_name ,sr.is_approved,sr.attendance,c.code,sr.exam_student_id,sr.exam_course_id
                             FROM students_in_exam se 
                             JOIN students s ON se.student_id = s.id 
                             JOIN advisors a ON se.advisor_id = a.id 
                             JOIN accounts sa ON s.account_id = sa.id 
                             JOIN accounts aa ON a.account_id = aa.id 
                             join student_registration sr on sr.exam_student_id=se.id
                             join courses_in_exam ce on ce.id=sr.exam_course_id
                             join courses c on c.id=ce.course_id
                             WHERE se.exam_id  = @Exam_id and c.code=@CourseCode";


                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {

                    cmd.Parameters.AddWithValue("@Exam_id", exam_id);
                    cmd.Parameters.AddWithValue("@CourseCode", CourseCode);
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

        public async Task<string> get_coursecode(string courseCode)
        {
            string courseName = "";
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                    SELECT co.name FROM courses co
                    WHERE co.code = @CourseCode";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@CourseCode", courseCode);

                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            courseName = reader.GetString("name");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return courseName;
        }
        public async Task<DataTable> get_enddate(int examIdNumber)
        {
            DataTable dataTable = new DataTable();
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
            SELECT e.end_date, e.coordinator_approval_extension 
            FROM exams e
            WHERE e.id = @ExamId and e.is_confirmed!=0";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@ExamId", examIdNumber);

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
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return dataTable;
        }

        public async Task<DataTable> is_confrom_exam_count(string email)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                        WITH DistinctExamCourse AS (
                        SELECT DISTINCT
                        se.exam_id,
                        c.code
                        FROM student_registration sr
                        JOIN students_in_exam se ON se.id = sr.exam_student_id
                        JOIN courses_in_exam cie ON cie.id = sr.exam_course_id
                        JOIN courses c ON c.id = cie.course_id
                        JOIN coordinators co ON co.id = cie.coordinator_id
                        JOIN accounts a ON a.id = co.account_id
                        WHERE a.ms_email = @Email
                         )
                        SELECT
                        de.exam_id,
                        de.code,
                        SUM(CASE WHEN sr.is_approved IS NULL THEN 1 ELSE 0 END) AS is_approved_null_count,
                        SUM(CASE WHEN sr.attendance IS NULL THEN 1 ELSE 0 END) AS attendance_null_count,
                        SUM(CASE WHEN sr.is_approved = 0 THEN 1 ELSE 0 END) AS is_approved_zero_count
                        FROM DistinctExamCourse de
                        JOIN courses c ON c.code = de.code
                        JOIN courses_in_exam cie ON cie.course_id = c.id
                        JOIN student_registration sr ON sr.exam_course_id = cie.id
                        JOIN students_in_exam se ON se.id = sr.exam_student_id AND se.exam_id = de.exam_id
                        GROUP BY
                        de.exam_id,
                        de.code";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return dataTable;
        }

        public async Task<DataTable> student_registration_table(string courseCode)
        {
            DataTable dataTable = new DataTable();
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
            SELECT
                sr.exam_student_id,
                sr.exam_course_id,
                sr.is_approved,
                sr.attendance,
                a.ms_email
            FROM 
                students st
                JOIN accounts a ON a.id = st.account_id
                JOIN students_in_exam se ON se.student_id = st.id
                JOIN student_registration sr ON se.id = sr.exam_student_id
                JOIN courses_in_exam ce ON ce.id = sr.exam_course_id
                JOIN courses c ON c.id = ce.course_id
            WHERE 
                c.code = @CourseCode";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@CourseCode", courseCode);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return dataTable;
        }

        public async Task save_change_coordinator_aproval(int exam_course_id, DataTable approval_table)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            using (var transaction = await _connection.BeginTransactionAsync())
            {
                try
                {
                    foreach (DataRow row in approval_table.Rows)
                    {
                        var examStudentId = row["exam_student_id"];
                        var isApproved = row["is_approved"];
                        var attendance = row["attendance"];

                        using (var command = new MySqlCommand())
                        {
                            command.Connection = _connection;
                            command.Transaction = transaction;
                            command.CommandText = @"
                        UPDATE student_registration 
                        SET is_approved = @isApproved, attendance = @attendance
                        WHERE exam_student_id = @examStudentId AND exam_course_id = @examCourseId";

                            command.Parameters.AddWithValue("@isApproved", isApproved);
                            command.Parameters.AddWithValue("@attendance", attendance);
                            command.Parameters.AddWithValue("@examStudentId", examStudentId);
                            command.Parameters.AddWithValue("@examCourseId", exam_course_id);

                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Transaction failed", ex);
                }
            }
        }
        public async Task<DataTable> getexam_id_details_coordinator_filter(string email, int semester_id)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                    SELECT ce.course_id, ce.exam_id , ce.department_id, et.end_date, et.coordinator_approval_extension, et.name, et.semester_id 
                    FROM courses_in_exam ce 
                    JOIN coordinators c ON c.id = ce.coordinator_id 
                    JOIN accounts a ON a.id = c.account_id 
                    JOIN exams et ON et.id = ce.exam_id 
                    WHERE a.ms_email = @Email and et.semester_id=@SemesterId
                    GROUP BY  ce.exam_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@SemesterId", semester_id);

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

        public async Task<DataTable> get_exam_all_details_coordinator_filter(string email, int semester_id)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                            SELECT ce.exam_id, 
                            et.semester_id, 
                            ce.department_id, 
                            ce.course_id, 
                            co.name AS course_name, 
                            co.code, 
                            et.end_date, 
                            et.coordinator_approval_extension, 
                            et.name AS exam_name, 
                            d.name AS department_name
                            FROM courses_in_exam ce
                            JOIN coordinators c ON c.id = ce.coordinator_id
                            JOIN accounts a ON a.id = c.account_id
                            JOIN exams et ON et.id = ce.exam_id
                            JOIN courses co ON co.id = ce.course_id
                            JOIN departments d ON d.id = ce.department_id
                            WHERE a.ms_email = @Email and and et.is_confirmed!=0";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@SemesterId", semester_id);

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


