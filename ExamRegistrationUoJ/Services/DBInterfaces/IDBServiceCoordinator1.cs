using System.Data;

namespace ExamRegistrationUoJ.Services.DBInterfaces
{
    public interface IDBServiceCoordinator1
    {

        public Task<DataTable> getDepartments();
        /*    (Coordinator Home)
        Return structure for getDepartments
        Name        Description         Type
        id          Department id (pk)  uint    
        name        Department name     string
        Need details from all departments
        */

        public Task<DataTable> getSemesters();
        /* (Coordinator Home)
        Return structure for getSemesters
        Name        Description         Type
        id          Semester id (pk)    uint
        name        Semester name       string
        
        Need details from all semesters
        */

        
    }
}
