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
        public async Task getDepartments()
        {
            this.Departments = await db.getDepartments();
        }

        // Method to retrieve semesters from the database
        public async Task getSemesters()
        {
            this.Semesters = await db.getSemesters();
        }

        // Method to retrieve all exams from the database
        public async Task getExams()
        {
            this.AllExams = await db.getExams();
            this.Exams = new DataView(AllExams);
        }

        // Method to filter exams based on selected filter options
        public void filterExams()
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
        public async Task<bool> registerForExam(int studentId, int examId)
        {
            return await db.registerForExam(studentId, examId);
        }
    }
}