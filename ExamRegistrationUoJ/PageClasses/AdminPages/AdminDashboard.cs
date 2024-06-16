using ExamRegistrationUoJ.Services.DBInterfaces;

namespace ExamRegistrationUoJ.PageClasses.AdminPages
{
    public class AdminDashboard
    {
        private IDBServiceAdminDashboard db;
        
        public AdminDashboard(IDBServiceAdminDashboard db) {
            this.db = db;
        }


    }
}
