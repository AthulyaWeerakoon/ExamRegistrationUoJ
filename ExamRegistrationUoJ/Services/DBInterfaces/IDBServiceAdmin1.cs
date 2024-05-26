using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceAdmin1
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

        public Task<DataTable> getActiveExams();
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

        Need details from all exams of which end_date has not yet passed
        */

        public Task<DataTable> getCompletedExams();
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

        Need details from all exams of which end_date has passed
        */

        public Task<DataTable> getAllCoursesInExam();
        /*
        Return structure for getExamAndDept
        Name        Description         Type
        course_name Name of the Course  string
        course_code Code of the Course  string
        exam_id     Exam ID             uint
        dept_id     Department ID       uint
        
        Need all exams and linking departments from course_in_exam tables
        */

        public Task<string> getExamTitle(int exam_id);
        /*
        Need exam name given its id
        */

        public Task<int> getExamSemester(int exam_id);
        /*
        Need exam name given its id
        */

        public Task<string?> getExamBatch(int exam_id);
        /*
        Need exam proper batch given its id, null if empty
        */

        public Task<DateTime?> getExamEndDate(int exam_id);
        /*
        Need exam end date given its id, null if empty
        */

        public Task<DataTable?> getCoursesInExam(int exam_id);
        /*
        Return structure for getCoursesInExam
        Name                Description                                             Type
        course_name         Name of the Course                                      string
        course_code         Code of the Course                                      string
        coordinator_id      Id of the coordinator; -1 if not assigned               int
        coordinator_name    Name of the coordinator; empty string if not assigned   string
        dept_id             Department ID                                           uint

        Need courses in exam given its id, null if empty
        */
    }
}
