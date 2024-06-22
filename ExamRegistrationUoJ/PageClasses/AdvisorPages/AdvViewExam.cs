using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace AdvisorPages

{
    public class AdvViewExam
    {
        private IDBServiceAdvisorViewExam db;
        public DataTable? examTitle { get; set; }
        public DataTable? students { get; set; }
        public AdvViewExam(IDBServiceAdvisorViewExam db)
        {
            this.db = db;
        }
        public async Task getExamTitle(uint exam_id)
        {
            this.examTitle = await db.getExamTitle(exam_id);
        }
        public async Task getStudent(uint student_id)
        {
            this.students = await db.getStudent(student_id);
        }
    }
}
