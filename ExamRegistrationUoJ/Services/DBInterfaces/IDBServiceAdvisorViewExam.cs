using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceAdvisorViewExam
    {
        public Task<DataTable> getStudent(uint studentId);
        public Task<DataTable> getExamTitle(uint examId);
    }
}
