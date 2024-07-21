using System.Data;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AdvisorPages; // Ensure this matches the namespace of your AdvViewExam class
using ExamRegistrationUoJ.Services.DBInterfaces;

namespace ExamRegistrationUoJ.Tests.PageClasses.AdminPages
{
    public class AdvViewExamTests
    {
        private Mock<IDBServiceAdvisorViewExam> CreateDbMock()
        {
            var dbMock = new Mock<IDBServiceAdvisorViewExam>();

            // Setup mock methods as needed

            // Mock getExamTitle
            var examTitleTable = new DataTable();
            examTitleTable.Columns.Add("exam_id");
            examTitleTable.Columns.Add("exam_name");
            var examTitleRow = examTitleTable.NewRow();
            examTitleRow["exam_id"] = 1;
            examTitleRow["exam_name"] = "Final Exam";
            examTitleTable.Rows.Add(examTitleRow);

            dbMock.Setup(db => db.getExamTitle(It.IsAny<uint>()))
                .ReturnsAsync(examTitleTable);

            // Mock getStudentsInExam
            var studentsTable = new DataTable();
            studentsTable.Columns.Add("student_id");
            studentsTable.Columns.Add("student_name");
            var studentsRow = studentsTable.NewRow();
            studentsRow["student_id"] = 1;
            studentsRow["student_name"] = "John Doe";
            studentsTable.Rows.Add(studentsRow);

            dbMock.Setup(db => db.getStudentsInExam(It.IsAny<uint>()))
                .ReturnsAsync(studentsTable);

            // Mock setAdvisorApproval
            dbMock.Setup(db => db.setAdvisorApproval(It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            return dbMock;
        }

        [Fact]
        public async Task GetExamTitle_ShouldSetExamTitleProperty()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advViewExam = new AdvViewExam(dbMock.Object);

            // Act
            await advViewExam.getExamTitle(1);

            // Assert
            Assert.NotNull(advViewExam.examTitle);
            Assert.Equal(1, advViewExam.examTitle.Rows.Count);
            Assert.Equal("Final Exam", advViewExam.examTitle.Rows[0]["exam_name"]);
        }

        [Fact]
        public async Task GetStudentsInExam_ShouldSetStudentsProperty()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advViewExam = new AdvViewExam(dbMock.Object);

            // Act
            await advViewExam.getStudentsInExam(1);

            // Assert
            Assert.NotNull(advViewExam.students);
            Assert.Equal(1, advViewExam.students.Rows.Count);
            Assert.Equal("John Doe", advViewExam.students.Rows[0]["student_name"]);
        }

        [Fact]
        public async Task SetAdvisorApproval_ShouldCallDbMethod()
        {
            // Arrange
            var dbMock = CreateDbMock();
            var advViewExam = new AdvViewExam(dbMock.Object);

            // Act
            await advViewExam.setAdvisorApproval(1, 1, true);

            // Assert
            dbMock.Verify(db => db.setAdvisorApproval(1, 1, true), Times.Once);
        }

       
    }
}
