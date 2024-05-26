using MySqlConnector;

namespace ExamRegistrationUoJ.Services
{
    public class ExamAuth : AuthInterface
    {
        private MySqlConnection authConnection;
        const string connectionString = $"Server=exam-reg-db.cd0iw8m2eq1n.ap-southeast-1.rds.amazonaws.com;Database=ExamRegistration;User ID=system_portal;Password=XkNkwp1LpCIbMowRPzQHFMKfhqui5CWe;";
        const string fetchIsARole = "select accounts.id from {0} join accounts on accounts.id = {0}.account_id where accounts.ms_email = @m";

        public ExamAuth() {
            try
            {
                authConnection = new MySqlConnection(connectionString);
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Cannot connect to Database for Authorization: {ex.Message}");
            }
        }

        private async Task<bool> isARoleFromDB(char role, string ms_email) {
            string query;
            switch(role)
            {
                case 'A':
                    query = string.Format(fetchIsARole, "administrators");
                    break;
                case 'a':
                    query = string.Format(fetchIsARole, "advisors");
                    break;
                case 'c':
                    query = string.Format(fetchIsARole, "coordinators");
                    break;
                case 's':
                    query = string.Format(fetchIsARole, "students");
                    break;
                default:
                    throw new InvalidDataException($"Invalid role character: {role}");
            }

            await authConnection.OpenAsync();
            using(var cmd = new MySqlCommand())
            {
                cmd.Connection = authConnection;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("m", ms_email);

                using MySqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows) {
                    await authConnection.CloseAsync();
                    return true;
                }

                await authConnection.CloseAsync();
                return false;
            }
        }

        public async Task<bool> IsAnAdministrator(string? email)
        {
            return email != null && await isARoleFromDB('A', email);
        }

        public async Task<bool> IsACoordinator(string? email, string? nameidentifier)
        {
            return email != null && await isARoleFromDB('c', email);
        }

        public async Task<bool> IsAStudent(string? email, string? nameidentifier)
        {
            return email != null && await isARoleFromDB('s', email);
        }

        public async Task<bool> IsAnAdvisor(string? email)
        {
            return email != null && await isARoleFromDB('a', email);
        }

        public async Task<bool> IsBothAdvisorCoordinator(string? email, string? nameidentifier)
        {
            return await IsAnAdvisor(email) || await IsACoordinator(email, nameidentifier);
        }
    }
}
