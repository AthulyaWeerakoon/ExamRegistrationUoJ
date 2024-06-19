using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace CoordinatorPages
{
    public class CoHome
    {
        private IDBServiceCoordinator1 db;


        public DataTable? departments { get; set; }
        public DataTable? semesters { get; set; }
        public DataTable? coordinatorID { get; set; }
        public DataTable? coursesInExam { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";

        public DataTable? Exam_details_coordinator { get; set; } 
        public DataTable? ExamDept_coordinator { get; set; }
        public DataTable? nullData_count { get; set; }


        public CoHome(IDBServiceCoordinator1 db)
        {
            this.db = db;
        }
        public async Task init()
        {
            await getSemesters();
            await getDepartments();
        }

        public async Task getDepartments()
        {
            this.departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
        }

        public async Task<int> getCoordinatorID(string email)
        {
            return await db.getCoordinatorID(email);
        }



        public async Task getExamDept_coordinator(string email)
        {
            this.nullData_count   = await db.getExamDept_coordinator(email);
        }


        public async Task getExamDetails_coordinator(string email)
        {
            this.Exam_details_coordinator = await db.getExamDetails_coordinator(email);
        }


        public async Task<DataTable> filter_exam(string email)
        {
            try
            {
                DataTable examDetails = await db.getExamDetails_coordinator(email);

                if (examDetails == null || examDetails.Rows.Count == 0)
                {
                    return new DataTable(); 
                }

                DataView filteredExamOnce = new DataView(examDetails);

                if (!string.IsNullOrEmpty(this.semesterOpt) && this.semesterOpt != "Semester" && this.semesterOpt != "All")
                {
                    filteredExamOnce.RowFilter = $"semester_id = '{this.semesterOpt}'";
                }

                if (!string.IsNullOrEmpty(this.departmentOpt) && this.departmentOpt != "Department" && this.departmentOpt != "All")
                {
                    if (!string.IsNullOrEmpty(filteredExamOnce.RowFilter))
                    {
                        filteredExamOnce.RowFilter += $" AND department_id = '{this.departmentOpt}'";
                    }
                    else
                    {
                        filteredExamOnce.RowFilter = $"department_id = '{this.departmentOpt}'";
                    }
                }

                Exam_details_coordinator = filteredExamOnce.ToTable();

                return Exam_details_coordinator;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new DataTable(); 
            }
        }







    }
}
