using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace StudentPages
{
    public class StudentReg
    {
        private IDBServiceSR db;
        public DataTable? departments { get; set; }
        public DataTable? semesters { get; set; }
        public DataTable? courses { get; set; }
        public DataTable? students { get; set; }
        public DataTable? advisors { get; set; }
        public DataTable? displayCourses { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string advisorOpt { get; set; } = "Advisor";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";

        public async Task init()
        {
            await getSemesters();
            await getDepartments();
        }
        public StudentReg(IDBServiceSR db) {
            this.db = db;
        }
        public async Task getDepartments()
        {
            this.departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
        }

        public async Task getCourses(ulong exam_id) 
        {
            this.courses = await db.getCourses(exam_id);
        }

        public async Task filterCourses()
        {
            DataView filteredCourses = new DataView(courses);
            try {
                if (this.departmentOpt != "Department" && this.departmentOpt != "All")
                {
                    filteredCourses.RowFilter = $"dep_id = {departmentOpt}";
                    displayCourses = filteredCourses.ToTable();
                }
                else
                {
                    displayCourses = filteredCourses.ToTable();
                }
            }
            catch(Exception ex) {
                displayCourses = filteredCourses.Table.Clone();
            }

            
        }

        public async Task getStudent(ulong student_id)
        {
            this.students = await db.getStudent(student_id);
        }

        public async Task getAdvisors()
        {
            this.advisors = await db.getAdvisors();
        }
    }
}
