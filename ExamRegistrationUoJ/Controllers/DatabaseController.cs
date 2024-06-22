using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services;
using ExamRegistrationUoJ.Services.DBInterfaces;
using ExamRegistrationUoJ.Services.MySQL;

namespace BlazorApp1.Controllers
{
    [Route("/database")]
    [ApiController]

    public class DatabaseController : ControllerBase
    {
        private readonly DBInterface _dbInterface;
        private readonly IDBServiceAdmin1 _IDBServiceAdmin1;
        private readonly IDBServiceStudentHome _IDBServiceStudentHome;
        private readonly IDBServiceSR _IDBServiceStudentRegistration;

        public DatabaseController(DBInterface dbInterface, IDBServiceAdmin1 IDBServiceAdmin1, IDBServiceStudentHome iDBServiceStudentHome, IDBServiceSR iDBServiceStudentRegistration)
        {
            _dbInterface = dbInterface;
            _IDBServiceAdmin1 = IDBServiceAdmin1;
            _IDBServiceStudentHome = iDBServiceStudentHome;
            _IDBServiceStudentRegistration = iDBServiceStudentRegistration;
        }

        [HttpGet("sakila")]
        public async Task<string> Sakila()
        {
            string JsonString = JsonConvert.SerializeObject(await _dbInterface.GetMostRentedFromSakila());
            return JsonString;
        }

        //checked
        [HttpGet("getDepartments")]
        public async Task<string> GetDepartments()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getDepartments());
            return jsonString;
        }

        //checked
        [HttpGet("getSemesters")]
        public async Task<string> GetSemesters()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getSemesters());
            return jsonString;
        }


        //checked
        [HttpGet("getActiveExams")]
        public async Task<string> GetActiveExams()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getActiveExams());
            return jsonString;
        }

        //checked
        [HttpGet("getCompletedExams")]
        public async Task<string> GetCompletedExams()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getCompletedExams());
            return jsonString;
        }

        //checked
        [HttpGet("getAllCoursesInExam")]
        public async Task<string> GetAllCoursesInExam()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getAllCoursesInExam());
            return jsonString;
        }

        //checked
        [HttpGet("getExamDescription/{exam_id}")]
        public async Task<string> GetExamDescription([FromRoute] int exam_id)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getExamDescription(exam_id));
            return jsonString;
        }

        //checked
        [HttpGet("getCoursesInExam/{exam_id}")]
        public async Task<string> GetCoursesInExam([FromRoute] int exam_id)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getCoursesInExam(exam_id));
            return jsonString;
        }

        //checked
        [HttpGet("getCoordinators")]
        public async Task<string> GetCoordinators()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getCoordinators());
            return jsonString;

        }
         
        //checked
        [HttpGet("addCoordinator/{email}")]
        public async Task<string> AddCoordinator([FromRoute] string email)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.addCoordinator(email));
            return jsonString;
        }

        //save chnages method is impossible lOl

        
        //checked
        [HttpGet("getCoursesFromDepartment/{deptId}")]
        public async Task<string> GetCoursesFromDepartment([FromRoute] int deptId)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getCoursesFromDepartment(deptId));
            return jsonString;
        }

        



        // Student Home ORM Checking

        //checked
        [HttpGet("getRegisteredExams/{student_id}")]
        public async Task<string> GetRegisteredExams([FromRoute] int student_id)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceStudentHome.getRegisteredExams(student_id));
            return jsonString;
        }

        // checked
        [HttpGet("getExams")]
        public async Task<string> GetExams()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceStudentHome.getExams());
            return jsonString;

        }

        //can not check no data in db
        [HttpGet("getFilteredExams/{department_id}/{semester_id}/{status_id}")]
        public async Task<string> GetFilteredExams([FromRoute] int department_id, int semester_id ,int status_id)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceStudentHome.getFilteredExams(department_id, semester_id, status_id));
            return jsonString;
        }

        //checked
        [HttpGet("registerForExam/{student_id}/{exam_id}")]
        public async Task<string> RegisterForExam([FromRoute] int student_id, int exam_id)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceStudentHome.registerForExam(student_id, exam_id));
            return jsonString;
        }


        //checked
        [HttpGet("getStudentIdByEmail/{email}")]
        public async Task<string> getStudentIdByEmail([FromRoute] string email)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceStudentHome.getStudentIdByEmail(email));
            return jsonString;
        }


        [HttpGet("getCoursesForExam/{exam_id}")]
        public async Task<string> GetCoursesForExam([FromRoute] int exam_id)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceStudentHome.getCoursesForExam(exam_id));
            return jsonString;
        }
    }
}