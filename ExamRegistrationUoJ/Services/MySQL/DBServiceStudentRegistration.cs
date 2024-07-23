// Ramith's workspace

using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Identity.Client;
using MySqlConnector;
using System.Data;
using System.Data.Common;
using static ExamRegistrationUoJ.Components.Pages.Administrator.AdminDashboard;
using static ExamRegistrationUoJ.Components.Pages.Home;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceSR
    {
        public async Task<DataTable> getStudent(uint studentId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

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

        public async Task<DataTable> getExamTitle(uint examId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = "SELECT exams.id AS id, exams.name AS exam_name, exams.batch, semesters.name AS semester_name " +
                               "FROM  exams " +
                               "JOIN  semesters ON exams.semester_id = semesters.id " +
                               "WHERE  exams.id = @examId;";

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

        public async Task<int> setStudentInExams(uint studentId, uint examId, uint isProper, uint advisorId)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to check if an entry already exists and get the primary key if it does
                string checkQuery = "SELECT id FROM students_in_exam WHERE student_id = @studentId AND exam_id = @examId;";

                // MySqlCommand to execute the check query
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, _connection))
                {
                    checkCmd.Parameters.AddWithValue("@studentId", studentId);
                    checkCmd.Parameters.AddWithValue("@examId", examId);

                    object result = await checkCmd.ExecuteScalarAsync();

                    if (result != null)
                    {
                        if (isProper == 1) 
                        {
                            // If an entry already exists, return the primary key
                            int existingStudentInExamId = Convert.ToInt32(result);

                            string updateQuery = "UPDATE students_in_exam SET is_proper = @isProper " +
                                                 "WHERE id = @existingStudentInExamId;";

                            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, _connection))
                            {
                                updateCmd.Parameters.AddWithValue("@isProper", isProper);
                                updateCmd.Parameters.AddWithValue("@existingStudentInExamId", existingStudentInExamId);

                                await updateCmd.ExecuteNonQueryAsync();
                            }
                            return existingStudentInExamId;
                        }
                        else 
                        {
                            // If an entry already exists, return the primary key
                            int existingStudentInExamId = Convert.ToInt32(result);

                            string updateQuery = "UPDATE students_in_exam SET is_proper = @isProper, advisor_id = @advisorId " +
                                                 "WHERE id = @existingStudentInExamId;";

                            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, _connection))
                            {
                                updateCmd.Parameters.AddWithValue("@isProper", isProper);
                                updateCmd.Parameters.AddWithValue("@advisorId", advisorId);
                                updateCmd.Parameters.AddWithValue("@existingStudentInExamId", existingStudentInExamId);

                                await updateCmd.ExecuteNonQueryAsync();
                            }
                            return existingStudentInExamId;
                        }                        
                    }
                }

                if (isProper == 1) 
                {
                    // SQL query to insert a new coordinator
                    string insertQuery = "INSERT INTO students_in_exam (student_id, exam_id, is_proper) " +
                                         "VALUES (@studentId, @examId, @isProper); " +
                                         "SELECT LAST_INSERT_ID();";

                    // MySqlCommand to execute the SQL insert query
                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, _connection))
                    {
                        insertCmd.Parameters.AddWithValue("@studentId", studentId);
                        insertCmd.Parameters.AddWithValue("@examId", examId);
                        insertCmd.Parameters.AddWithValue("@isProper", isProper);

                        int newStudentInExamId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
                        return newStudentInExamId;
                    }
                }
                else
                {
                    // SQL query to insert a new coordinator
                    string insertQuery = "INSERT INTO students_in_exam (student_id, exam_id, is_proper, advisor_id, advisor_approved) " +
                                         "VALUES (@studentId, @examId, @isProper, @advisorId, 0); " +
                                         "SELECT LAST_INSERT_ID();";

                    // MySqlCommand to execute the SQL insert query
                    using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, _connection))
                    {
                        insertCmd.Parameters.AddWithValue("@studentId", studentId);
                        insertCmd.Parameters.AddWithValue("@examId", examId);
                        insertCmd.Parameters.AddWithValue("@isProper", isProper);
                        insertCmd.Parameters.AddWithValue("@advisorId", advisorId);

                        int newStudentInExamId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
                        return newStudentInExamId;
                    }
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<uint> getAdvisorId(string msEmail)
        {
            DataTable dataTable = new DataTable();
            uint advisorId = 0;

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select the advisor id and name from the accounts table
                string query = "SELECT accounts.id as id, accounts.name AS name, advisors.id as advisor_id " +
                               "FROM advisors " +
                               "JOIN accounts ON advisors.account_id = accounts.id " +
                               "WHERE accounts.ms_email = @msEmail;";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Define the parameter and assign its value
                    cmd.Parameters.AddWithValue("@msEmail", msEmail);

                    // Execute the query and load the results into a DataTable
                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }

                // Check if the DataTable has any rows and get the advisor id
                if (dataTable.Rows.Count > 0)
                {
                    advisorId = Convert.ToUInt32(dataTable.Rows[0]["advisor_id"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return advisorId;
        }

        public async Task<int> setPayments(uint studentId, uint examId, string paymentReceipt)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to check if an entry already exists and get the primary key if it does
                string checkQuery = "SELECT id FROM payments WHERE student_id = @studentId AND exam_id = @examId;";

                // MySqlCommand to execute the check query
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, _connection))
                {
                    checkCmd.Parameters.AddWithValue("@studentId", studentId);
                    checkCmd.Parameters.AddWithValue("@examId", examId);

                    object result = await checkCmd.ExecuteScalarAsync();

                    if (result != null)
                    {
                        string updateQuery = "UPDATE payments SET receipt = @paymentReceipt " +
                                             "WHERE student_id = @studentId AND exam_id = @examId;";

                        using (MySqlCommand cmd = new MySqlCommand(updateQuery, _connection))
                        {
                            // Set parameters
                            cmd.Parameters.AddWithValue("@paymentReceipt", paymentReceipt);
                            cmd.Parameters.AddWithValue("@studentId", studentId);
                            cmd.Parameters.AddWithValue("@examId", examId);

                            // Execute the query
                            await cmd.ExecuteNonQueryAsync();
                            return -1;
                        }                        
                    }
                }
                string query = "INSERT INTO payments (student_id, exam_id, is_verified, receipt) " +
                               "VALUES (@studentId, @examId, 0, @paymentReceipt);";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@studentId", studentId);
                    cmd.Parameters.AddWithValue("@examId", examId);
                    cmd.Parameters.AddWithValue("@paymentReceipt", paymentReceipt);

                    await cmd.ExecuteNonQueryAsync();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<DataTable?> getCoursesInStudentRegistration(int? studentInExamId, uint departmentId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to retrieve exam details along with course information for the given students_in_exam ID

                string query = "";

                if (departmentId == 0)
                {
                    query = @"
                    SELECT 
                        cie.id AS course_in_exam_id,
                        c.code AS course_code,
                        c.name AS course_name,
                        a.ms_email AS coordinator_email,
                        cie.department_id AS department_id
                    FROM 
                        student_registration sreg
                    JOIN 
                        courses_in_exam cie ON sreg.exam_course_id = cie.id
                    JOIN 
                        courses c ON cie.course_id = c.id
                    JOIN 
                        exams e ON cie.exam_id = e.id
                    JOIN 
                        coordinators co ON cie.coordinator_id = co.id
                    JOIN 
                        accounts a ON co.account_id = a.id
                    WHERE 
                        sreg.exam_student_id = @studentInExamId;";
                }
                else
                {
                    query = @"
                    SELECT 
                        cie.id AS course_in_exam_id,
                        c.code AS course_code,
                        c.name AS course_name,
                        a.ms_email AS coordinator_email,
                        cie.department_id AS department_id
                    FROM 
                        student_registration sreg
                    JOIN 
                        courses_in_exam cie ON sreg.exam_course_id = cie.id
                    JOIN 
                        courses c ON cie.course_id = c.id
                    JOIN 
                        exams e ON cie.exam_id = e.id
                    JOIN 
                        coordinators co ON cie.coordinator_id = co.id
                    JOIN 
                        accounts a ON co.account_id = a.id
                    WHERE 
                        sreg.exam_student_id = @studentInExamId AND cie.department_id = @departmentId;";
                }

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the student_in_exam ID
                    cmd.Parameters.AddWithValue("@studentInExamId", studentInExamId);
                    cmd.Parameters.AddWithValue("@departmentId", departmentId);

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

        public async Task<DataTable?> getCoursesNotInStudentRegistration(int examId, int? studentInExamId, uint departmentId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to retrieve details of exams in the exams table but not in the student_registration table
                string query = "";

                if (departmentId == 0)
                {
                    query = @"
                    SELECT 
                        cie.id AS course_in_exam_id,
                        c.code AS course_code,
                        c.name AS course_name,
                        a.ms_email AS coordinator_email,
                        cie.department_id
                    FROM 
                        courses_in_exam cie
                    JOIN 
                        courses c ON cie.course_id = c.id
                    JOIN 
                        coordinators co ON cie.coordinator_id = co.id
                    JOIN 
                        accounts a ON co.account_id = a.id
                    WHERE 
                        cie.exam_id = @examId
                        AND cie.id NOT IN (
                            SELECT 
                                exam_course_id
                            FROM 
                                student_registration
                            WHERE 
                                exam_student_id = @studentInExamId);";
                }
                else
                {
                    query = @"
                    SELECT 
                        cie.id AS course_in_exam_id,
                        c.code AS course_code,
                        c.name AS course_name,
                        a.ms_email AS coordinator_email,
                        cie.department_id
                    FROM 
                        courses_in_exam cie
                    JOIN 
                        courses c ON cie.course_id = c.id
                    JOIN 
                        coordinators co ON cie.coordinator_id = co.id
                    JOIN 
                        accounts a ON co.account_id = a.id
                    WHERE 
                        cie.exam_id = @examId
                        AND cie.id NOT IN (
                            SELECT 
                                exam_course_id
                            FROM 
                                student_registration
                            WHERE 
                                exam_student_id = @studentInExamId)
                        AND cie.department_id = @departmentId;";
                }

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameters for the exam ID and students_in_exam ID
                    cmd.Parameters.AddWithValue("@examId", examId);
                    cmd.Parameters.AddWithValue("@studentInExamId", studentInExamId);
                    cmd.Parameters.AddWithValue("@departmentId", departmentId);

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

            // Check if the DataTable is empty and return null in that case
            if (dataTable.Rows.Count == 0)
                return null;

            return dataTable;
        }

        public async Task<DataTable> getDepartmentsInExam(uint examId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = "SELECT DISTINCT d.name, d.id " +
                               "FROM courses_in_exam cie " +
                               "JOIN departments d ON cie.department_id = d.id " +
                               "WHERE cie.exam_id = @examId; ";

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

        public async Task setStudentRegistration(DataTable courseStates, int? studentInExamId)
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("Database connection is not initialized.");
            }

            try
            {
                // Open the connection if it's not already open
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }

                using (var transaction = await _connection.BeginTransactionAsync())
                {
                    if (courseStates != null && courseStates.Rows.Count > 0)
                    {
                        // First, delete existing records for the student in this exam
                        string deleteQuery = "DELETE FROM student_registration WHERE exam_student_id = @studentInExamId;";
                        using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, _connection, transaction))
                        {
                            deleteCmd.Parameters.AddWithValue("@studentInExamId", studentInExamId);
                            await deleteCmd.ExecuteNonQueryAsync();
                        }

                        // Now, insert the new records
                        string insertQuery = "INSERT INTO student_registration (exam_student_id, exam_course_id, is_approved) VALUES (@studentInExamId, @examCourseId, 0);";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, _connection, transaction))
                        {
                            insertCmd.Parameters.Add("@studentInExamId", MySqlDbType.Int32).Value = studentInExamId;
                            insertCmd.Parameters.Add("@examCourseId", MySqlDbType.Int32);

                            foreach (DataRow row in courseStates.Rows)
                            {
                                if ((bool)row["is_added"])
                                {
                                    insertCmd.Parameters["@examCourseId"].Value = row["course_in_exam_id"];
                                    await insertCmd.ExecuteNonQueryAsync();
                                }
                            }
                        }
                    }
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                // Ensure the connection is closed
                if (_connection.State == ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
        }

    }

}
