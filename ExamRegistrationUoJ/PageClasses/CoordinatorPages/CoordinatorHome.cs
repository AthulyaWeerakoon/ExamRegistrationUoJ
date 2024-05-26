using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Data;

namespace CoordinatorPages
{
    public class CoordinatorHome
    {
        private IDBServiceCoordinator1 db;
        public DataTable? department { get; set; }
        public DataTable? semester { get; set; }
        public DataTable? exams { get; set; }
    }
}
