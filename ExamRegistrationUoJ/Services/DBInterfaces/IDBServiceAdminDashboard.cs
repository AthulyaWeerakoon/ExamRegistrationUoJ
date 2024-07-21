//this shit is mine (ramith)

using System.Collections;
using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceAdminDashboard
    {
        //done
        public Task<DataTable> getDepartments();
        /*
        Return structure for getDepartments
        Name        Description         Type
        id          Department id (pk)  uint
        name        Department name     string
        
        Need details from all departments
        */

        //done
        public Task<DataTable> getSemesters();
        /*
        Return structure for getSemesters
        Name        Description         Type
        id          Semester id (pk)    uint
        name        Semester name       string
        
        Need details from all semesters
        */

        //done
        // implemented in student registration
        public Task<DataTable?> getAllCourses();
        /*
        Return structure for getCourses
        Name        Description                                 Type
        id          Course in exam id                           unit
        name        Name of the Course                          string
        code        Code of the Course                          string
        semester_id Id of the semester                          int

        Need all courses, null if empty
        */

        public Task<DataTable> getAdvisors();
        /*
        Return structure for getCourses
        Name        Description                                 Type
        id          Course in exam id                           unit
        name        Name of the Course                          string
        code        Code of the Course                          string
        semester_id Id of the semester                          int

        Need all courses, null if empty
        */

        public Task UpdateAdvisor(int advisorId, string newName, string newEmail);
        public Task UpdateDepartmentName(int departmentId, string newName);
        public Task UpdateSemesterName(int semesterId, string newName);
        public Task UpdateCourse(int courseId, string newCode, string newName, int newSemesterId, int[] departments);
        public Task<int> AddAdvisor(string name, string email);
        public Task<int> AddDepartment(string name);
        public Task<int> AddSemester(string name);
        public Task<int> AddCourse(string code, string name, int semesterId, int[] departments);
        Task<string> DropDepartment(int departmentId);
        Task<string> DropSemester(int semesterId);
        Task<string> DropAdvisor(int advisorId);
        Task<string> DropCourse(int courseId);
        string IntListToDelimitedString(List<int> intList);
        List<int> DelimitedStringToIntList(string delimitedString);
        Task<List<int>> GetCourseDeptsLinks(int courseId);
    }
}
