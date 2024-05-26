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
        private int examId;
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

        public async Task getExamTitle() 
        {
            this.examTitleInput = await db.getExamTitle(this.examId);
        }

        public async Task getSemester()
        {
            this.examTitleInput = (await db.getExamSemester(this.examId)).ToString();
        }

        public async Task getBatch()
        {
            this.batchInput = await db.getExamBatch(this.examId);
        }

        public async Task getEndDate()
        {
            this.SelectedDate = await db.getExamEndDate(this.examId);
        }

        public async Task getDeptsAndExams()
        {
            DataTable coursesInExam = await db.getCoursesInExam(this.examId);

        }
    }
}
