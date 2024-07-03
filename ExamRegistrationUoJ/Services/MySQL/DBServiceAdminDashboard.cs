using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections.Specialized;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Runtime.Intrinsics.Arm;
using static ExamRegistrationUoJ.Components.Pages.Administrator.AdminDashboard;

// Bhagya's workspace! Do not mess with me laddie!
// Contains Registration Fetch Services as well
namespace ExamRegistrationUoJ.Services.MySQL
{
    partial class DBMySQL : IDBServiceAdminDashboard, IDBRegistrationFetchService
    {
        public async Task<int> AddAdvisor(string name, string email)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            int accountId;
            using (var command = new MySqlCommand("INSERT INTO accounts (name, ms_email) VALUES (@name, @Email); SELECT LAST_INSERT_ID();", _connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@Email", email);
                accountId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            int advId;
            if (accountId > 0)
            {
                using (var command = new MySqlCommand("INSERT INTO advisors (account_id) VALUES (@accountId); SELECT LAST_INSERT_ID();", _connection))
                {
                    command.Parameters.AddWithValue("@accountId", accountId);
                    advId = Convert.ToInt32(await command.ExecuteScalarAsync());
                }

                return advId;
            }

            throw new Exception("Account Insertion Failed");
        }

        public async Task<int> AddCourse(string code, string name, int semesterId)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            int courseId;
            using (var command = new MySqlCommand("INSERT INTO courses (code, name, semester_id) VALUES (@code, @name, @semesterId); SELECT LAST_INSERT_ID();", _connection))
            {
                command.Parameters.AddWithValue("@code", code);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@semesterId", semesterId);
                courseId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            return courseId;
        }

        public async Task<int> AddDepartment(string name)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            int deptId;
            using (var command = new MySqlCommand("INSERT INTO departments (name) VALUES (@name); SELECT LAST_INSERT_ID();", _connection))
            {
                command.Parameters.AddWithValue("@name", name);
                deptId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            return deptId;
        }

        public async Task<int> AddSemester(string name)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            int semId;
            using (var command = new MySqlCommand("INSERT INTO semesters (name) VALUES (@name); SELECT LAST_INSERT_ID();", _connection))
            {
                command.Parameters.AddWithValue("@name", name);
                semId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            return semId;
        }

        public async Task<DataTable> getAllCourses()
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            var dataTable = new DataTable();

            using (var command = new MySqlCommand("SELECT * FROM courses", _connection))
            {
                using (var adapter = new MySqlDataAdapter(command))
                {
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }

            return dataTable;
        }

        public async Task<DataTable> getAdvisors()
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            var dataTable = new DataTable();

            using (var command = new MySqlCommand("SELECT ad.id AS id, ac.ms_email AS email, ac.name AS name FROM advisors ad JOIN accounts ac ON ac.id = ad.account_id", _connection))
            {
                using (var adapter = new MySqlDataAdapter(command))
                {
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }

            return dataTable;
        }

        public async Task UpdateAdvisor(int advisorId, string newName, string newEmail)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            int accountId;
            using (var command = new MySqlCommand("SELECT account_id FROM advisors WHERE id = @advisorId", _connection))
            {
                command.Parameters.AddWithValue("@advisorId", advisorId);
                accountId = Convert.ToInt32(await command.ExecuteScalarAsync());
            }

            if (accountId > 0)
            {
                // Update the name and email in the accounts table
                using (var command = new MySqlCommand("UPDATE accounts SET name = @newName, ms_email = @newEmail WHERE id = @accountId", _connection))
                {
                    command.Parameters.AddWithValue("@newName", newName);
                    command.Parameters.AddWithValue("@newEmail", newEmail);
                    command.Parameters.AddWithValue("@accountId", accountId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateCourse(int courseId, string newCode, string newName, int newSemesterId)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            using (var command = new MySqlCommand("UPDATE courses SET code = @code, name = @name, semester_id = @semesterId WHERE id = @id", _connection))
            {
                command.Parameters.AddWithValue("@code", newCode);
                command.Parameters.AddWithValue("@name", newName);
                command.Parameters.AddWithValue("@semesterId", newSemesterId);
                command.Parameters.AddWithValue("@id", courseId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateDepartmentName(int departmentId, string newName)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            using (var command = new MySqlCommand("UPDATE departments SET name = @name WHERE id = @id", _connection))
            {
                command.Parameters.AddWithValue("@name", newName);
                command.Parameters.AddWithValue("@id", departmentId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateSemesterName(int semesterId, string newName)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            using (var command = new MySqlCommand("UPDATE semesters SET name = @name WHERE id = @id", _connection))
            {
                command.Parameters.AddWithValue("@name", newName);
                command.Parameters.AddWithValue("@id", semesterId);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task<string> DropItemAsync(string query, int itemId)
        {
            try
            {
                if (_connection?.State != ConnectionState.Open)
                    OpenConnection();

                using (MySqlCommand cmd = new MySqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", itemId);
                    await cmd.ExecuteNonQueryAsync();
                }
                return null; // Success
            }
            catch (MySqlException ex)
            {
                return ex.Message; // Return error message
            }
        }

        public async Task<string> DropDepartment(int departmentId)
        {
            return await DropItemAsync("DELETE FROM departments WHERE id = @id", departmentId);
        }

        public async Task<string> DropSemester(int semesterId)
        {
            return await DropItemAsync("DELETE FROM semesters WHERE id = @id", semesterId);
        }

        public async Task<string> DropAdvisor(int advisorId)
        {
            using (var transaction = await _connection.BeginTransactionAsync())
            {
                try
                {
                    // Retrieve the account_id associated with the advisor
                    string getAccountIdQuery = "SELECT account_id FROM advisors WHERE id = @advisorId";
                    MySqlCommand getAccountIdCommand = new MySqlCommand(getAccountIdQuery, _connection);
                    getAccountIdCommand.Parameters.AddWithValue("@advisorId", advisorId);
                    var accountId = (int?)await getAccountIdCommand.ExecuteScalarAsync();

                    if (accountId == null)
                    {
                        return "Advisor not found";
                    }

                    // Delete the advisor
                    string deleteAdvisorQuery = "DELETE FROM advisors WHERE id = @advisorId";
                    MySqlCommand deleteAdvisorCommand = new MySqlCommand(deleteAdvisorQuery, _connection);
                    deleteAdvisorCommand.Parameters.AddWithValue("@advisorId", advisorId);
                    await deleteAdvisorCommand.ExecuteNonQueryAsync();

                    // If advisor delete successful, delete the account
                    string deleteAccountResult = await DropAccount(accountId.Value);
                    if (deleteAccountResult != null)
                    {
                        await transaction.RollbackAsync();
                        return deleteAccountResult;
                    }

                    await transaction.CommitAsync();
                    return null;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ex.Message;
                }
            }
        }

        private async Task<string> DropAccount(int value)
        {
            return await DropItemAsync("DELETE FROM accounts WHERE id = @id", value);
        }

        public async Task<string> DropCourse(int courseId)
        {
            return await DropItemAsync("DELETE FROM courses WHERE Id = @Id", courseId);
        }

        public async Task<DataTable> getRegDescription(int exam_id, int student_id)
        {
            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            var dataTable = new DataTable();

            string query = @"
                    SELECT 
                        ac.name AS name,
                        ac.ms_email AS email,
                        se.name AS semester,
                        ex.name AS exam_name,
                        ex.batch AS batch
                    FROM 
                        students_in_exam sx
                    JOIN students st ON st.id = sx.student_id
                    JOIN accounts ac ON ac.id = st.account_id
                    JOIN exams ex ON ex.id = sx.exam_id
                    JOIN semesters se ON ex.semester_id = se.id
                    WHERE 
                        sx.student_id = @sid AND sx.exam_id = @eid AND sx.advisor_approved='1';
                ";

            using (var command = new MySqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@sid", student_id);
                command.Parameters.AddWithValue("@eid", exam_id);
                using (var adapter = new MySqlDataAdapter(command))
                {
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }

            query = @"
                    SELECT d.name AS name
                    FROM student_registration sr
                    JOIN students_in_exam sie ON sr.exam_student_id = sie.id
                    JOIN courses_in_exam cie ON sr.exam_course_id = cie.id
                    JOIN departments d ON cie.department_id = d.id
                    WHERE sie.student_id = @student_id AND sie.exam_id = @exam_id
                    GROUP BY d.id, d.name
                    ORDER BY COUNT(sr.exam_course_id) DESC
                    LIMIT 1;
                ";

            using (var command = new MySqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@sid", student_id);
                command.Parameters.AddWithValue("@eid", exam_id);

                // add column to datatable to insert department name
                dataTable.Columns.Add("dept_name", typeof(string));
                dataTable.Rows[0]["dept_name"] = Convert.ToString(await command.ExecuteScalarAsync());
            }

            return dataTable;
        }

        public async Task<DataTable> getRegCourses(int exam_id, int student_id)
        {

            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            var dataTable = new DataTable();

            string query = @"
                    SELECT 
                        co.code AS code,
                        co.name AS name,
                        sx.is_proper AS is_proper,
                        sr.attendance AS attendance,
                        sr.is_approved AS is_approved
                    FROM
                        student_registration sr
                    JOIN courses_in_exam cx ON cx.id = sr.exam_course_id
                    JOIN students_in_exam sx ON sx.id = sr.exam_student_id
                    JOIN courses co ON co.id = cx.course_id
                    JOIN exams ex ON ex.id = sx.exam_id
                    WHERE 
                        sx.student_id = @sid AND sx.exam_id = @eid AND sx.advisor_approved='1';
                ";

            using (var command = new MySqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@sid", student_id);
                command.Parameters.AddWithValue("@eid", exam_id);
                using (var adapter = new MySqlDataAdapter(command))
                {
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }

            return dataTable;
        }

        public async Task<DataTable> getApprovedStudents(int exam_id)
        {

            if (_connection?.State != ConnectionState.Open)
                OpenConnection();

            var dataTable = new DataTable();

            string query = @"
                    SELECT 
                        sx.student_id AS student_id,
                        ac.name AS student_name,
                        ac.ms_email AS email
                    FROM 
                        students_in_exam sx
                    JOIN students st ON st.id = sx.student_id
                    JOIN accounts ac ON ac.id = st.account_id
                    WHERE 
                        sx.exam_id = @eid AND sx.advisor_approved='1';
                ";

            using (var command = new MySqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@eid", exam_id);
                using (var adapter = new MySqlDataAdapter(command))
                {
                    await Task.Run(() => adapter.Fill(dataTable));
                }
            }

            return dataTable;
        }
    }
}
