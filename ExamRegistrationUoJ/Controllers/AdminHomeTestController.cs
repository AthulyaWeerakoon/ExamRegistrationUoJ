using AdminPages;
using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExamRegistrationUoJ.Controllers
{
    [Route("/test")]
    [ApiController]
    public class AdminHomeTestController : Controller
    {
        private readonly AdminHome adminHome;

        public AdminHomeTestController(IDBServiceAdmin1 IDBServiceAdmin1) { 
            adminHome = new AdminHome(IDBServiceAdmin1);
        }

        [HttpGet("filter")]
        public async Task<string> Filter()
        {
            await adminHome.init();

            // test one
            adminHome.departmentOpt = "2";
            adminHome.statusOpt = "0";
            adminHome.semesterOpt = "3";
            adminHome.filterExam();
            string jsonOne = JsonConvert.SerializeObject(adminHome.displayExams);

            // test two
            adminHome.departmentOpt = "All";
            adminHome.statusOpt = "1";
            adminHome.semesterOpt = "All";
            adminHome.filterExam();
            string jsonTwo = JsonConvert.SerializeObject(adminHome.displayExams);

            return $"Test One: {jsonOne}\n\nTest Two: {jsonTwo}";
        }
    }
}
