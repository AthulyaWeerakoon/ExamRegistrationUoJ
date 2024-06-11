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

        public DatabaseController(DBInterface dbInterface,IDBServiceAdmin1 IDBServiceAdmin1)
        {
            _dbInterface = dbInterface;
            _IDBServiceAdmin1 = IDBServiceAdmin1;
        }

        [HttpGet("sakila")]
        public async Task<string> Sakila()
        {
            string JsonString = JsonConvert.SerializeObject(await _dbInterface.GetMostRentedFromSakila());
            return JsonString;
        }

        [HttpGet("getDepartments")]
        public async Task<string> GetDepartments()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getDepartments());
            return jsonString;
        }

        [HttpGet("getSemesters")]
        public async Task<string> GetSemesters()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getSemesters());
            return jsonString;
        }

        [HttpGet("getActiveExams")]
        public async Task<string> GetActiveExams()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getActiveExams());
            return jsonString;
        }

        [HttpGet("getCompletedExams")]
        public async Task<string> GetCompletedExams()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getCompletedExams());
            return jsonString;
        }
        
        [HttpGet("getAllCoursesInExam")]
        public async Task<string> GetAllCoursesInExam()
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getAllCoursesInExam());
            return jsonString;
        }


        [HttpGet("getCoursesInExam/{exam_id}")]
        public async Task<string> GetCoursesInExam([FromRoute] int exam_id)
        {
            string jsonString = JsonConvert.SerializeObject(await _IDBServiceAdmin1.getCoursesInExam(exam_id));
            return jsonString;
        }



    }
}
