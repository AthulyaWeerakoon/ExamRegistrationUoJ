using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceSR
    {
        public Task<DataTable> getDepartments();
        /*
        Return structure for getDepartments

        Name        Description         Type
        id          Department id (pk)  uint
        name        Department name     string
        
        Need details from all departments
        */

        public Task<DataTable> getSemesters();
        /*
        Return structure for getSemesters

        Name        Description         Type
        id          Semester id (pk)    uint
        name        Semester name       string
        
        Need details from all semesters
        */

        public Task<DataTable> getCourses(uint examId, uint depId);
        /*
        input parameters : ulong examId

        Return structure for getCourse 

        Name        Description         Type
        id          id(pk)              uint
        dep_id      department id       uint
        name        course name         string
        coordinator course coordinator  string
        code        course code         string

        need details of courses where examId = exam_id in courses in exam table
        
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
        /*

        Return structure for getAdvisors
        Name        Description         Type
        id          advisor id(pk)      uint
        name        advisor name        string
        */

        public Task<uint> getCourseInExamId(string examId, string depId, string courseId);

        public Task<uint> getStudentInExamId(string studentId, string examId);

        public Task<DataTable> getExamTitle(uint examId);

        public Task<int> setStudentExams(uint studentId, uint examId, uint isProper, uint advisorId);

        public Task<int> setStudentRegistration(uint examStudnetId, uint examCourseId, string addOrDrop);
    }
}
