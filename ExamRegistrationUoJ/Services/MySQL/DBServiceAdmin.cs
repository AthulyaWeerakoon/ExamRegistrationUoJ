using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections.Specialized;
using System.Text;


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
                string ConnectionString = $"Server={instance};Database={database};User ID={uid};Password={password};Allow User Variables=true;";
                
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
                        DATE_ADD(e.end_date, INTERVAL COALESCE(e.coordinator_approval_extension, 0) + COALESCE(e.advisor_approval_extension, 0) WEEK) >= CURDATE();
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
                        e.is_confirmed = 1 AND
                        DATE_ADD(e.end_date, INTERVAL COALESCE(e.coordinator_approval_extension, 0) + COALESCE(e.advisor_approval_extension, 0) WEEK) < CURDATE()";

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
                        c.id AS course_id,
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
                    c.id AS id, 
                    a.ms_email AS email
                FROM 
                    coordinators c
                JOIN 
                    accounts a ON c.account_id = a.id";

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

        public async Task<int> addCoordinator(string email)
        {
            int coordinatorId;

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to insert account and coordinator, then retrieve the coordinator ID
                string query = @"
                    INSERT INTO accounts (nameidentifier, name, ms_email)
                    VALUES (UUID(), 'placeholder', @Email);
                    SELECT LAST_INSERT_ID() INTO @accountId;
                    INSERT INTO coordinators (account_id)
                    VALUES (@accountId);
                    SELECT LAST_INSERT_ID() AS coordinator_id;";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    // Add the email parameter
                    cmd.Parameters.AddWithValue("@Email", email);

                    // Execute the query and retrieve the coordinator ID
                    coordinatorId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return coordinatorId;
        }


        public async Task<int?> saveChanges(int? examId, string? examTitle, int? semester, string? batch, int? coordTimeExtent, int? adviTimeExtent, List<int>? removeList, DataTable? updateList, DataTable? addList)
        {
            int? newExamId = examId;

            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                using (MySqlTransaction transaction = _connection.BeginTransaction())
                {
                    // If examId is null, insert a new exam
                    if (examId == null)
                    {
                        string insertExamQuery = @"
                    INSERT INTO exams (name, semester_id, batch, coordinator_approval_extension, advisor_approval_extension)
                    VALUES (@examTitle, @semester, @batch, @coordTimeExtent, @adviTimeExtent);
                    SELECT LAST_INSERT_ID();";

                        using (MySqlCommand cmd = new MySqlCommand(insertExamQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@examTitle", examTitle);
                            cmd.Parameters.AddWithValue("@semester", semester);
                            cmd.Parameters.AddWithValue("@batch", batch);
                            cmd.Parameters.AddWithValue("@coordTimeExtent", coordTimeExtent);
                            cmd.Parameters.AddWithValue("@adviTimeExtent", adviTimeExtent);

                            newExamId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }
                    }
                    else
                    {
                        // Update existing exam
                        string updateExamQuery = @"
                    UPDATE exams
                    SET name = @examTitle, semester_id = @semester, batch = @batch,
                        coordinator_approval_extension = @coordTimeExtent, advisor_approval_extension = @adviTimeExtent
                    WHERE id = @examId";

                        using (MySqlCommand cmd = new MySqlCommand(updateExamQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@examTitle", examTitle);
                            cmd.Parameters.AddWithValue("@semester", semester);
                            cmd.Parameters.AddWithValue("@batch", batch);
                            cmd.Parameters.AddWithValue("@coordTimeExtent", coordTimeExtent);
                            cmd.Parameters.AddWithValue("@adviTimeExtent", adviTimeExtent);
                            cmd.Parameters.AddWithValue("@examId", examId);
                                    cmd.Parameters.AddWithValue("@Batch", batch);
                                if (coordTimeExtent.HasValue)
                                    cmd.Parameters.AddWithValue("@CoordTimeExtent", coordTimeExtent);
                                if (adviTimeExtent.HasValue)
                                    cmd.Parameters.AddWithValue("@AdviTimeExtent", adviTimeExtent);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    // Remove courses specified in removeList
                    if (removeList != null && removeList.Count > 0)
                    {
                        string removeCoursesQuery = "DELETE FROM courses_in_exam WHERE id IN (@removeList)";
                        using (MySqlCommand cmd = new MySqlCommand(removeCoursesQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@removeList", string.Join(",", removeList));
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    // Update courses specified in updateList
                    if (updateList != null && updateList.Rows.Count > 0)
                    {
                        foreach (DataRow row in updateList.Rows)
                        {
                            string updateCourseQuery = @"
                        UPDATE courses_in_exam
                        SET coordinator_id = @coordinator_id
                        WHERE id = @course_in_exam_id";

                            using (MySqlCommand cmd = new MySqlCommand(updateCourseQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@coordinator_id", row["coordinator_id"]);
                                cmd.Parameters.AddWithValue("@course_in_exam_id", row["course_in_exam_id"]);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Add new courses specified in addList
                    if (addList != null && addList.Rows.Count > 0)
                    {
                        foreach (DataRow row in addList.Rows)
                        {
                            string addCourseQuery = @"
                        INSERT INTO courses_in_exam (exam_id, course_id, department_id, coordinator_id)
                        VALUES (@exam_id, @course_id, @department_id, @coordinator_id)";

                            using (MySqlCommand cmd = new MySqlCommand(addCourseQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@exam_id", newExamId ?? row["exam_id"]);
                                cmd.Parameters.AddWithValue("@course_id", row["course_id"]);
                                cmd.Parameters.AddWithValue("@department_id", row["department_id"]);
                                cmd.Parameters.AddWithValue("@coordinator_id", row["coordinator_id"]);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return newExamId;
        }


        public async Task<DataTable> getCoursesFromDepartment(int deptId)
        {
            DataTable dataTable = new DataTable();
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                // SQL query to select course id, name, and code from the courses table
                string query = @"
            SELECT 
                c.id AS course_id, 
                c.name AS course_name, 
                c.code AS course_code
            FROM 
                courses c
            JOIN 
                course_departments cd ON c.id = cd.course_id
            WHERE 
                cd.department_id = @deptId";

                // MySqlCommand to execute the SQL query
                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@deptId", deptId);

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

        public async Task<bool> isExamFinalized(int examId)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                string query = "SELECT is_confirmed FROM exams WHERE id = @examId LIMIT 1;";
                using var command = new MySqlCommand(query, _connection);
                command.Parameters.AddWithValue("@examId", examId);

                var result = await command.ExecuteScalarAsync();

                if (result == null || Convert.ToInt32(result) == 1)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
                return true;
            }
        }

        public async Task finalizeExam(int examId)
        {
            try
            {
                // Open the connection if it's not already open
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                using (MySqlTransaction transaction = _connection.BeginTransaction())
                {
                    // Check if all required fields in the exam are non-null
                    string checkExamQuery = @"
                SELECT name, semester_id, batch, coordinator_approval_extension, advisor_approval_extension 
                FROM exams 
                WHERE id = @examId";

                    using (MySqlCommand cmd = new MySqlCommand(checkExamQuery, _connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@examId", examId);
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (reader.IsDBNull(0) || reader.IsDBNull(1) || reader.IsDBNull(2) || reader.IsDBNull(3) || reader.IsDBNull(4))
                                {
                                    throw new Exception("All fields of Exam description should have values, none should be null.");
                                }
                            }
                            else
                            {
                                throw new Exception("Exam not found.");
                            }
                        }
                    }

                    // Check if there is at least one course associated with the exam
                    string checkCoursesQuery = @"
                SELECT COUNT(*) 
                FROM courses_in_exam 
                WHERE exam_id = @examId";

                    using (MySqlCommand cmd = new MySqlCommand(checkCoursesQuery, _connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@examId", examId);
                        int courseCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        if (courseCount == 0)
                        {
                            throw new Exception("There must be at least one course in exam for this exam ID.");
                        }
                    }

                    // Set is_confirmed to 1
                    string updateExamQuery = @"
                UPDATE exams 
                SET is_confirmed = 1 
                WHERE id = @examId";

                    using (MySqlCommand cmd = new MySqlCommand(updateExamQuery, _connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@examId", examId);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }

        Task<int?> IDBServiceAdmin1.addOrSaveExamDescription(int? examId, string? examTitle, int? semester, string? batch, int? cordTimeExtent, int? adviTimeExtent)
        {
            throw new NotImplementedException();
        }

        Task IDBServiceAdmin1.saveCourseChanges(int examId, List<int>? removeList, DataTable? updateList, DataTable? addList)
        {
            throw new NotImplementedException();
        }
    }
}
