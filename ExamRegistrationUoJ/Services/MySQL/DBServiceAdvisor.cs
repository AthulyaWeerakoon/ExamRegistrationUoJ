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
            int regNo = -1; // Initialize regNo to an invalid value or your default value
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

       
    }
}