using Xunit;
using Moq;
using FluentAssertions;
using System.Data;
using System.Threading.Tasks;
using ExamRegistrationUoJ.Services.DBInterfaces;
using ExamRegistrationUoJ.PageClasses.AdminPages;

namespace ExamRegistrationUoJ.Tests
{
    public class AdmininstratorDashboardTests
    {
        private readonly Mock<IDBServiceAdminDashboard> _mockDbService;
        private readonly AdmininstratorDashboard _dashboard;

        public AdmininstratorDashboardTests()
        {
            _mockDbService = new Mock<IDBServiceAdminDashboard>();
            _dashboard = new AdmininstratorDashboard(_mockDbService.Object);
        }

        [Fact]
        public async Task Init_ShouldInitializeAllData()
        {
            // Arrange
            _mockDbService.Setup(db => db.getSemesters()).ReturnsAsync(new DataTable());
            _mockDbService.Setup(db => db.getDepartments()).ReturnsAsync(new DataTable());
            _mockDbService.Setup(db => db.getAdvisors()).ReturnsAsync(new DataTable());
            _mockDbService.Setup(db => db.getAllCourses()).ReturnsAsync(new DataTable());

            // Act
            await _dashboard.init();

            // Assert
            _dashboard.Semesters.Should().NotBeNull();
            _dashboard.Departements.Should().NotBeNull();
            _dashboard.Advisors.Should().NotBeNull();
            _dashboard.Courses.Should().NotBeNull();
        }

        [Fact]
        public async Task GetSemesters_ShouldFetchSemesters()
        {
            // Arrange
            var semestersTable = new DataTable();
            _mockDbService.Setup(db => db.getSemesters()).ReturnsAsync(semestersTable);

            // Act
            var method = _dashboard.GetType().GetMethod("getSemesters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task)method.Invoke(_dashboard, null);
            await task;

            // Assert
            _dashboard.Semesters.Should().BeSameAs(semestersTable);
        }

        [Fact]
        public async Task GetDepartments_ShouldFetchDepartments()
        {
            // Arrange
            var departmentsTable = new DataTable();
            _mockDbService.Setup(db => db.getDepartments()).ReturnsAsync(departmentsTable);

            // Act
            var method = _dashboard.GetType().GetMethod("getDepartments", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task)method.Invoke(_dashboard, null);
            await task;

            // Assert
            _dashboard.Departements.Should().BeSameAs(departmentsTable);
        }

        [Fact]
        public async Task GetAdvisors_ShouldFetchAdvisors()
        {
            // Arrange
            var advisorsTable = new DataTable();
            _mockDbService.Setup(db => db.getAdvisors()).ReturnsAsync(advisorsTable);

            // Act
            var method = _dashboard.GetType().GetMethod("getAdvisors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task)method.Invoke(_dashboard, null);
            await task;

            // Assert
            _dashboard.Advisors.Should().BeSameAs(advisorsTable);
        }

        [Fact]
        public async Task GetCourses_ShouldFetchCourses()
        {
            // Arrange
            var coursesTable = new DataTable();
            _mockDbService.Setup(db => db.getAllCourses()).ReturnsAsync(coursesTable);

            // Act
            var method = _dashboard.GetType().GetMethod("getCourses", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var task = (Task)method.Invoke(_dashboard, null);
            await task;

            // Assert
            _dashboard.Courses.Should().BeSameAs(coursesTable);
        }

        [Fact]
        public async Task UpdateAdvisor_ShouldCallUpdateAdvisor()
        {
            // Arrange
            int advisorId = 1;
            string newName = "New Name";
            string newEmail = "newemail@example.com";

            // Act
            await _dashboard.UpdateAdvisor(advisorId, newName, newEmail);

            // Assert
            _mockDbService.Verify(db => db.UpdateAdvisor(advisorId, newName, newEmail), Times.Once);
        }

        [Fact]
        public async Task UpdateDepartmentName_ShouldCallUpdateDepartmentName()
        {
            // Arrange
            int departmentId = 1;
            string newName = "New Department Name";

            // Act
            await _dashboard.UpdateDepartmentName(departmentId, newName);

            // Assert
            _mockDbService.Verify(db => db.UpdateDepartmentName(departmentId, newName), Times.Once);
        }

        [Fact]
        public async Task UpdateSemesterName_ShouldCallUpdateSemesterName()
        {
            // Arrange
            int semesterId = 1;
            string newName = "New Semester Name";

            // Act
            await _dashboard.UpdateSemesterName(semesterId, newName);

            // Assert
            _mockDbService.Verify(db => db.UpdateSemesterName(semesterId, newName), Times.Once);
        }

        [Fact]
        public async Task UpdateCourse_ShouldCallUpdateCourse()
        {
            // Arrange
            int courseId = 1;
            string newCode = "New Code";
            string newName = "New Course Name";
            int newSemesterId = 2;
            int[] departments = new int[] { 1, 2 };

            // Act
            await _dashboard.UpdateCourse(courseId, newCode, newName, newSemesterId, departments);

            // Assert
            _mockDbService.Verify(db => db.UpdateCourse(courseId, newCode, newName, newSemesterId, departments), Times.Once);
        }

        [Fact]
        public async Task AddAdvisor_ShouldCallAddAdvisorAndReturnId()
        {
            // Arrange
            string name = "Advisor Name";
            string email = "advisor@example.com";
            int expectedId = 1;
            _mockDbService.Setup(db => db.AddAdvisor(name, email)).ReturnsAsync(expectedId);

            // Act
            var result = await _dashboard.AddAdvisor(name, email);

            // Assert
            result.Should().Be(expectedId);
            _mockDbService.Verify(db => db.AddAdvisor(name, email), Times.Once);
        }

        [Fact]
        public async Task AddDepartment_ShouldCallAddDepartmentAndReturnId()
        {
            // Arrange
            string name = "Department Name";
            int expectedId = 1;
            _mockDbService.Setup(db => db.AddDepartment(name)).ReturnsAsync(expectedId);

            // Act
            var result = await _dashboard.AddDepartment(name);

            // Assert
            result.Should().Be(expectedId);
            _mockDbService.Verify(db => db.AddDepartment(name), Times.Once);
        }

        [Fact]
        public async Task AddSemester_ShouldCallAddSemesterAndReturnId()
        {
            // Arrange
            string name = "Semester Name";
            int expectedId = 1;
            _mockDbService.Setup(db => db.AddSemester(name)).ReturnsAsync(expectedId);

            // Act
            var result = await _dashboard.AddSemester(name);

            // Assert
            result.Should().Be(expectedId);
            _mockDbService.Verify(db => db.AddSemester(name), Times.Once);
        }

        [Fact]
        public async Task AddCourse_ShouldCallAddCourseAndReturnId()
        {
            // Arrange
            string code = "Course Code";
            string name = "Course Name";
            int semesterId = 1;
            int[] departments = new int[] { 1, 2 };
            int expectedId = 1;
            _mockDbService.Setup(db => db.AddCourse(code, name, semesterId, departments)).ReturnsAsync(expectedId);

            // Act
            var result = await _dashboard.AddCourse(code, name, semesterId, departments);

            // Assert
            result.Should().Be(expectedId);
            _mockDbService.Verify(db => db.AddCourse(code, name, semesterId, departments), Times.Once);
        }

        [Fact]
        public void GetSemesterFromId_ShouldReturnCorrectSemesterName()
        {
            // Arrange
            var semestersTable = new DataTable();
            semestersTable.Columns.Add("id", typeof(int));
            semestersTable.Columns.Add("name", typeof(string));
            semestersTable.Rows.Add(1, "Semester 1");
            semestersTable.Rows.Add(2, "Semester 2");
            _dashboard.Semesters = semestersTable;

            // Act
            var result = _dashboard.getSemesterFromId(1);

            // Assert
            result.Should().Be("Semester 1");
        }

        [Fact]
        public void GetSemesterFromId_ShouldReturnDefaultStringIfNotFound()
        {
            // Arrange
            var semestersTable = new DataTable();
            semestersTable.Columns.Add("id", typeof(int));
            semestersTable.Columns.Add("name", typeof(string));
            semestersTable.Rows.Add(1, "Semester 1");
            semestersTable.Rows.Add(2, "Semester 2");
            _dashboard.Semesters = semestersTable;

            // Act
            var result = _dashboard.getSemesterFromId(3);

            // Assert
            result.Should().Be("...");
        }
    }
}
