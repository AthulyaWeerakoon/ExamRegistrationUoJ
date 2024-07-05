using System.Data;
using System.Threading.Tasks;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceAdvisorHome
    {
        Task<DataTable> getDepartments();
        /*
        Return structure for getDepartments:
        Column Name   Description         Type
        ------------------------------------------------
        id            Department ID (pk)  int
        name          Department Name     string

        Description: This method returns a DataTable containing all the departments,
                     including department ID and name.
        */

        Task<DataTable> getSemesters();
        /*
        Return structure for getSemesters:
        Column Name   Description         Type
        ------------------------------------------------
        id            Semester ID (pk)    int
        name          Semester Name       string

        Description: This method returns a DataTable containing all the semesters,
                     including semester ID and name.
        */

        Task<DataTable> getExams(int departmentId, int semesterId);
        /*
        Parameters:
          - departmentId: The ID of the department to filter exams.
          - semesterId: The ID of the semester to filter exams.

        Return structure for getExams:
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

        Description: This method returns a DataTable containing exams filtered by the provided
                     department ID and semester ID, including exam ID, description, semester ID,
                     semester name, department ID, department name, approval opens date, and closed date.
        */

        Task<DataTable> getCoursesForExam(int examId);
        /*
        Parameters:
          - examId: The ID of the exam to fetch courses for.

        Return structure for getCoursesForExam:
        Column Name       Description                Type
        ------------------------------------------------------------
        id                Course ID (pk)             int
        code              Course Code                string
        name              Course Name                string

        Description: This method returns a DataTable containing courses for the specified exam,
                     including course ID, course code, and course name.
        */


        Task<DataTable> getExamForAdvisorApproval(int semesterId);
        /*
        Parameters:
          - semesterId: The ID of the semester to filter exams.

        Return structure for getExams:
        Column Name       Description                Type
        ------------------------------------------------------------
        id                Exam ID (pk)               int
        description       Exam Description           string
        semester_id       Semester ID                int
        semester          Semester Name              string
        approval_opens    Approval Opens Date        DateTime
        closed            Closed Date                DateTime

        Description: This method returns a DataTable containing exams filtered by the provided
                     department ID and semester ID, including exam ID, description, semester ID,
                     semester name, department ID, department name, approval opens date, and closed date  where 
        date + coordinator extended date< current date<date + coddinator extension+ advisor extension
        */


    }
}
