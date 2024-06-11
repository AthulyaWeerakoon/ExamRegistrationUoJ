using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceStudentHome
    {
        // Retrieves all departments from the database
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
        Task<DataTable> GetExams();
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

        Task<DataView> GetRegisteredExams(string studentId);
        /*
        Parameters:
          - studentId: The ID of the student

        Return structure for GetRegisteredExams:
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

        Description: This method returns a DataView containing the registered exams for the specified student,
                     including exam ID, name, batch, semester ID, semester name, department ID, 
                     department name, registration status, and registration close date.
        */

        Task<DataTable> GetFilteredExams(string departmentOpt, string semesterOpt, string statusOpt);
        /*
        Parameters:
          - departmentOpt: The selected department ID (can be null or empty for no filtering)
          - semesterOpt: The selected semester ID (can be null or empty for no filtering)
          - statusOpt: The selected registration status (can be null or empty for no filtering)

        Return structure for GetFilteredExams:
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

        Description: This method returns a DataTable containing exams filtered by the specified parameters,
                     including exam ID, name, batch, semester ID, semester name, department ID, 
                     department name, registration status, and registration close date.
        */

        Task<bool> RegisterForExam(string studentId, uint examId);
        /*
        Parameters:
          - studentId: The ID of the student
          - examId: The ID of the exam

        Return:
          - bool: True if registration is successful, false otherwise

        Description: This method registers the student for the specified exam and returns a boolean indicating success.
        */
    }
}
