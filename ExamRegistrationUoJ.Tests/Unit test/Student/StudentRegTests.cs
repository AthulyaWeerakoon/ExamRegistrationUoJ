using StudentPages;
using ExamRegistrationUoJ.Services.DBInterfaces;
using Moq;
using System.Data;
using Xunit;

namespace StudentPages.Tests
{
    public class StudentRegTests
    {
        private readonly Mock<IDBServiceSR> _dbServiceMock;
        private readonly StudentReg _studentReg;

        public StudentRegTests()
        {
            _dbServiceMock = new Mock<IDBServiceSR>();
            _studentReg = new StudentReg(_dbServiceMock.Object);
        }

        [Fact]
        public async Task Init_ShouldInitializePageWithData()
        {
            // Arrange
            var departments = new DataTable();
            var semesters = new DataTable();
            var advisors = new DataTable();

            _dbServiceMock.Setup(db => db.getDepartments()).ReturnsAsync(departments);
            _dbServiceMock.Setup(db => db.getSemesters()).ReturnsAsync(semesters);
            _dbServiceMock.Setup(db => db.getAdvisors()).ReturnsAsync(advisors);

            // Act
            await _studentReg.init();

            // Assert
            Assert.Equal(departments, _studentReg.departments);
            Assert.Equal(semesters, _studentReg.semesters);
            Assert.Equal(advisors, _studentReg.advisors);
        }

        [Fact]
        public async Task GetExamTitle_ShouldSetExamTitle()
        {
            // Arrange
            var examTitle = new DataTable();
            uint examId = 1;

            _dbServiceMock.Setup(db => db.getExamTitle(examId)).ReturnsAsync(examTitle);

            // Act
            await _studentReg.getExamTitle(examId);

            // Assert
            Assert.Equal(examTitle, _studentReg.examTitle);
        }

        [Fact]
        public async Task GetDepartments_ShouldSetDepartments()
        {
            // Arrange
            var departments = new DataTable();

            _dbServiceMock.Setup(db => db.getDepartments()).ReturnsAsync(departments);

            // Act
            await _studentReg.getDepartments();

            // Assert
            Assert.Equal(departments, _studentReg.departments);
        }

        [Fact]
        public async Task GetSemesters_ShouldSetSemesters()
        {
            // Arrange
            var semesters = new DataTable();

            _dbServiceMock.Setup(db => db.getSemesters()).ReturnsAsync(semesters);

            // Act
            await _studentReg.getSemesters();

            // Assert
            Assert.Equal(semesters, _studentReg.semesters);
        }

        [Fact]
        public async Task GetCourses_ShouldSetCourses()
        {
            // Arrange
            var courses = new DataTable();
            uint examId = 1;
            uint depId = 1;

            _dbServiceMock.Setup(db => db.getCourses(examId, depId)).ReturnsAsync(courses);

            // Act
            await _studentReg.getCourses(examId, depId);

            // Assert
            Assert.Equal(courses, _studentReg.courses);
        }

        [Fact]
        public async Task GetStudent_ShouldSetStudents()
        {
            // Arrange
            var students = new DataTable();
            uint studentId = 1;

            _dbServiceMock.Setup(db => db.getStudent(studentId)).ReturnsAsync(students);

            // Act
            await _studentReg.getStudent(studentId);

            // Assert
            Assert.Equal(students, _studentReg.students);
        }

        [Fact]
        public async Task GetAdvisorId_ShouldSetAdvisorId()
        {
            // Arrange
            string msEmail = "advisor@example.com";
            uint advisorId = 1;

            _dbServiceMock.Setup(db => db.getAdvisorId(msEmail)).ReturnsAsync(advisorId);

            // Act
            await _studentReg.getAdvisorId(msEmail);

            // Assert
            Assert.Equal(advisorId, _studentReg.advisorId);
        }

        [Fact]
        public async Task GetAdvisors_ShouldSetAdvisors()
        {
            // Arrange
            var advisors = new DataTable();

            _dbServiceMock.Setup(db => db.getAdvisors()).ReturnsAsync(advisors);

            // Act
            await _studentReg.getAdvisors();

            // Assert
            Assert.Equal(advisors, _studentReg.advisors);
        }

        [Fact]
        public async Task GetInitialRegisteredCourses_ShouldSetInitialRegisteredCourses()
        {
            // Arrange
            var initialRegisteredCourses = new DataTable();
            uint examStudentId = 1;

            _dbServiceMock.Setup(db => db.getInitialRegisteredCourses(examStudentId)).ReturnsAsync(initialRegisteredCourses);

            // Act
            await _studentReg.getInitialRegisteredCourses(examStudentId);

            // Assert
            Assert.Equal(initialRegisteredCourses, _studentReg.initialRegisteredCourses);
        }

        [Fact]
        public async Task SetStudentInExam_ShouldReturnInt()
        {
            // Arrange
            uint studentId = 1;
            uint examId = 1;
            uint isProper = 1;
            uint advisorId = 1;
            int expected = 1;

            _dbServiceMock.Setup(db => db.setStudentInExams(studentId, examId, isProper, advisorId)).ReturnsAsync(expected);

            // Act
            int result = await _studentReg.setStudentInExam(studentId, examId, isProper, advisorId);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task SetStudentRegistration_ShouldReturnInt()
        {
            // Arrange
            uint examStudentId = 1;
            uint examCourseId = 1;
            string addOrDrop = "Add";
            int expected = 1;

            _dbServiceMock.Setup(db => db.setStudentRegistration(examStudentId, examCourseId, addOrDrop)).ReturnsAsync(expected);

            // Act
            int result = await _studentReg.setStudentRegistration(examStudentId, examCourseId, addOrDrop);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task SetPayments_ShouldReturnInt()
        {
            // Arrange
            uint studentId = 1;
            uint examId = 1;
            byte[] paymentReceipt = new byte[] { 1, 2, 3 };
            string contentType = "image/png";
            int expected = 1;

            _dbServiceMock.Setup(db => db.setPayments(studentId, examId, paymentReceipt, contentType)).ReturnsAsync(expected);

            // Act
            int result = await _studentReg.setPayments(studentId, examId, paymentReceipt, contentType);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task IsAdded_ShouldReturnInt()
        {
            // Arrange
            uint examStudentId = 1;
            uint examCourseId = 1;
            int expected = 1;

            _dbServiceMock.Setup(db => db.isAdded(examStudentId, examCourseId)).ReturnsAsync(expected);

            // Act
            int result = await _studentReg.isAdded(examStudentId, examCourseId);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetStudentIdByEmail_ShouldReturnInt()
        {
            // Arrange
            string email = "student@example.com";
            int? expected = 1;

            _dbServiceMock.Setup(db => db.getStudentIdByEmail(email)).ReturnsAsync(expected);

            // Act
            int? result = await _studentReg.getStudentIdByEmail(email);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Test_ShouldSetTestDt()
        {
            // Arrange
            var testDt = new DataTable();

            _dbServiceMock.Setup(db => db.test_a()).ReturnsAsync(testDt);

            // Act
            await _studentReg.test();

            // Assert
            Assert.Equal(testDt, _studentReg.test_dt);
        }
    }
}
