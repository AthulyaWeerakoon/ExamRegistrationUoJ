using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace CoordinatorPages
{
    public class CoApproval
    {
        private IDBServiceCoordinator1 db;
        public DataTable? examAndStudent { get; set; }
        public DataTable? coordinatorCourse { get; set; }

        public CoApproval(IDBServiceCoordinator1 db)
        {
            this.db = db;
        }

    }
}
