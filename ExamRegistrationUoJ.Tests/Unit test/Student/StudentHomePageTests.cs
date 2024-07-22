using StudentPages;
using ExamRegistrationUoJ.Services.DBInterfaces;
using Moq;
using System.Data;
using Xunit;

namespace StudentPages.Tests
{
    public class StudentHomePageTests
    {
        private readonly Mock<IDBServiceStudentHome> _dbServiceMock;
        private readonly StudentHomePage _studentHomePage;
        private readonly int _studentId = 123;

        public StudentHomePageTests()
        {
            _dbServiceMock = new Mock<IDBServiceStudentHome>();
            _studentHomePage = new StudentHomePage(_dbServiceMock.Object, _studentId);
        }

        [Fact]
        public async Task Init_ShouldInitializePageWithData()
        {
            // Arrange
            var departments = new DataTable();
            var semesters = new DataTable();
            var registeredExams = new DataTable();

            _dbServiceMock.Setup(db => db.getDepartments()).ReturnsAsync(departments);
            _dbServiceMock.Setup(db => db.getSemesters()).ReturnsAsync(semesters);
            _dbServiceMock.Setup(db => db.getRegisteredExams(_studentId)).ReturnsAsync(registeredExams);

            // Act
            await _studentHomePage.Init();

            // Assert
            Assert.Equal(departments, _studentHomePage.departments);
            Assert.Equal(semesters, _studentHomePage.semesters);
            Assert.Equal(registeredExams, _studentHomePage.registeredExams);
        }

        [Fact]
        public async Task FilterExam_ShouldSetDisplayExams()
        {
            // Arrange
            var filteredExams = new DataTable();
            _studentHomePage.department = "1";
            _studentHomePage.semester = "1";
            _studentHomePage.status = "1";

            _dbServiceMock.Setup(db => db.getFilteredExams(1, 1, 1)).ReturnsAsync(filteredExams);

            // Act
            await _studentHomePage.filterExam();

            // Assert
            Assert.Equal(filteredExams, _studentHomePage.displayExams);
        }

        [Fact]
        public async Task RegisterForExam_ShouldReturnTrue_WhenSuccessful()
        {
            // Arrange
            int examId = 1;
            _dbServiceMock.Setup(db => db.registerForExam(_studentId, examId)).ReturnsAsync(true);

            // Act
            bool result = await _studentHomePage.registerForExam(examId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LoadRegisteredExams_ShouldSetRegisteredExamsAndCourses()
        {
            // Arrange
            var registeredExams = new DataTable();
            registeredExams.Columns.Add("id", typeof(int));
            registeredExams.Rows.Add(1);

            var courses = new DataTable();

            _dbServiceMock.Setup(db => db.getRegisteredExams(_studentId)).ReturnsAsync(registeredExams);
            _dbServiceMock.Setup(db => db.getCoursesForExam(1)).ReturnsAsync(courses);

            // Act
            await _studentHomePage.LoadRegisteredExams();

            // Assert
            Assert.Equal(registeredExams, _studentHomePage.registeredExams);
            Assert.Equal(courses, _studentHomePage.registeredExamCourses[1]);
        }

        [Fact]
        public async Task GetCoursesForExam_ShouldSetExamCourses()
        {
            // Arrange
            int examId = 1;
            var courses = new DataTable();

            _dbServiceMock.Setup(db => db.getCoursesForExam(examId)).ReturnsAsync(courses);

            // Act
            await _studentHomePage.getCoursesForExam(examId);

            // Assert
            Assert.Equal(courses, _studentHomePage.examCourses[examId]);
        }
    }
}
