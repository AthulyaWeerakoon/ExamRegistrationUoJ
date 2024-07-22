using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceAdvisor1
    {
        public Task<String> GetStudentEmail(int id);
        /*
         Return structure for get student's register number
         Name       Description     Type
         id         student_id      string
         */
        
        public Task<String> getStudentName(int id);
        Task<DataTable> GetReAttemptDetails(int acc_id,int exam_id);
        Task<String> GetExamDetails(int exam_id);

        Task AdvisorApproval(int acc_id, int exam_id);
        Task AdvisorRejection(int acc_id, int exam_id);
        //public Task<String> getCourseCode(int id);
        //public Task<String> getCourseName(int id);
        //public Task<String> getCoApprovalStatus(int id);
        //public Task<String> getSubjectCoordinator(int coordinatorID;

    }
}

