using MySqlConnector;
using System.Data;
using System.Threading.Tasks;
using ExamRegistrationUoJ.Services.DBInterfaces;
using Newtonsoft.Json;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceAdvisor1
    {
        public async Task<string> GetExamDetails(int exam_id)
        {
            string result = string.Empty;
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
            SELECT name, semester_id, batch 
            FROM exams 
            WHERE id = @exam_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var examDetails = new 
                            {
                                Name = reader["name"].ToString(),
                                SemesterId = reader["semester_id"].ToString(),
                                Batch = reader["batch"].ToString()
                            };

                            result = JsonConvert.SerializeObject(examDetails);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return result;
        }

        //Advisor Approval Update
        public async Task AdvisorApproval(int acc_id, int exam_id)
        {
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
            UPDATE students_in_exam 
            SET advisor_approved = 1 
            WHERE student_id = (
                SELECT id FROM students WHERE account_id = @acc_id
            ) AND exam_id = @exam_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@acc_id", acc_id);
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }
        }
        
        //Advisor Rejection Update
        public async Task AdvisorRejection(int acc_id, int exam_id)
        {
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
            UPDATE students_in_exam 
            SET advisor_approved = 0
            WHERE student_id = (
                SELECT id FROM students WHERE account_id = @acc_id
            ) AND exam_id = @exam_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@acc_id", acc_id);
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }
        }

        
        //get Registration Number
        public async Task<string> getStudentRegNo(int acc_id)
        {
            int regNo = -1; 
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                    SELECT s.id FROM students s
                    WHERE s.account_id = @acc_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@acc_id", acc_id);

                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            regNo = reader.GetInt32("id");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return regNo.ToString();
        }

        public async Task<string> getStudentName(int acc_id)
        {
            string name = ""; 
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
            SELECT a.name 
            FROM accounts a
            JOIN students s ON s.account_id = a.id
            WHERE s.account_id = @acc_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@acc_id", acc_id);

                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            name = reader.GetString("name");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return name;
        }
        
        // Get Re-Attempt Details
        public async Task<DataTable> GetReAttemptDetails(int acc_id, int exam_id)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
            SELECT 
                c.code AS courseCode, 
                c.name AS courseName, 
                sie.is_proper AS isProper, 
                sr.is_approved AS approvalStatus, 
                a.name AS coordinatorName
            FROM 
                students s
                JOIN student_registration sr ON s.id = sr.exam_student_id
                JOIN students_in_exam sie ON s.id = sie.student_id
                JOIN courses c ON sr.exam_course_id = c.id
                JOIN accounts a ON sie.id = a.id
                JOIN exams e ON sr.exam_student_id = e.id
            WHERE 
                s.account_id = @acc_id AND e.id = @exam_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@acc_id", acc_id);
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            finally
            {
                if (_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return dataTable;
        }


    }
}