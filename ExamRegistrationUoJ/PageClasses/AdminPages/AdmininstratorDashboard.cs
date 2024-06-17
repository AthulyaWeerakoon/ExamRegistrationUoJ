using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Data;

namespace ExamRegistrationUoJ.PageClasses.AdminPages
{
    public class AdmininstratorDashboard
    {
        private IDBServiceAdminDashboard db;
        public DataTable Semesters { get; set; }
        public DataTable Departements { get; set; }
        public DataTable Advisors { get; set; }
        public DataTable Courses { get; set; }

        public AdmininstratorDashboard(IDBServiceAdminDashboard db)
        {
            this.db = db;
        }

        public async Task init()
        {
            await getSemesters();
            await getDepartments();
            await getAdvisors();
            await getCourses();
        }

        private async Task getSemesters()
        {
            this.Semesters = await db.getSemesters();
        }

        private async Task getDepartments()
        {
            this.Departements = await db.getDepartments();
        }

        private async Task getAdvisors()
        {
            this.Advisors = await db.getAdvisors();
        }

        private async Task getCourses()
        {
            this.Courses = await db.getAllCourses();
        }

        public async Task UpdateAdvisor(int advisorId, string newName, string newEmail)
        {
            await db.UpdateAdvisor(advisorId, newName, newEmail);
        }

        public async Task UpdateDepartmentName(int departmentId, string newName)
        {
            await db.UpdateDepartmentName(departmentId, newName);
        }

        public async Task UpdateSemesterName(int semesterId, string newName)
        {
            await db.UpdateSemesterName(semesterId, newName);
        }

        public async Task UpdateCourse(int courseId, string newCode, string newName, int newSemesterId)
        { 
            await db.UpdateCourse(courseId, newCode, newName, newSemesterId);
        }

        public async Task<int> AddAdvisor(string name, string email)
        { 
            return await db.AddAdvisor (name, email);
        }

        public async Task<int> AddDepartment(string name) 
        { 
            return await db.AddDepartment (name);
        }

        public async Task<int> AddSemester(string name)
        { 
            return await db.AddSemester (name);
        }

        public async Task<int> AddCourse(string code, string name, int semesterId)
        { 
            return await db.AddCourse (code, name, semesterId);
        }

        public string getSemesterFromId(int id) {
            foreach (DataRow row in Semesters.Rows) {
                if (Convert.ToInt32(row["id"]) == id) { return Convert.ToString(row["name"]); }
            }

            return "...";
        }


    }
}
