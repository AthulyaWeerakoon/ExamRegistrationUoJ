// Ramith's workspace

using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Identity.Client;
using MySqlConnector;
using System.Data;
using System.Data.Common;
using static ExamRegistrationUoJ.Components.Pages.Administrator.AdminDashboard;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceSR
    {

        public async Task<DataTable> getCourses(uint examId, uint depId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = "";

                if (depId == 0)
                {
                    query = "SELECT cie.id AS id, c.name AS course_name, c.code AS course_code, a.ms_email AS coordinator_email, cie.course_id AS course_id " +
                               "FROM courses_in_exam cie " +
                               "JOIN courses c ON cie.course_id = c.id " +
                               "JOIN coordinators co ON cie.coordinator_id = co.id " +
                               "JOIN accounts a ON co.account_id = a.id " +
                               "WHERE cie.exam_id = @examId;";
                }
                else
                {
                    query = "SELECT cie.id AS id, c.name AS course_name, c.code AS course_code, a.ms_email AS coordinator_email, cie.course_id AS course_id " +
                               "FROM courses_in_exam cie " +
                               "JOIN courses c ON cie.course_id = c.id " +
                               "JOIN coordinators co ON cie.coordinator_id = co.id " +
                               "JOIN accounts a ON co.account_id = a.id " +
                               "WHERE cie.exam_id = @examId AND cie.department_id = @depId;";
                }


                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@examId", examId);
                    cmd.Parameters.AddWithValue("@depId", depId);

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


        public async Task<DataTable> getStudent(uint studentId)
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

        public async Task<DataTable> getExamTitle(uint examId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
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

        public async Task<int> setStudentRegistration(uint examStudentId, uint examCourseId, string addOrDrop)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to check if an entry already exists and get the primary key if it does
                string checkQuery = "SELECT 1 FROM student_registration WHERE exam_student_id = @examStudentId AND exam_course_id = @examCourseId;";

                // MySqlCommand to execute the check query
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, _connection))
                {
                    checkCmd.Parameters.AddWithValue("@examStudentId", examStudentId);
                    checkCmd.Parameters.AddWithValue("@examCourseId", examCourseId);

                    object result = await checkCmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        if (addOrDrop == "Add")
                        {
                            // SQL query to insert a new coordinator
                            string query = "UPDATE student_registration " +
                                           "SET is_approved = 0, attendance = 0 " +
                                           "WHERE exam_student_id = @examStudentId AND exam_course_id = @examCourseId;";

                            // MySqlCommand to execute the SQL query
                            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                            {
                                cmd.Parameters.AddWithValue("@examStudentId", examStudentId);
                                cmd.Parameters.AddWithValue("@examCourseId", examCourseId);

                                // Execute the query and return the number of affected rows
                                await cmd.ExecuteNonQueryAsync();
                                return 1;
                            }
                        }
                        else
                        {
                            string query = "DELETE FROM student_registration WHERE exam_student_id = @examStudentId AND exam_course_id = @examCourseId;";

                            // MySqlCommand to execute the SQL query
                            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                            {
                                cmd.Parameters.AddWithValue("@examStudentId", examStudentId);
                                cmd.Parameters.AddWithValue("@examCourseId", examCourseId);

                                // Execute the query and return the number of affected rows
                                await cmd.ExecuteNonQueryAsync();
                                return 0;
                            }
                        }
                    }
                    else
                    {
                        if (addOrDrop == "Add")
                        {
                            // SQL query to insert a new coordinator
                            string query = "INSERT INTO student_registration (exam_student_id, exam_course_id, is_approved, attendance) " +
                                           "VALUES (@examStudentId, @examCourseId, 0, 0);";

                            // MySqlCommand to execute the SQL query
                            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                            {
                                cmd.Parameters.AddWithValue("@examStudentId", examStudentId);
                                cmd.Parameters.AddWithValue("@examCourseId", examCourseId);

                                // Execute the query and return the number of affected rows
                                await cmd.ExecuteNonQueryAsync();
                                return 1;
                            }
                        }
                        else
                        {
                            string query = "DELETE FROM student_registration WHERE exam_student_id = @examStudentId AND exam_course_id = @examCourseId;";

                            // MySqlCommand to execute the SQL query
                            using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                            {
                                cmd.Parameters.AddWithValue("@examStudentId", examStudentId);
                                cmd.Parameters.AddWithValue("@examCourseId", examCourseId);

                                // Execute the query and return the number of affected rows
                                await cmd.ExecuteNonQueryAsync();
                                return 0;
                            }
                        }
                    }

                }
                    

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        public async Task<int> setPayments(uint studentId, uint examId, byte[] paymentReceipt, string contentType)
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
                        return -1;
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

        public async Task<DataTable> test_a()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = "SELECT * FROM students_in_exam ; ";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Define the parameter and assign its value
                    //cmd.Parameters.AddWithValue("@studentId", studentId);

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

        public async Task<int> isAdded(uint examStudentId, uint examCourseId)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to check if an entry already exists and get the primary key if it does
                string checkQuery = "SELECT * FROM student_registration WHERE exam_student_id = @examStudentId AND exam_course_id = @examCourseId;";

                // MySqlCommand to execute the check query
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, _connection))
                {
                    checkCmd.Parameters.AddWithValue("@examStudentId", examStudentId);
                    checkCmd.Parameters.AddWithValue("@examCourseId", examCourseId);

                    object result = await checkCmd.ExecuteScalarAsync();

                    return result != null ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        public async Task<DataTable> getInitialRegisteredCourses(uint examStudentId)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = "SELECT exam_course_id FROM student_registration WHERE exam_student_id = @examStudentId; ";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Define the parameter and assign its value
                    cmd.Parameters.AddWithValue("@examStudentId", examStudentId);

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
