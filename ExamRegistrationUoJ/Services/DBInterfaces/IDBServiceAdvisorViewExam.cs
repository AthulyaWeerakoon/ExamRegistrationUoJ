using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceAdvisorViewExam
    {
        public Task<DataTable> getStudentsInExam(uint examId);
        public Task<DataTable> getExamTitle(uint examId);
        //public Task<DataTable> test_a(uint examid);
        public Task setAdvisorApproval(uint sieId, uint examId, bool isApproved);
    }
}
