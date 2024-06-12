using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceCoordinator1
    {

        public Task<DataTable> getDepartments();
        /*    (Coordinator Home)
        Return structure for getDepartments
        Name        Description         Type
        id          Department id (pk)  uint    
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

        public Task<DataTable> getExams();
        /* (Coordinator Home page table)
        Return structure for getExams
        Name        Description         Type
        id          Exam id (pk)        uint
        open_date   Open date           date
        closed_date Closed date         date
        name        Exam name           string
        id          Semester id (pk)    uint
        semester_id Semester ID         uint
        semester    Semester Name       string
        status      Is confirmed        uint
        */

        public Task<DataTable> getExamAndStudent();
        /* (Coordinator approve page)
        Return structure for getExamAndStudent
        Name        Description         Type
        name        Student name        string
        enumber     Student number(pk)  string
        student_advisor Student advisor string
        eligible    Is eligible         uint
        */

        public Task<DataTable> getCorrdinatorCource();
        /* (Coordinator approve page)
        Return structure for getCorrdinatorCource
        Name        Description         Type
        course_id   Course ID           uint
        course_name Course name         string
        */
    }
}
