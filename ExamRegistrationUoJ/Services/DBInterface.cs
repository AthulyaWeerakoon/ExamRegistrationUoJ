using System.Data;

namespace ExamRegistrationUoJ.Services
{
    public interface DBInterface
    {
        Task<DataTable> GetMostRentedFromSakila();
    }
}
