using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace StudentPages
{
    public class StudentRegistration
    {
        private IDBServiceStudentRegistration db;
        public DataTable? departments { get; set; }
        public DataTable? semesters { get; set; }
        public DataTable? students { get; set; }
        private DataTable? allExams { get; set; }
        private DataTable? examDept { get; set; }
        public DataTable? exams { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";
        StudentRegistration(IDBServiceStudentRegistration db)
        {
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

        //get students from students table 
        public async Task getStudents()
        {
            this.students = await db.getStudents();
        }
        //get exams from courses_in_exam table
        public async Task getExams()
        {
            this.exams = await db.getExams();
        }
    }
}