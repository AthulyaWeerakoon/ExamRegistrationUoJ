using System.Data;
using MySqlConnector;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;
namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceHome
    {
        public async Task<DataTable> GetActiveExamsHome()
        {
            DataTable dataTable = new DataTable();
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();
                
                string query = @"
                SELECT name, batch, semester_id 
                FROM ExamRegistration.exams 
                WHERE is_confirmed != 0";
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
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