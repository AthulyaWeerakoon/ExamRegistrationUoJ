using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace CoordinatorPages
{
    public class CoordinatorApproval
    {
        private IDBServiceCoordinator1 db;
        public DataTable? examAndStudent { get; set; }
        public DataTable? coordinatorCourse { get; set; }
    }
}
