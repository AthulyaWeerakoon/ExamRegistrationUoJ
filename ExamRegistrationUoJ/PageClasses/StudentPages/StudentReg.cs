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
        public DataTable? advisors { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";
        public bool isRepeat { get; set; }
        public bool addDrop { get; set; }
        public string advisor_email { get; set; }
        public byte[] paymentReceipt { get; set; }
        public uint advisorId { get; set; }
        public uint courseInExamId { get; set; }
        public uint studentInExamId { get; set; }

        public async Task init()
        {
            await getSemesters();
            await getDepartments();
            await getAdvisors();
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

        public async Task getAdvisors() 
        {
            this.advisors = await db.getAdvisors();
        }

        public async Task getStudentInExamId(uint student_id, uint exam_id)
        {
            this.studentInExamId = await db.getStudentInExamId(student_id, exam_id);
        }

        public async Task<int> setStudentInExam(uint student_id, uint exam_id, uint is_proper, uint advisor_id)
        {
            return await db.setStudentInExams(student_id, exam_id, is_proper, advisor_id);
        }

        public DataTable? test_dt { get; set; }
        public async Task test()
        {
            this.test_dt = await db.test_a();
        }
        /*public async Task<int> AddAdvisor(string name, string email)
        {
            return await db.AddAdvisor(name, email);
        }*/


        public async Task<int> setStudentRegistration(uint exam_student_id, uint exam_course_id, string add_or_drop) 
        {
            return await db.setStudentRegistration(exam_student_id, exam_course_id, add_or_drop);
        }
        public async Task setPayments(uint student_id, uint exam_id, byte[] payment_receipt) 
        {
            await db.setPayments(student_id, exam_id, payment_receipt);
        }
    }
}
