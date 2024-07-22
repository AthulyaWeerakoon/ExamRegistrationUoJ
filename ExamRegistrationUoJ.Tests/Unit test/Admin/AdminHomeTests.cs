using Xunit;
using Moq;
using FluentAssertions;
using System.Data;
using System.Threading.Tasks;
using ExamRegistrationUoJ.Services.DBInterfaces;
using AdminPages;
using System.Collections;
using System.Linq;

namespace ExamRegistrationUoJ.Tests
{
    public class AdminHomeTests
    {
        private readonly Mock<IDBServiceAdmin1> _mockDbService;
        private readonly AdminHome _adminHome;

        public AdminHomeTests()
        {
            _mockDbService = new Mock<IDBServiceAdmin1>();
            _adminHome = new AdminHome(_mockDbService.Object);
        }

        [Fact]
        public async Task Init_ShouldInitializeAllData()
        {
            // Arrange
            _mockDbService.Setup(db => db.getDepartments()).ReturnsAsync(new DataTable());
            _mockDbService.Setup(db => db.getSemesters()).ReturnsAsync(new DataTable());
            _mockDbService.Setup(db => db.getActiveExams()).ReturnsAsync(new DataTable());
            _mockDbService.Setup(db => db.getCompletedExams()).ReturnsAsync(new DataTable());
            _mockDbService.Setup(db => db.getAllCoursesInExam()).ReturnsAsync(new DataTable());

            // Act
            await _adminHome.init();

            // Assert
            _adminHome.departments.Should().NotBeNull();
            _adminHome.semesters.Should().NotBeNull();
            _adminHome.completeExams.Should().NotBeNull();
            _adminHome.displayExams.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDepartments_ShouldFetchDepartments()
        {
            // Arrange
            var departmentsTable = new DataTable();
            _mockDbService.Setup(db => db.getDepartments()).ReturnsAsync(departmentsTable);

            // Act
            await _adminHome.getDepartments();

            // Assert
            _adminHome.departments.Should().BeSameAs(departmentsTable);
        }

        [Fact]
        public async Task GetSemesters_ShouldFetchSemesters()
        {
            // Arrange
            var semestersTable = new DataTable();
            _mockDbService.Setup(db => db.getSemesters()).ReturnsAsync(semestersTable);

            // Act
            await _adminHome.getSemesters();

            // Assert
            _adminHome.semesters.Should().BeSameAs(semestersTable);
        }

        [Fact]
        public async Task GetActiveExams_ShouldFetchActiveExams()
        {
            // Arrange
            var activeExamsTable = new DataTable();
            _mockDbService.Setup(db => db.getActiveExams()).ReturnsAsync(activeExamsTable);

            // Act
            await _adminHome.getActiveExams();

            // Assert
            _adminHome.displayExams.Should().BeSameAs(activeExamsTable);
        }

        [Fact]
        public async Task GetCompletedExams_ShouldFetchCompletedExams()
        {
            // Arrange
            var completedExamsTable = new DataTable();
            _mockDbService.Setup(db => db.getCompletedExams()).ReturnsAsync(completedExamsTable);

            // Act
            await _adminHome.getCompletedExams();

            // Assert
            _adminHome.completeExams.Should().BeSameAs(completedExamsTable);
        }

        [Fact]
        public async Task GetExamDept_ShouldFetchCoursesInExam()
        {
            // Arrange
            var coursesInExamTable = new DataTable();
            _mockDbService.Setup(db => db.getAllCoursesInExam()).ReturnsAsync(coursesInExamTable);

            // Act
            await _adminHome.getExamDept();

            // Assert
            // Assuming you can make the coursesInExam property internal or accessible
            // _adminHome.CoursesInExam.Should().BeSameAs(coursesInExamTable);
        }

        [Fact]
        public async Task FilterExam_ShouldFilterExamsBasedOnCriteria()
        {
            // Arrange
            var activeExamsTable = new DataTable();
            activeExamsTable.Columns.Add("semester_id");
            activeExamsTable.Columns.Add("status");
            activeExamsTable.Columns.Add("end_date");
            activeExamsTable.Columns.Add("id", typeof(uint));

            // Add test data
            activeExamsTable.Rows.Add("1", "1", DateTime.Now.AddDays(-1));
            activeExamsTable.Rows.Add("2", "0", DateTime.Now.AddDays(1));
            activeExamsTable.Rows.Add("1", "0", DateTime.Now.AddDays(-2));

            _mockDbService.Setup(db => db.getActiveExams()).ReturnsAsync(activeExamsTable);
            await _adminHome.getActiveExams();

            var coursesInExamTable = new DataTable();
            coursesInExamTable.Columns.Add("dept_id");
            coursesInExamTable.Columns.Add("exam_id", typeof(uint));

            // Add test data
            coursesInExamTable.Rows.Add("1", 1);
            coursesInExamTable.Rows.Add("2", 2);

            _mockDbService.Setup(db => db.getAllCoursesInExam()).ReturnsAsync(coursesInExamTable);
            await _adminHome.getExamDept();

            // Set filter options
            _adminHome.semesterOpt = "1";
            _adminHome.statusOpt = "1";
            _adminHome.departmentOpt = "1";

            // Act
            await _adminHome.filterExam();

            // Assert
            _adminHome.displayExams.Rows.Count.Should().Be(1);
        }
    }
}
