using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface ICoordinatorHome
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

        public Task<DataTable> getExams();
        /*
        Return structure for getExams
        Name        Description         Type
        id          Semester id (pk)    uint
        name        Semester name       string
        batch       Batch               string
        semester_id Semester ID         uint
        semester    Semester Name       string
        status      Is confirmed        uint
        end_date    End date            date

        Need details from all exams
        */

        public Task<DataTable> getExamAndDept();
        /*
        Return structure for getExamAndDept
        Name        Description         Type
        exam_id     Exam ID             uint
        dept_id     Department ID       uint
        
        Need all exams and linking departments from course_in_exam tables
        */
    }
}
