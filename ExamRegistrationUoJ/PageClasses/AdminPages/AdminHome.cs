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
        private DataTable? allExams { get; set; }
        private DataTable? examDept {  get; set; }
        public DataView? exams { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";

        AdminHome(IDBServiceAdmin1 db) {
            this.db = db;
        }

        public async Task getDepartments() {
            this.departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
        }

        public async Task getExams()
        {
            this.allExams = await db.getExams();
            this.exams = new DataView(allExams);
        }

        public async Task filterExam()
        {
            string filter = "";
            ArrayList filters = new ArrayList();

            // get filter options
            if (this.semesterOpt != "Semester") filters.Add($"semester = {semesterOpt}");
            if (this.departmentOpt != "Department") filters.Add($"deparment = {departmentOpt}");
            if (this.statusOpt != "Registration Status") filters.Add($"is_confirmed = {statusOpt}");

            // build filter
            for(int i = 0; i < filters.Count; i++)
            {
                filter += filters[i];
                if(i < filters.Count - 1) filter += " AND ";
            }

            // apply filter
            this.exams.RowFilter = filter;
        }
    }
}
