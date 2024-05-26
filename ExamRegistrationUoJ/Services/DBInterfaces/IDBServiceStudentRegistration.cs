using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceStudentRegistration
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

        public Task<DataTable> getStudents();
        /*
        parameters : user id/student id

        Return structure for getStudents

        Name        Description         Type
        id          student id(pk)      uint
        name        student name        string
        
        */

        public Task<DataTable> getExams();
        /*
        parameters : ?

        Return structure for getExams

        Name            Description         Type
        id              exam id             uint
        course_id       course id           uint
        course_name     course name         string
        department_id   deparment id        uint
        department_name department name     string
        coordinator_id  coordinator id      uint
        coordinator_name coordinator name   string
        
        Need details from all semesters
        */
    }
}
