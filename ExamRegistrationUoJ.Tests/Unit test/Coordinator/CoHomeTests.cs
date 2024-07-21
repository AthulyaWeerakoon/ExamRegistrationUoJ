using CoordinatorPages;
using ExamRegistrationUoJ.Services.DBInterfaces;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace CoordinatorPages.Tests
{
    public class CoHomeTests
    {
        private readonly Mock<IDBServiceCoordinator1> _dbServiceMock;
        private readonly CoHome _coHome;

        public CoHomeTests()
        {
            _dbServiceMock = new Mock<IDBServiceCoordinator1>();
            _coHome = new CoHome(_dbServiceMock.Object);
        }

        [Fact]
        public async Task Init_ShouldCallGetSemestersAndGetDepartments()
        {
            _dbServiceMock.Setup(db => db.getSemesters()).ReturnsAsync(new DataTable());
            _dbServiceMock.Setup(db => db.getDepartments()).ReturnsAsync(new DataTable());

            await _coHome.init();

            _dbServiceMock.Verify(db => db.getSemesters(), Times.Once);
            _dbServiceMock.Verify(db => db.getDepartments(), Times.Once);
        }

        [Fact]
        public async Task GetDepartments_ShouldSetDepartments()
        {
            var dataTable = new DataTable();
            _dbServiceMock.Setup(db => db.getDepartments()).ReturnsAsync(dataTable);

            await _coHome.getDepartments();

            Assert.Equal(dataTable, _coHome.departments);
        }

        [Fact]
        public async Task GetSemesters_ShouldSetSemesters()
        {
            var dataTable = new DataTable();
            _dbServiceMock.Setup(db => db.getSemesters()).ReturnsAsync(dataTable);

            await _coHome.getSemesters();

            Assert.Equal(dataTable, _coHome.semesters);
        }

        [Fact]
        public async Task GetCoordinatorID_ShouldReturnCoordinatorID()
        {
            var email = "test@example.com";
            var coordinatorID = 123;
            _dbServiceMock.Setup(db => db.getCoordinatorID(email)).ReturnsAsync(coordinatorID);

            var result = await _coHome.getCoordinatorID(email);

            Assert.Equal(coordinatorID, result);
        }

        [Fact]
        public async Task GetExamIdDetailsCoordinator_ShouldSetNullDataCount()
        {
            var email = "test@example.com";
            var dataTable = new DataTable();
            _dbServiceMock.Setup(db => db.getexam_id_details_coordinator(email)).ReturnsAsync(dataTable);

            await _coHome.getexam_id_details_coordinator(email);

            Assert.Equal(dataTable, _coHome.nullData_count);
        }

        [Fact]
        public async Task GetExamAllDetailsCoordinator_ShouldSetExamsAllDetails()
        {
            var email = "test@example.com";
            var dataTable = new DataTable();
            _dbServiceMock.Setup(db => db.get_exam_all_details_coordinator(email)).ReturnsAsync(dataTable);

            await _coHome.get_exam_all_details_coordinator(email);

            Assert.Equal(dataTable, _coHome.exams_all_details);
        }

        [Fact]
        public async Task FilterExam_ShouldFilterBasedOnSemesterAndDepartment()
        {
            var email = "test@example.com";
            var dataTable = new DataTable();
            dataTable.Columns.Add("semester_id");
            dataTable.Columns.Add("department_id");
            dataTable.Rows.Add("1", "1");
            dataTable.Rows.Add("2", "2");

            _dbServiceMock.Setup(db => db.get_exam_all_details_coordinator(email)).ReturnsAsync(dataTable);

            _coHome.semesterOpt = "1";
            _coHome.departmentOpt = "1";

            var result = await _coHome.filter_exam(email);

            Assert.Single(result.Rows);
            Assert.Equal("1", result.Rows[0]["semester_id"]);
            Assert.Equal("1", result.Rows[0]["department_id"]);
        }

        [Fact]
        public async Task FilterExamSemseter_ShouldReturnSemesterOpt()
        {
            _coHome.semesterOpt = "1";

            var result = await _coHome.filter_exam_semseter();

            Assert.Equal("1", result);
        }

        [Fact]
        public async Task FilterExamSemseter_ShouldReturnAllWhenSemesterOptIsNotSet()
        {
            _coHome.semesterOpt = "Semester";

            var result = await _coHome.filter_exam_semseter();

            Assert.Equal("all", result);
        }
    }
}
