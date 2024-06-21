using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceAdvisor1
    {
        string regNo = "";
        public async Task<String> getStudentRegNo(int acc_id)
        {
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = @"
                    SELECT s.id FROM students s
                    WHERE s.account_id = @acc_id";

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@acc_id", regNo);

                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            regNo = reader.GetString("id");
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
                if(_connection?.State == ConnectionState.Open)
                    _connection.Close();
            }

            return regNo;
        }
    }
}

