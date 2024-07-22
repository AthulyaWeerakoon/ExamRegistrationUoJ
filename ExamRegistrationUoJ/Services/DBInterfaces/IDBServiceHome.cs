using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceHome
    {
        Task<DataTable> GetActiveExamsHome();
    }
}