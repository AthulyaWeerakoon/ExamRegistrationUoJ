using MySqlConnector;
using System.Data;
using Newtonsoft.Json;

namespace ExamRegistrationUoJ.Services
{
    public class DBMySQL : DBInterface
    {
        private const string sakileTestJson = "[{\"title\":\"GANGS PRIDE\",\"description\":\"A Taut Character Study of a Woman And a A Shark who must Confront a Frisbee in Berlin\",\"rating\":\"PG-13\"},{\"title\":\"DARN FORRESTER\",\"description\":\"A Fateful Story of a A Shark And a Explorer who must Succumb a Technical Writer in A Jet Boat\",\"rating\":\"G\"},{\"title\":\"CHICAGO NORTH\",\"description\":\"A Fateful Yarn of a Mad Cow And a Waitress who must Battle a Student in California\",\"rating\":\"PG-13\"},{\"title\":\"SWEET BROTHERHOOD\",\"description\":\"A Unbelieveable Epistle of a Sumo Wrestler And a Hunter who must Chase a Forensic Psychologist in A Baloon\",\"rating\":\"R\"},{\"title\":\"HOME PITY\",\"description\":\"A Touching Panorama of a Man And a Secret Agent who must Challenge a Teacher in A MySQL Convention\",\"rating\":\"R\"}]";

        private MySqlConnection? _connection;
        private IConfiguration _configuration;

        public DBMySQL(IConfiguration configuration)
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
                string database = _configuration.GetValue<string>("MySQL:Database");
                string ConnectionString = $"Server={instance};Database={database};User ID={uid};Password={password};";
                _connection = new MySqlConnection(ConnectionString);
            }
        }

        // This method is only here as a placeholder, remove it once the weather page has been replaced
        public async Task<DataTable?> GetMostRentedFromSakila()
        {
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(sakileTestJson);

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
            return true;
        }
    }
}
