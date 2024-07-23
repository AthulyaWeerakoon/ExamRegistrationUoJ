using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceStudentHome
    {
        Task<DataTable> getRegisteredExams(int studentId);
        /*
        Parameters:
          - studentId: The ID of the student

        Return structure for GetRegisteredExams:
        Column Name   Description         Type
        ------------------------------------------------
        id            Exam id (pk)        int
        name          Exam name           string
        batch         Batch               string
        semester_id   Semester ID         uint
        semester      Semester Name       string
        department_id Department ID       int
        department    Department Name     string
        registration_status Registration Status  string
        registration_close_date Registration Close Date date

        Description: This method returns a DataView containing the registered exams for the specified student,
                     including exam ID, name, batch, semester ID, semester name, department ID, 
                     department name, registration status, and registration close date.
        */

        Task<DataTable> getExams();
        /*
        Return structure for GetExams:
        Column Name   Description         Type
        ------------------------------------------------
        id            Exam id (pk)        uint
        name          Exam name           string
        batch         Batch               string
        semester_id   Semester ID         uint
        semester      Semester Name       string
        department_id Department ID       uint
        department    Department Name     string
        registration_status Registration Status  string
        registration_close_date Registration Close Date date

        Description: This method returns a DataTable containing the details of all exams, 
                     including exam ID, name, batch, semester ID, semester name, department ID, 
                     department name, registration status, and registration close date.
        */


        Task<DataTable> getFilteredExams(int semesterId);
        /*
        Parameters:
          - departmentId: The ID of the department to filter exams.
          - semesterId: The ID of the semester to filter exams.

        Return structure for GetExams:
        Column Name       Description                Type
        ------------------------------------------------------------
        id                Exam ID (pk)               int
        description       Exam Description           string
        semester_id       Semester ID                int
        semester          Semester Name              string
        department_id     Department ID              int
        department        Department Name            string
        approval_opens    Approval Opens Date        DateTime
        closed            Closed Date                DateTime
        registration_status Registration Status      string

        Description: This method returns a DataTable containing exams filtered by the provided
                     department ID and semester ID, including exam ID, description, semester ID,
                     semester name, department ID, department name, approval opens date, closed date,
                     and registration status.
        */

        Task<bool> registerForExam(int studentId, int examId);
        /*
        Parameters:
          - studentId: The ID of the student
          - examId: The ID of the exam

        Return:
          - bool: True if registration is successful, false otherwise

        Description: This method registers the student for the specified exam and returns a boolean indicating success.
        */


        Task<int?> getStudentIdByEmail(string email);
        /*
        Parameters:
          - email: The email of the student

        Return:
          - int?: The ID of the student or null if not found

        Description: This method retrieves the student ID based on the provided email.
        */


        //this method is implemented in DBServiceAdmin
        Task<DataTable> getDepartments();
        /*
        Return structure for GetDepartments:
        Column Name   Description         Type
        ------------------------------------------------
        id            Department id (pk)  uint
        name          Department name     string

        Description: This method returns a DataTable containing the details of all departments.
        */

        // Retrieves all semesters from the database


        //this method is implemented in DBServiceAdmin
        Task<DataTable> getSemesters();
        /*
        Return structure for GetSemesters:
        Column Name   Description         Type
        ------------------------------------------------
        id            Semester id (pk)    uint
        name          Semester name       string

        Description: This method returns a DataTable containing the details of all semesters.
        */

        // Retrieves all exams from the database

        Task<DataTable> getCoursesForExam(int examId);
        /*
        Parameters:
          - examId: The ID of the exam

        Return structure for GetCoursesForExam:
        Column Name   Description         Type
        ------------------------------------------------
        course_code   Course code         string
        course_name   Course name         string

        Description: This method returns a DataTable containing the courses related to the specified exam,
                     including course code and course name.
        */





    }
}
