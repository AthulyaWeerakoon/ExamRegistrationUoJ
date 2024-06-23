using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace AdvisorPages
{
    public class AdvisorApproval
    {
        private IDBServiceAdvisor1 db;

        public AdvisorApproval(IDBServiceAdvisor1 db)
        {
            this.db = db;
        }

    }
}
