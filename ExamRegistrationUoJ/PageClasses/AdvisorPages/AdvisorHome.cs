using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdvisorPages
{
    public class AdvisorHome
    {
        private readonly IDBServiceAdvisorHome db;

        public DataTable? Departments { get; private set; }
        public DataTable? Semesters { get; private set; }
        public DataView? Exams { get; private set; }

        public int DepartmentOpt { get; set; } = -1;
        public int SemesterOpt { get; set; } = -1;
        public string StatusOpt { get; set; } = "Registration Status";

        private Dictionary<int, DataTable> examCourses = new();
        private HashSet<int> openDropdowns = new();

        public AdvisorHome(IDBServiceAdvisorHome db)
        {
            this.db = db;
        }

        public async Task getDepartments()
        {
            this.Departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.Semesters = await db.getSemesters();
        }

        public async Task getExams()
        {
            int departmentId = DepartmentOpt != -1 && DepartmentOpt != 0 ? DepartmentOpt : -1;
            int semesterId = SemesterOpt != -1 && SemesterOpt != 0 ? SemesterOpt : -1;
            var examsTable = await db.getExams(departmentId, semesterId);
            this.Exams = new DataView(examsTable);
        }

        public void filterExams()
        {
            if (this.Exams != null)
            {
                var filters = new List<string>();

                // Add filters based on selected options
                if (this.SemesterOpt != -1 && this.SemesterOpt != 0) filters.Add($"semester_id = {SemesterOpt}");
                if (this.DepartmentOpt != -1 && this.DepartmentOpt != 0) filters.Add($"department_id = {DepartmentOpt}");

                // Build filter string
                string filter = string.Join(" AND ", filters);

                // Apply filter
                this.Exams.RowFilter = filter;
            }
        }

        public async Task LoadCoursesForExam(int examId)
        {
            if (!examCourses.ContainsKey(examId))
            {
                var courses = await db.getCoursesForExam(examId);
                examCourses[examId] = courses;
            }
        }

        public DataTable GetCoursesForExam(int examId)
        {
            return examCourses.ContainsKey(examId) ? examCourses[examId] : new DataTable();
        }

        public bool IsDropdownOpen(int examId)
        {
            return openDropdowns.Contains(examId);
        }

        public void ToggleDropdown(int examId)
        {
            if (IsDropdownOpen(examId))
            {
                openDropdowns.Remove(examId);
            }
            else
            {
                openDropdowns.Add(examId);
            }
        }
    }
}
