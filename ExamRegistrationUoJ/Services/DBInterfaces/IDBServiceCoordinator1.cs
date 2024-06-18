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

        public Task<int> getCoordinatorID(string email);

        public  Task<DataTable> getexam_id_details_coordinator(string email);

        public Task<DataTable> get_exam_all_details_coordinator(string email);

        public  Task<DataTable> getExamDetails_student(int exam_id, string CourseCode);

        public Task<string> get_coursecode(string courseCode);

        public Task<DataTable> get_enddate(int Exam_id_number);
        public Task<DataTable>? is_confrom_exam_count(string email);

        public Task<DataTable> student_registration_table( string course_code);

        public Task save_change_coordinator_aproval(int exam_course_id, DataTable approval_table);
        /*    
        Return structure for save_change_coordinator_aproval
        Name                Description         Type
        exam_student_id                          int    
        exam_course_id                          string
        is_approved                             string
        attendance                              string

        Need details from all departments
        */
    }
}
