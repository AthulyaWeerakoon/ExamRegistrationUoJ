//this shit is mine (ramith)

using System.Collections;
using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceAdmin1
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
        public Task<DataTable> getActiveExams(); // <----
        /*
        Return structure for getExams
        Name        Description         Type
        id          exam id (pk)    uint
        name        exam name       string
        batch       Batch               string
        semester_id Semester ID         uint
        semester    Semester Name       string
        status      Is confirmed        uint
        end_date    End date            date

        Need details from all active exams
        Active Exams are exams that are either,
            a) not confirmed 
            b) end_date + approval_extension has not yet passed
        */

        //done
        public Task<DataTable> getCompletedExams(); // <---
        /*
        Return structure for getExams
        Name        Description         Type
        id          exam id (pk)    uint
        name        exam name       string
        batch       Batch               string
        semester_id Semester ID         uint
        semester    Semester Name       string
        status      Is confirmed        uint
        end_date    End date            date

        Need details from all exams which are not active 
        */

        //done
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

        //done
        public Task<DataTable?> getExamDescription(int exam_id);
        /*
        Return structure for getExamAndDept
        Name                            Description         Type
        name                            Exam name           string
        semester_id                     Semester id (pk)    uint
        batch                           Batch               string
        end_date                        End Date            date
        coordinator_approval_extension  Approval Extension  uint
        advisor_approval_extension      Approval Extension  uint
        is_confirmed                    Is Confirmed        uint
        
        Need exam description given its id, null if empty. Only fetch once.
        */

        //done
        public Task<DataTable?> getCoursesInExam(int exam_id);
        /*
        Return structure for getCoursesInExam
        Name                Description                                             Type
        id                  Course in exam id                                       unit
        course_name         Name of the Course                                      string
        course_code         Code of the Course                                      string
        coordinator_id      Id of the coordinator; -1 if not assigned               int
        dept_id             Department ID                                           uint

        Need courses in exam given its id, null if empty
        */

        // done
        public Task<DataTable?> getCoordinators();
        /*
        Return structure for getCoordinators
        Name    Description         Type
        id      Course in exam id   unit
        email   Email address       string

        Need courses in exam given its id, null if empty
        */


        // public Task<int> addCoordinator(string email);
        /*
        Parameter description for saveChanges
        email - email address of the coordinator

        Add the coordinator email to the database and set remaining nullable fields as nullable and other fields with placeholder value 'placeholder',
        and return the id of the newly added coordinator
        */

        // public Task saveChanges(int? examId, string? examTitle, int? semester, string? batch, int? cordTimeExtent, int? adviTimeExtent, List<int>? removeList, DataTable? updateList, DataTable? addList);
        /*
        Parameter description for saveChanges
        examId          - exam id
        examTitle       - exam name
        semester        - semster id
        batch           - batch
        coordTimeExtent - coordinator_approval_extension
        adviTimeExtent  - advisor_approval_extension
        removeList      - list of course in exam ids to be removed from the database
        addList         - list of courses to be added into the course in exam

        Structure for addList DataTable
        Name            Description                     Type
        exam_id         Exam id                         unit
        course_id       Course id                       unit
        department_id   Department id                   unit
        coordinator_id  Coordinator id, -1 if not set   int

        Structure for addList DataTable
        Name                Description                     Type
        course_in_exam_id   Exam id                         unit
        coordinator_id      Coordinator id, -1 if not set   int

        This function updates the database with the informed changes, removes ids mentioned in remove list, updates coordinators of ids mentioned in updatelist and adds courses_in_exam in addList
        */

        // public Task<DataTable> getCoursesFromDepartment(int deptId);
        /*
        Return structure for getCoursesFromDepartments
        Name            Description     Type
        course_id       Course id       unit
        course_name     Course name     string
        course_code     Course code     string

        Need all courses, null if empty
        */

        //done
        // implemented in student registration
        public Task<DataTable?> getCourses();
        /*
        Return structure for getCoursesInExam
        Name        Description                                 Type
        id          Course in exam id                           unit
        name        Name of the Course                          uint
        code        Code of the Course                          uint
        semester_id Id of the coordinator; -1 if not assigned   int

        Need all courses, null if empty
        */

        //done
        

        

        

        
    }
}
