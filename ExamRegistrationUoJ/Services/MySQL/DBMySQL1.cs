using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections.Specialized;


// ramith's workspace
namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL :  IDBServiceAdmin1
    {
        // remove this attribute with the removal of the weather page, this is for the placeholder method
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

            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }



        public async Task<DataTable> getDepartments()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select department id and name from the departments table
                string query = "SELECT id, name FROM departments";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
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


        public async Task<DataTable> getSemesters()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select semester id and name from the semesters table
                string query = "SELECT id, name FROM semesters";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
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


        public async Task<DataTable> getActiveExams()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // Get the current date
                DateTime currentDate = DateTime.Now;

                // SQL query to select active exams
                string query = "SELECT e.id, e.name, e.batch, e.semester_id, s.name AS semester, e.is_confirmed AS status, e.end_date " +
                               "FROM exams e " +
                               "JOIN semesters s ON e.semester_id = s.id " +
                               "WHERE e.end_date > @currentDate";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the current date
                    cmd.Parameters.AddWithValue("@currentDate", currentDate);

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


        public async Task<DataTable> getCompletedExams()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // Get the current date
                DateTime currentDate = DateTime.Now;

                // SQL query to select completed exams
                string query = "SELECT e.id, e.name, e.batch, e.semester_id, s.name AS semester, e.is_confirmed AS status, e.end_date " +
                               "FROM exams e " +
                               "JOIN semesters s ON e.semester_id = s.id " +
                               "WHERE e.end_date <= @currentDate";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the current date
                    cmd.Parameters.AddWithValue("@currentDate", currentDate);

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


        public async Task<DataTable> getAllCoursesInExam()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select all courses in exams and their linking departments
                string query = "SELECT c.name AS course_name, c.code AS course_code, cie.exam_id, cie.department_id AS dept_id " +
                               "FROM courses_in_exam cie " +
                               "JOIN courses c ON cie.course_id = c.id";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
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


        public async Task<string> getExamTitle(int exam_id)
        {
            string examTitle = "";

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select the exam name based on the exam ID
                string query = "SELECT name FROM exams WHERE id = @exam_id";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the exam ID
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    // Execute the query and retrieve the exam name
                    object result = await cmd.ExecuteScalarAsync();

                    // Check if the result is not null
                    if (result != null)
                    {
                        examTitle = Convert.ToString(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            return examTitle;
        }


        public async Task<int> getExamSemester(int exam_id)
        {
            int semesterId = -1; // Default value in case no semester is found

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select the semester ID based on the exam ID
                string query = "SELECT semester_id FROM exams WHERE id = @exam_id";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the exam ID
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    // Execute the query and retrieve the semester ID
                    object result = await cmd.ExecuteScalarAsync();

                    // Check if the result is not null
                    if (result != null)
                    {
                        semesterId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            return semesterId;
        }


        public async Task<string?> getExamBatch(int exam_id)
        {
            string? examBatch = null;

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select the exam batch based on the exam ID
                string query = "SELECT batch FROM exams WHERE id = @exam_id";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the exam ID
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    // Execute the query and retrieve the exam batch
                    object result = await cmd.ExecuteScalarAsync();

                    // Check if the result is not null
                    if (result != null)
                    {
                        examBatch = Convert.ToString(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            return examBatch;
        }


        public async Task<DateTime?> getExamEndDate(int exam_id)
        {
            DateTime? examEndDate = null;

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select the exam end date based on the exam ID
                string query = "SELECT end_date FROM exams WHERE id = @exam_id";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the exam ID
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

                    // Execute the query and retrieve the exam end date
                    object result = await cmd.ExecuteScalarAsync();

                    // Check if the result is not null
                    if (result != null)
                    {
                        examEndDate = Convert.ToDateTime(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            return examEndDate;
        }


        public async Task<DataTable?> getCoursesInExam(int exam_id)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select courses in the given exam
                string query = "SELECT c.name AS course_name, c.code AS course_code, cie.department_id AS dept_id, " +
                               "cie.coordinator_id, co.name AS coordinator_name " +
                               "FROM courses_in_exam cie " +
                               "JOIN courses c ON cie.course_id = c.id " +
                               "LEFT JOIN coordinators co ON cie.coordinator_id = co.id " +
                               "WHERE cie.exam_id = @exam_id";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameter for the exam ID
                    cmd.Parameters.AddWithValue("@exam_id", exam_id);

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


    }
}
