using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBRegistrationFetchService
    {
        public Task<DataTable> getRegDescription(int exam_id, int student_id);
        public Task<DataTable> getRegCourses(int exam_id, int student_id);
        public Task<DataTable> getApprovedStudents(int exam_id);

        // for view payment receipt
        public Task<string> getPaymentReceiptUrl(int exam_id, int student_id);
        //
    }
}
