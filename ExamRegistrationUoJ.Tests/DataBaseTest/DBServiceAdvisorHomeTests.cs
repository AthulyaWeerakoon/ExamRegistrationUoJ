using System;
using System.Data;
using System.Threading.Tasks;
using Moq;
using MySqlConnector;
using Xunit;
using ExamRegistrationUoJ.Services.MySQL;
using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Extensions.Configuration;

namespace ExamRegistrationUoJ.Tests.DataBaseTest
{
    public class DBServiceAdvisorHomeTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<MySqlConnection> _mockConnection;
        private readonly Mock<MySqlCommand> _mockCommand;
        private readonly Mock<MySqlDataReader> _mockReader;
        private readonly DBMySQL _dbService;

        public DBServiceAdvisorHomeTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConnection = new Mock<MySqlConnection>();
            _mockCommand = new Mock<MySqlCommand>();
            _mockReader = new Mock<MySqlDataReader>();

            // Set up the mock configuration to return a connection string
            var connectionString = "server=localhost;port=3306;database=mydb;user=root;password=mypassword;";
            _mockConfiguration.Setup(config => config.GetSection("ConnectionStrings:DefaultConnection").Value)
                .Returns(connectionString);

            // Initialize the DBMySQL class with the mocked configuration
            _dbService = new DBMySQL(_mockConfiguration.Object);
        }

        [Fact]
        public async Task GetExams_ReturnsDataTable()
        {
            // Arrange
            int departmentId = 1;
            int semesterId = 1;
            var dataTable = new DataTable();

            // Mock the connection state
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);

            // Mock the command creation
            _mockCommand.Setup(cmd => cmd.ExecuteReaderAsync(default)).ReturnsAsync(_mockReader.Object);

            // Mock the reader behavior
            _mockReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(false);
            _mockReader.Setup(r => r.HasRows).Returns(true);

            // Act
            var result = await _dbService.getExams(departmentId, semesterId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DataTable>(result);
        }

        [Fact]
        public async Task GetExamForAdvisorApproval_ReturnsDataTable()
        {
            // Arrange
            int semesterId = 1;
            var dataTable = new DataTable();

            // Mock the connection state
            _mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);

            // Mock the command creation
            _mockCommand.Setup(cmd => cmd.ExecuteReaderAsync(default)).ReturnsAsync(_mockReader.Object);

            // Mock the reader behavior
            _mockReader.SetupSequence(r => r.Read())
                .Returns(true)
                .Returns(false);
            _mockReader.Setup(r => r.HasRows).Returns(true);

            // Act
            var result = await _dbService.getExamForAdvisorApproval(semesterId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DataTable>(result);
        }
    }
}
