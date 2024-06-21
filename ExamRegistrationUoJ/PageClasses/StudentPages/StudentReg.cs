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
        public DataTable? examTitle { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string advisorOpt { get; set; } = "Advisor";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";
        public bool isRepeat { get; set; }
        public bool addDrop { get; set; }
        public string advisor_email { get; set; }
        public byte[] paymentReceipt { get; set; }
        public uint? advisorId { get; set; }

        public async Task init()
        {
            await getSemesters();
            await getDepartments();
        }
        public StudentReg(IDBServiceSR db) {
            this.db = db;
        }

        public async Task getExamTitle(uint exam_id)
        {
            this.examTitle = await db.getExamTitle(exam_id);
        }
        public async Task getDepartments()
        {
            this.departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
        }

        public async Task getCourses(uint exam_id, uint dep_id)
        {
            this.courses = await db.getCourses(exam_id, dep_id);
        }

        public async Task getStudent(uint student_id)
        {
            this.students = await db.getStudent(student_id);
        }

        public async Task getAdvisorId(string ms_email)
        {
            this.advisorId = await db.getAdvisorId(ms_email);
        }
        public async Task setStudentExam(uint student_id, uint exam_id, uint is_proper, uint advisor_id)
        {
            await db.setStudentExams(student_id, exam_id, is_proper, advisor_id);
        }

        public async Task setStudentRegistration(uint exam_studnet_id, uint exam_course_id, string add_or_drop) 
        {
            await db.setStudentRegistration(exam_studnet_id, exam_course_id, add_or_drop);
        }
    }
}
