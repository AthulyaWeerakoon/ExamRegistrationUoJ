using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceSR
    {
        public Task<DataTable> getDepartmentsInExam(uint examId);
        /*
        Return structure for getDepartmentsInExam

        Name        Description         Type
        id          Department id (pk)  uint
        name        Department name     string
        
        Need details from all departments from given exam
        */

        public Task<DataTable> getSemesters();
        /*
        Return structure for getSemesters

        Name        Description         Type
        id          Semester id (pk)    uint
        name        Semester name       string
        
        Need details from all semesters
        */


        public Task<DataTable> getStudent(uint studentId);
        /*
        INput parameters : ulong studentId

        Return structure for getCourse 

        Name        Description         Type
        id          id(pk)              uint
        name        student name        string
        ms_email       student email       string

        need name and email address for given student id
        */

        public Task<uint> getAdvisorId(string msEmail);

        public Task<DataTable> getAdvisors();

        public Task<DataTable> getExamTitle(uint examId);

        public Task<int> setStudentInExams(uint studentId, uint examId, uint isProper, uint advisorId);

        public Task<int> setPayments(uint studnetId, uint examId, string payment_receipt, string receipt_number);

        ///
        public Task<int?> getStudentIdByEmail(string email);

        ///
        public Task<DataTable?> getCoursesInStudentRegistration(int? studentInExamId, uint departmentId);

        public Task<DataTable?> getCoursesNotInStudentRegistration(int examId, int? studentInExamId, uint departmentId);

        public Task setStudentRegistration(DataTable courseStates, int? studentInExamId);
    }
}
