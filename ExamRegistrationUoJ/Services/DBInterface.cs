using System.Data;

namespace ExamRegistrationUoJ.Services
{
    public interface DBInterface
    {
        Task<DataTable> GetMostRentedFromSakila(); // this interface is a placeholder to be removed upon the removal of weather page
        Task<bool> IsAnAdministrator(string nameidentifier);
        Task<bool> IsACoordinator(string nameidentifier);
        Task<bool> IsAStudent(string nameidentifier);
        Task<bool> IsAnAdvisor(string nameidentifier);
    }
}
