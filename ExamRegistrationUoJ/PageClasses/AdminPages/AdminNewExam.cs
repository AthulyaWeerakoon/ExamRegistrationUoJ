using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace AdminPages
{
    struct NewExamForm {
        public string? examTitle;
        public int? semester;
        public string? batch;
        public DateTime? endDate;
        public List<string>? depts;
        public List<DataTable> coursesFromDepts;
    }
    public class AdminNewExam
    {
        private IDBServiceAdmin1 db;
        private int? examId;
        public string? examTitleInput { get; set; }
        public string? semesterOpt { get; set; }
        public string? batchInput { get; set; }
        public DateTime? SelectedDate { get; set; }
        public List<string>? deptOpts {  get; set; }
        public List<DataTable>? coursesFromDepts { get; set; }

        public AdminNewExam(IDBServiceAdmin1 db, int examId) {
            this.db = db;
            this.examId = examId;
        }
        public AdminNewExam(IDBServiceAdmin1 db)
        {
            this.db = db;
            this.examId = null;
        }

        public async Task getExamDescription()
        {
            if (this.examId != null) 
            {
                DataTable? examDescription = await db.getExamDescription((int)this.examId);
                if (examDescription != null) {
                    examTitleInput = Convert.ToString(examDescription.Rows[0]["name"]);
                    examTitleInput = Convert.ToString(examDescription.Rows[0]["name"]);
                    examTitleInput = Convert.ToString(examDescription.Rows[0]["name"]);
                    examTitleInput = Convert.ToString(examDescription.Rows[0]["name"]);
                }
            }
        }

        public async Task getDeptsAndExams()
        {
            if (this.examId != null)
            {
                DataTable? coursesInExam = await db.getCoursesInExam((int)this.examId);
                if (coursesInExam != null)
                {
                    
                }
            }

        }
    }
}
