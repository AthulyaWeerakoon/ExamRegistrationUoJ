using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceCoordinator1
    {

        public Task<DataTable> getDepartments();
        /*    (Coordinator Home)
        Return structure for getDepartments
        Name        Description         Type
        id          Department id (pk)  uint    S
        name        Department name     string
        Need details from all departments
        */

        public Task<DataTable> getSemesters();
        /* (Coordinator Home)
        Return structure for getSemesters
        Name        Description         Type
        id          Semester id (pk)    uint
        name        Semester name       string
        
        Need details from all semesters
        */

        //practice to how to get email
        public Task<int> GetCoordinatorID(string email);

        public  Task<DataTable> getExamDept_coordinator(string email);

        public Task<DataTable> getExamDetails_coordinator(string email);

        public  Task<DataTable> getExamDetails_student(int exam_id);

        public Task<string> get_coursecode(string courseCode);

        public Task<DataTable> get_enddate(int Exam_id_number);
        //ramith's workspace
        //public  Task<DataTable> getStudentDetails_in_Course(int exam_id, string course_id, int coordinator_id);
    }
}
