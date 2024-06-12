using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace CoordinatorPages
{
    public class CoHome
    {
        private IDBServiceCoordinator1 db;
        public DataTable? departments { get; set; }
        public DataTable? semesters { get; set; }

        public DataTable? Departments { get; set; }
        public DataTable? Semesters { get; set; }
        /*private DataTable? allExams { get; set; }
        private DataTable? examDept { get; set; }*/
        public DataView? exams { get; set; }
        public DataTable? displayExams { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";
        

        public CoHome(IDBServiceCoordinator1 db)
        {
            this.db = db;
        }
        public async Task init()
        {
            await getSemesters();
            await getDepartments();
        }

        public async Task getDepartments()
        {
            this.departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
        }

        /*public async Task getExams()
        {
            this.allExams = await db.getExams();
            this.exams = new DataView(allExams);
        }*/

        public async Task filterExam()
        {
            ArrayList filters = new ArrayList();

            // Get filter options
            if (this.semesterOpt != "Semester") filters.Add($"semester = '{semesterOpt}'");
            if (this.departmentOpt != "Department") filters.Add($"department = '{departmentOpt}'");

            /*// Build filter
            for (int i = 0; i < filters.Count; i++)
            {
                filter += " AND " + filters[i];
            }

            // Apply filter
            if (this.exams != null)
            {
                this.exams.RowFilter = filter;
            }*/
        }

    }
}
