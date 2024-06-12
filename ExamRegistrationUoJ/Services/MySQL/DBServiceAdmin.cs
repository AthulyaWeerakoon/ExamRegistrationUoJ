using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections.Specialized;


// ramith's workspace
namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceAdmin1
    {

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
                string query = @"
                    SELECT 
                        e.id AS id,
                        e.name AS name,
                        e.batch AS batch,
                        e.semester_id AS semester_id,
                        s.name AS semester,
                        e.is_confirmed AS status,
                        e.end_date AS end_date
                    FROM 
                        exams e
                    JOIN 
                        semesters s ON e.semester_id = s.id
                    WHERE 
                        e.is_confirmed = 0 OR
                        DATE_ADD(e.end_date, INTERVAL COALESCE(e.coordinator_approval_extension, 0) + COALESCE(e.advisor_approval_extension, 0) DAY) > CURDATE();
                ";

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
                string query = @"
                    SELECT 
                        e.id AS id,
                        e.name AS name,
                        e.batch AS batch,
                        e.semester_id AS semester_id,
                        s.name AS semester,
                        e.is_confirmed AS status,
                        e.end_date AS end_date
                    FROM 
                        exams e
                    JOIN 
                        semesters s ON e.semester_id = s.id
                    WHERE 
                        e.is_confirmed = 1 OR
                        DATE_ADD(e.end_date, INTERVAL COALESCE(e.coordinator_approval_extension, 0) + COALESCE(e.advisor_approval_extension, 0) DAY) <= CURDATE()";

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

        public async Task<DataTable> getExamDescription(int exam_id)
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
                string query = @"
                    SELECT 
                        e.name AS name,
                        e.semester_id AS semester_id,
                        e.batch AS batch,
                        e.end_date AS end_date,
                        e.coordinator_approval_extension AS coordinator_approval_extension,
                        e.advisor_approval_extension AS advisor_approval_extension,
                        e.is_confirmed AS is_confirmed
                    FROM 
                        exams e
                    WHERE 
                        e.id = @exam_id";

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

        public async Task<DataTable?> getCoursesInExam(int exam_id)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select courses in the given exam
                string query = @"
                    SELECT 
                        cie.id AS id,
                        c.name AS course_name,
                        c.code AS course_code,
                        cie.coordinator_id AS coordinator_id,
                        cie.department_id AS dept_id
                    FROM 
                        courses_in_exam cie
                    JOIN 
                        courses c ON cie.course_id = c.id
                    WHERE 
                        cie.exam_id = @exam_id";

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

        public async Task<DataTable?> getCoordinators()
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select coordinators
                string query = @"
                    SELECT 
                        id,
                        ms_email AS email
                    FROM 
                        coordinators";

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

            // Return null if the DataTable is empty
            if (dataTable.Rows.Count == 0)
                return null;

            return dataTable;
        }

        public async Task addCoordinator(string email)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to insert a new coordinator
                string query = @"
            INSERT INTO coordinators (ms_email)
            VALUES (@email)";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add parameters
                    cmd.Parameters.AddWithValue("@email", email);

                    // Execute the query
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


        public async Task saveChanges(string? examTitle, int? semester, string? batch, int? cordTimeExtent, int? adviTimeExtent, List<int>? removeList, DataTable? updateList, DataTable? addList)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // Begin a transaction
                using (var transaction = _connection.BeginTransaction())
                {
                    // Update exam details if provided
                    if (examTitle != null || semester != null || batch != null || cordTimeExtent != null || adviTimeExtent != null)
                    {
                        string updateExamQuery = "UPDATE exams SET ";
                        List<string> updateFields = new List<string>();
                        if (examTitle != null)
                            updateFields.Add("name = @examTitle");
                        if (semester != null)
                            updateFields.Add("semester_id = @semester");
                        if (batch != null)
                            updateFields.Add("batch = @batch");
                        if (cordTimeExtent != null)
                            updateFields.Add("coordinator_approval_extension = @cordTimeExtent");
                        if (adviTimeExtent != null)
                            updateFields.Add("advisor_approval_extension = @adviTimeExtent");

                        updateExamQuery += string.Join(", ", updateFields) + " WHERE id = @examId";

                        using (MySqlCommand cmd = new MySqlCommand(updateExamQuery, _connection, transaction))
                        {
                            if (examTitle != null)
                                cmd.Parameters.AddWithValue("@examTitle", examTitle);
                            if (semester != null)
                                cmd.Parameters.AddWithValue("@semester", semester);
                            if (batch != null)
                                cmd.Parameters.AddWithValue("@batch", batch);
                            if (cordTimeExtent != null)
                                cmd.Parameters.AddWithValue("@cordTimeExtent", cordTimeExtent);
                            if (adviTimeExtent != null)
                                cmd.Parameters.AddWithValue("@adviTimeExtent", adviTimeExtent);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    // Remove courses if removeList is provided
                    if (removeList != null && removeList.Count > 0)
                    {
                        string removeCoursesQuery = "DELETE FROM courses_in_exam WHERE id = @courseInExamId";
                        using (MySqlCommand cmd = new MySqlCommand(removeCoursesQuery, _connection, transaction))
                        {
                            foreach (var courseId in removeList)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@courseInExamId", courseId);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Update courses if updateList is provided
                    if (updateList != null && updateList.Rows.Count > 0)
                    {
                        string updateCoursesQuery = "UPDATE courses_in_exam SET course_id = @courseId, department_id = @departmentId, coordinator_id = @coordinatorId WHERE id = @courseInExamId";
                        using (MySqlCommand cmd = new MySqlCommand(updateCoursesQuery, _connection, transaction))
                        {
                            foreach (DataRow row in updateList.Rows)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@courseInExamId", row["id"]);
                                cmd.Parameters.AddWithValue("@courseId", row["course_id"]);
                                cmd.Parameters.AddWithValue("@departmentId", row["department_id"]);
                                cmd.Parameters.AddWithValue("@coordinatorId", row["coordinator_id"] == DBNull.Value ? (object)DBNull.Value : row["coordinator_id"]);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Add new courses if addList is provided
                    if (addList != null && addList.Rows.Count > 0)
                    {
                        string addCoursesQuery = "INSERT INTO courses_in_exam (exam_id, course_id, department_id, coordinator_id) VALUES (@examId, @courseId, @departmentId, @coordinatorId)";
                        using (MySqlCommand cmd = new MySqlCommand(addCoursesQuery, _connection, transaction))
                        {
                            foreach (DataRow row in addList.Rows)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@examId", row["exam_id"]);
                                cmd.Parameters.AddWithValue("@courseId", row["course_id"]);
                                cmd.Parameters.AddWithValue("@departmentId", row["department_id"]);
                                cmd.Parameters.AddWithValue("@coordinatorId", row["coordinator_id"] == DBNull.Value ? (object)DBNull.Value : row["coordinator_id"]);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }


 












        // additional methods which was previously neede but not now 
        // dola did this to me

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

        Task<int> IDBServiceAdmin1.addCoordinator(string email)
        {
            throw new NotImplementedException();
        }
    }
}
