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
    }
}
