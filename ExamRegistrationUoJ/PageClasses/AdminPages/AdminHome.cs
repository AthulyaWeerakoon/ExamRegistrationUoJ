using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace AdminPages
{
    public class AdminHome
    {
        private IDBServiceAdmin1 db;
        private AdminHomeProcessor adminHomeProcessor;
        public DataTable? departments { get; set; }
        public DataTable? semesters { get; set; }
        private DataTable? activeExams { get; set; }
        public DataTable? completeExams { get; set; }
        private DataTable? coursesInExam {  get; set; }
        public DataTable? displayExams { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";

        public AdminHome(IDBServiceAdmin1 db) {
            this.db = db;
            adminHomeProcessor = new AdminHomeProcessor();
        }

        public async Task init() 
        {
            await getSemesters();
            await getActiveExams();
            await getCompletedExams();
            await getExamDept();
            await getDepartments();
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
            this.coursesInExam = await db.getAllCoursesInExam();
        }

        public void filterExam()
        {
            this.displayExams = adminHomeProcessor.FilterExam(this.activeExams, this.coursesInExam, this.departmentOpt, this.semesterOpt, this.statusOpt);
        }
    }

    class AdminHomeProcessor
    {
        public DataTable FilterExam(DataTable? activeExams, DataTable? coursesInExam, string departmentOpt, string semesterOpt, string statusOpt)
        {
            string filter = "";
            ArrayList filters = new ArrayList();
            DataView filteredExamOnce = new DataView(activeExams);
            DataTable displayExams;

            // get filter options
            if (semesterOpt != "Semester" && semesterOpt != "All") filters.Add($"semester_id = {semesterOpt}");
            if (statusOpt != "Registration Status" && statusOpt != "All")
            {
                if (statusOpt == "-1") filters.Add($"status = 0");
                else
                {
                    filters.Add($"status = 1");
                    if (statusOpt == "0") filters.Add($"end_date > #{DateTime.Now.ToString("MM/dd/yyyy")}#");
                    else if (statusOpt == "1") filters.Add($"end_date <= #{DateTime.Now.ToString("MM/dd/yyyy")}#");
                }
            }

            // build filter
            for (int i = 0; i < filters.Count; i++)
            {
                filter += filters[i];
                if (i < filters.Count - 1) filter += " AND ";
            }

            // apply filter for semester and completion status
            filteredExamOnce.RowFilter = filter;

            try
            {
                if (departmentOpt != "Department" && departmentOpt != "All")
                {
                    // if filtered by exam
                    DataView coursesInExamView = new DataView(coursesInExam);
                    coursesInExamView.RowFilter = $"dept_id = {departmentOpt}";
                    DataTable filteredCourses = coursesInExamView.ToTable();
                    DataTable filteredExamsTwice = filteredExamOnce.ToTable();
                    var fullyFiltered = filteredExamsTwice.AsEnumerable().Where(row => filteredCourses.AsEnumerable().Any(course => Convert.ToUInt32(course["exam_id"]) == Convert.ToUInt32(row["id"])));
                    displayExams = fullyFiltered.CopyToDataTable();
                }
                else
                {
                    // if not filtered by exam
                    displayExams = filteredExamOnce.ToTable();
                }
            }
            catch (Exception ex)
            {
                displayExams = filteredExamOnce.Table.Clone();
            }

            return displayExams;
        }
    }
}
