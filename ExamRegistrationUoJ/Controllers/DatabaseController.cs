using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services;

namespace BlazorApp1.Controllers
{
    [Route("/database")]
    [ApiController]

    public class DatabaseController : ControllerBase
    {
        private readonly DBInterface _dbInterface;

        public DatabaseController(DBInterface dbInterface)
        {
            _dbInterface = dbInterface;
        }

        [HttpGet("sakila")]
        public async Task<string> Sakila()
        {
            string JsonString = JsonConvert.SerializeObject(await _dbInterface.GetMostRentedFromSakila());
            return JsonString;
        }
    }
}
