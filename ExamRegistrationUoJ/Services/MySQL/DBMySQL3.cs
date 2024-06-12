using MySqlConnector;
using System.Data;
using Newtonsoft.Json;
using ExamRegistrationUoJ.Services.DBInterfaces;

// Arosha's workspace
using ExamRegistrationUoJ.Services.DBInterfaces;

namespace ExamRegistrationUoJ.Services.MySQL
{
    public partial class DBMySQL : IDBServiceCoordinator1
    {

        // This method is only here as a placeholder, remove it once the weather page has been replaced
        /*public async Task<DataTable?> GetMostRentedFromSakila()
        {
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(sakileTestJson);

            return dataTable;
        }*/

        /*public async Task<bool> IsAnAdministrator(string nameidentifier)
        {
            return false;
        }

        public async Task<bool> IsACoordinator(string nameidentifier)
        {
            return false;
        }

        public async Task<bool> IsAStudent(string nameidentifier)
        {
            return false;
        }

        public async Task<bool> IsAnAdvisor(string nameidentifier)
        {
            return true;
        }*/


        public Task<DataTable> getCorrdinatorCource()
        {
            throw new NotImplementedException();
        }

   

        public Task<DataTable> getExamAndStudent()
        {
            throw new NotImplementedException();
        }

        public Task<DataTable> GetExams()
        {
            throw new NotImplementedException();
        }

    
    }

}
