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
        public DataTable? initialRegisteredCourses { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";
        public string statusOpt { get; set; } = "Registration Status";
        public bool isRepeat { get; set; }
        public bool addDrop { get; set; }
        public string advisor_email { get; set; }
        public byte[] paymentReceipt { get; set; }
        public string fileType { get; set; }
        public uint advisorId { get; set; }

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

        public async Task getInitialRegisteredCourses(uint exam_student_id)
        {
            this.initialRegisteredCourses = await db.getInitialRegisteredCourses(exam_student_id);
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

        public async Task<int> setStudentRegistration(uint exam_student_id, uint exam_course_id, string add_or_drop) 
        {
            return await db.setStudentRegistration(exam_student_id, exam_course_id, add_or_drop);
        }
        public async Task<int> setPayments(uint student_id, uint exam_id, byte[] payment_receipt, string content_type) 
        {
            return await db.setPayments(student_id, exam_id, payment_receipt, content_type);
        }

        public async Task<int> isAdded(uint exam_student_id, uint exam_course_id)
        {
            return await db.isAdded(exam_student_id, exam_course_id);
        }

        ////
        public async Task<int?> getStudentIdByEmail(string email) 
        { 
            return await db.getStudentIdByEmail(email);
        }
    }
}
