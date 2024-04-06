using MySqlConnector;
using System.Collections.Generic;
using System.Data;

namespace ExamRegistrationUoJ.Services
{
    public class DBSakilaTest : DBInterface
    {
        // private const string getMostRentedFromSakila = "select first_name, last_name, count(inventory_id) as count from rental join customer on rental.customer_id = customer.customer_id group by rental.customer_id order by count(inventory_id) desc limit 10"; from rental join customer on rental.customer_id = customer.customer_id group by rental.customer_id order by count(inventory_id) desc limit 10";
        private const string sakilaTest = "select title, description, rating from film order by length desc limit 5";

        private MySqlConnection? _connection;
        private IConfiguration _configuration;

        public DBSakilaTest(IConfiguration configuration)
        {
            _configuration = configuration;
            OpenConnection();
        }

        private void OpenConnection()
        {
            if (_connection == null)
            {
                string instance = _configuration.GetValue<string>("MySQL:Instance");
                string password = _configuration.GetValue<string>("MySQL:Password");
                string uid = _configuration.GetValue<string>("MySQL:UserID");
                string ConnectionString = $"Server={instance};Database=sakila;User ID={uid};Password={password};";
                _connection = new MySqlConnection(ConnectionString);
            }
        }

        public async Task<DataTable> GetMostRentedFromSakila()
        {
            await _connection.OpenAsync();

            using var cmd = new MySqlCommand();
            cmd.Connection = _connection;
            cmd.CommandText = sakilaTest;

            using var reader = await cmd.ExecuteReaderAsync();
            var dataTable = new DataTable();
            dataTable.Load(reader);

            await _connection.CloseAsync();

            return dataTable;
        }

        public async Task<bool> IsAnAdministrator(string nameidentifier)
        {
            return false;
        }

        public async Task<bool> IsACoordinator(string nameidentifier)
        {
            return false;
        }

        public async Task<bool> IsAStudent(string nameidentifier)
        {
            return false;
        }

        public async Task<bool> IsAnAdvisor(string nameidentifier)
        {
            return false;
        }
    }
}
