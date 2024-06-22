using MySqlConnector;
using System.Data;
using System.Threading.Tasks;
using ExamRegistrationUoJ.Services.DBInterfaces;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceAdvisor1
    {
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
        
        //Get Re-Attempt Details
        public async Task<DataTable> GetReAttemptDetails(int regNo)
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
            WHERE 
                s.regNo = @regNo";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@regNo", regNo);

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