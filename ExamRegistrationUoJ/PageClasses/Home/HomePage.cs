using System.Data;

using ExamRegistrationUoJ.Services.DBInterfaces;
namespace ExamRegistrationUoJ.PageClasses.Home{
    public class HomePage
    {
    
        private IDBServiceHome db;
    
        public HomePage(IDBServiceHome db)
        {
            this.db = db;
        }

    }
}