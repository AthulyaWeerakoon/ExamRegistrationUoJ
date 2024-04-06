using System.Data;

namespace ExamRegistrationUoJ.Services
{
    public interface DBInterface
    {
        Task<DataTable> GetMostRentedFromSakila(); // this interface is a placeholder to be removed upon the removal of weather page
    }
}
