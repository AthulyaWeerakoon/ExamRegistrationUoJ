using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AdvisorPages; // Ensure this matches the namespace of your AdvisorHome class
using ExamRegistrationUoJ.Services.DBInterfaces;

namespace ExamRegistrationUoJ.Tests.PageClasses.AdminPages
{
    public class AdvisorHomeTests
    {
        private Mock<IDBServiceAdvisorHome> CreateDbMock()
        {
            var dbMock = new Mock<IDBServiceAdvisorHome>();

            // Setup mock methods as needed
            var departmentsTable = new DataTable();
            departmentsTable.Columns.Add("department_id");
            departmentsTable.Columns.Add("department_name");
            var departmentsRow = departmentsTable.NewRow();
            departmentsRow["department_id"] = 1;
            departmentsRow["department_name"] = "Computer Science";
            departmentsTable.Rows.Add(departmentsRow);

            dbMock.Setup(db => db.getDepartments())
                .ReturnsAsync(departmentsTable);

            var semestersTable = new DataTable();
            semestersTable.Columns.Add("semester_id");
            semestersTable.Columns.Add("semester_name");
            var semestersRow = semestersTable.NewRow();
            semestersRow["semester_id"] = 1;
            semestersRow["semester_name"] = "Semester 01";
            semestersTable.Rows.Add(semestersRow);

            dbMock.Setup(db => db.getSemesters())
                .ReturnsAsync(semestersTable);

            var examsTable = new DataTable();
            examsTable.Columns.Add("exam_id");
            examsTable.Columns.Add("exam_name");
            var examsRow = examsTable.NewRow();
            examsRow["exam_id"] = 1;
            examsRow["exam_name"] = "Midterm Exam";
            examsTable.Rows.Add(examsRow);

            dbMock.Setup(db => db.getExams(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(examsTable);

            var coursesTable = new DataTable();
            coursesTable.Columns.Add("course_id");
            coursesTable.Columns.Add("course_name");
            var coursesRow = coursesTable.NewRow();
            coursesRow["course_id"] = 1;
            coursesRow["course_name"] = "Data Structures";
            coursesTable.Rows.Add(coursesRow);

            dbMock.Setup(db => db.getCoursesForExam(It.IsAny<int>()))
                .ReturnsAsync(coursesTable);

            return dbMock;
        }

        [Fact]
        public async Task GetDepartments_ShouldSetDepartmentsProperty()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advisorHome = new AdvisorHome(dbMock.Object);

            // Act
            await advisorHome.getDepartments();

            // Assert
            Assert.NotNull(advisorHome.Departments);
            Assert.Equal(1, advisorHome.Departments.Rows.Count);
            Assert.Equal("Computer Science", advisorHome.Departments.Rows[0]["department_name"]);
        }

        [Fact]
        public async Task GetSemesters_ShouldSetSemestersProperty()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advisorHome = new AdvisorHome(dbMock.Object);

            // Act
            await advisorHome.getSemesters();

            // Assert
            Assert.NotNull(advisorHome.Semesters);
            Assert.Equal(1, advisorHome.Semesters.Rows.Count);
            Assert.Equal("Semester 01", advisorHome.Semesters.Rows[0]["semester_name"]);
        }

        [Fact]
        public async Task GetExams_ShouldSetExamsPropertyWithCorrectFilter()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advisorHome = new AdvisorHome(dbMock.Object);
            advisorHome.DepartmentOpt = 1;
            advisorHome.SemesterOpt = 1;

            // Act
            await advisorHome.getExams();

            // Assert
            Assert.NotNull(advisorHome.Exams);
            Assert.Equal(1, advisorHome.Exams.Table.Rows.Count);
            Assert.Equal("Midterm Exam", advisorHome.Exams.Table.Rows[0]["exam_name"]);
        }

        [Fact]
        public void FilterExams_ShouldApplyFiltersCorrectly()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advisorHome = new AdvisorHome(dbMock.Object);
            var examsTable = new DataTable();
            examsTable.Columns.Add("exam_id");
            examsTable.Columns.Add("exam_name");
            var examsRow = examsTable.NewRow();
            examsRow["exam_id"] = 1;
            examsRow["exam_name"] = "Midterm Exam";
            examsTable.Rows.Add(examsRow);
            var examsView = new DataView(examsTable);
            var field = typeof(AdvisorHome).GetField("Exams", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(advisorHome, examsView);

            advisorHome.SemesterOpt = 1;
            advisorHome.DepartmentOpt = 1;

            // Act
            advisorHome.filterExams();

            // Assert
            Assert.NotNull(advisorHome.Exams);
            Assert.Equal("semester_id = 1 AND department_id = 1", advisorHome.Exams.RowFilter);
        }

        [Fact]
        public async Task LoadCoursesForExam_ShouldLoadCoursesForExam()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advisorHome = new AdvisorHome(dbMock.Object);

            // Act
            await advisorHome.LoadCoursesForExam(1);

            // Assert
            Assert.NotNull(advisorHome.GetCoursesForExam(1));
            Assert.Equal(1, advisorHome.GetCoursesForExam(1).Rows.Count);
            Assert.Equal("Data Structures", advisorHome.GetCoursesForExam(1).Rows[0]["course_name"]);
        }

        [Fact]
        public void ToggleDropdown_ShouldUpdateOpenDropdowns()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advisorHome = new AdvisorHome(dbMock.Object);

            // Act
            advisorHome.ToggleDropdown(1);

            // Assert
            Assert.True(advisorHome.IsDropdownOpen(1));

            // Act
            advisorHome.ToggleDropdown(1);

            // Assert
            Assert.False(advisorHome.IsDropdownOpen(1));
        }
    }
}
