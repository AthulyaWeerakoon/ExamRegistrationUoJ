using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Data;
using System.Collections;

namespace StudentPages
{
    public class StudentHome
    {
        private IDBServiceStudentHome db;

        // Properties to hold data for departments, semesters, and exams
        public DataTable? Departments { get; private set; }
        public DataTable? Semesters { get; private set; }
        private DataTable? AllExams { get; set; }
        public DataView? Exams { get; private set; }

        // Properties to hold selected filter options
        public string DepartmentOpt { get; set; } = "Department";
        public string SemesterOpt { get; set; } = "Semester";
        public string StatusOpt { get; set; } = "Registration Status";

        // Constructor to initialize the database service
        public StudentHome(IDBServiceStudentHome db)
        {
            this.db = db;
        }

        // Method to retrieve departments from the database
        public async Task GetDepartments()
        {
            this.Departments = await db.GetDepartments();
        }

        // Method to retrieve semesters from the database
        public async Task GetSemesters()
        {
            this.Semesters = await db.GetSemesters();
        }

        // Method to retrieve all exams from the database
        public async Task GetExams()
        {
            this.AllExams = await db.GetExams();
            this.Exams = new DataView(AllExams);
        }

        // Method to filter exams based on selected filter options
        public void FilterExams()
        {
            var filters = new List<string>();

            // Add filters based on selected options
            if (this.SemesterOpt != "Semester") filters.Add($"semester = '{SemesterOpt}'");
            if (this.DepartmentOpt != "Department") filters.Add($"department = '{DepartmentOpt}'");
            if (this.StatusOpt != "Registration Status") filters.Add($"is_confirmed = '{StatusOpt}'");

            // Build filter string
            string filter = string.Join(" AND ", filters);

            // build filter
            for (int i = 0; i < filters.Count; i++)
            {
                filter += filters[i];
                if (i < filters.Count - 1) filter += " AND ";
            }

            // apply filter
            this.Exams.RowFilter = filter;

        }
    }
}
