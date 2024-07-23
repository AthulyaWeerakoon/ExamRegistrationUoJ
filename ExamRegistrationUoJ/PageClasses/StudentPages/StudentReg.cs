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
        public DataTable? students { get; set; }
        public DataTable? examTitle { get; set; }
        public DataTable? advisors { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public bool isRepeat { get; set; }
        public string paymentReceipt { get; set; }
        public uint advisorId { get; set; }

        ////
        public DataTable? addedCourses { get; set; }
        public DataTable? notAddedCourses { get; set; }

        public async Task init()
        {
            await getSemesters();
            await getAdvisors();
        }
        public StudentReg(IDBServiceSR db) {
            this.db = db;
        }

        public async Task getExamTitle(uint exam_id)
        {
            this.examTitle = await db.getExamTitle(exam_id);
        }

        public async Task getDepartmentsInExam(uint exam_id)
        {
            this.departments = await db.getDepartmentsInExam(exam_id);
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
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

        public async Task<int> setStudentInExam(uint student_id, uint exam_id, uint is_proper, uint advisor_id)
        {
            return await db.setStudentInExams(student_id, exam_id, is_proper, advisor_id);
        }

        public async Task<int> setPayments(uint student_id, uint exam_id, string payment_receipt) 
        {
            return await db.setPayments(student_id, exam_id, payment_receipt);
        }

        ////
        public async Task<int?> getStudentIdByEmail(string email) 
        { 
            return await db.getStudentIdByEmail(email);
        }

        ////
        public async Task getCoursesInStudentRegistration(int? studentInExamId, uint departmentId) 
        {
            this.addedCourses = await db.getCoursesInStudentRegistration(studentInExamId, departmentId);
        }

        public async Task getCoursesNotInStudentRegistration(int examId, int? studentInExamId, uint departmentId)
        {
            this.notAddedCourses = await db.getCoursesNotInStudentRegistration(examId, studentInExamId, departmentId);
        }

        public async Task setStudentRegistration(DataTable courseStates, int? studentInExamId)
        {
            await db.setStudentRegistration(courseStates, studentInExamId);
        }
    }
}
