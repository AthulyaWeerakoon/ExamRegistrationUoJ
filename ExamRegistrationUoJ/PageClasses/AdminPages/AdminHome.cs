using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace AdminPages
{
    public class AdminHome
    {
        private IDBServiceAdmin1 db;
        public DataTable? departments { get; set; }
        public DataTable? semesters { get; set; }
        private DataTable? activeExams { get; set; }
        private DataTable? completeExams { get; set; }
        private DataTable? examDept {  get; set; }
        public DataTable? displayExams { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";

        public AdminHome(IDBServiceAdmin1 db) {
            this.db = db;
        }

        public async Task init() 
        {
            await Task.WhenAll(getDepartments(), getSemesters(), getActiveExams(), getExamDept());
        }

        public async Task getDepartments() {
            this.departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
        }

        public async Task getActiveExams()
        {
            this.activeExams = await db.getActiveExams();
            this.displayExams = this.activeExams.Copy();
        }

        public async Task getCompletedExams()
        {
            this.completeExams = await db.getCompletedExams();
        }

        public async Task getExamDept()
        { 
            // store departments and linking exams from course in exam table
            this.examDept = await db.getExamAndDept();
        }

        public async Task filterExam()
        {
            string filter = "";
            ArrayList filters = new ArrayList();
            DataView filteredExamOnce = new DataView(activeExams);

            // get filter options
            if (this.semesterOpt != "Semester" && this.semesterOpt != "All") filters.Add($"semester = {semesterOpt}");
            if (this.statusOpt != "Registration Status" && this.statusOpt != "All") filters.Add($"is_confirmed = {statusOpt}");

            // build filter
            for(int i = 0; i < filters.Count; i++)
            {
                filter += filters[i];
                if(i < filters.Count - 1) filter += " AND ";
            }

            // apply filter for semester and completion status
            filteredExamOnce.RowFilter = filter;

            if (this.departmentOpt != "Department" && this.departmentOpt != "All")
            {
                // if filtered by exam
                DataView coursesInExamView = new DataView(examDept);
                coursesInExamView.RowFilter = $"department_id = {departmentOpt}";
                DataTable filteredCourses = coursesInExamView.ToTable();
                DataTable filteredExamsTwice = filteredExamOnce.ToTable();
                var fullyFiltered = filteredExamsTwice.AsEnumerable().Where(row => filteredCourses.AsEnumerable().Any(course => course.Field<int>("exam_id") == row.Field<int>("id")));
                displayExams = fullyFiltered.CopyToDataTable();
            }
            else {
                // if not filtered by exam
                displayExams = filteredExamOnce.ToTable();
            }
        }
    }
}
