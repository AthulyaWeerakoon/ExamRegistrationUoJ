using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace AdvisorPages

{
    public class AdvViewExam
    {
        private IDBServiceAdvisorViewExam db;
        public DataTable? examTitle { get; set; }
        public DataTable? test { get; set; }
        public DataTable? students { get; set; }
        public AdvViewExam(IDBServiceAdvisorViewExam db)
        {
            this.db = db;
        }
        public async Task getExamTitle(uint exam_id)
        {
            this.examTitle = await db.getExamTitle(exam_id);
        }
        public async Task getStudentsInExam(uint exam_id)
        {
            this.students = await db.getStudentsInExam(exam_id);
        }
        /*public async Task test_a(uint exam_id)
        {
            this.test = await db.test_a(exam_id);
        }*/
        public async Task setAdvisorApproval(uint sie_id, uint exam_id, bool is_approved) 
        {
            await db.setAdvisorApproval(sie_id, exam_id, is_approved);
        }
    }
}
