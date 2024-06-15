using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Collections;
using System.Data;

namespace CoordinatorPages
{
    public class CoHome
    {
        private IDBServiceCoordinator1 db;
        private object filteredCourses;

        public DataTable? departments { get; set; }
        public DataTable? semesters { get; set; }
        public DataTable? coordinatorID { get; set; }

        public DataTable? coursesInExam { get; set; }
        public string departmentOpt { get; set; } = "Department";
        public string semesterOpt { get; set; } = "Semester";


        public DataTable Exam_details_coordinator { get; set; } 

        public DataTable ExamDept_coordinator { get; set; }
        public DataTable nullData_count { get; set; }


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


        //practice to how to get email
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

                // Check if examDetails is null or has no rows
                if (examDetails == null || examDetails.Rows.Count == 0)
                {
                    // Handle the case where no exam details are retrieved
                    return new DataTable(); // Return an empty DataTable
                }

                // Create a DataView from the examDetails DataTable
                DataView filteredExamOnce = new DataView(examDetails);

                // Apply semester filter first
                if (!string.IsNullOrEmpty(this.semesterOpt) && this.semesterOpt != "Semester" && this.semesterOpt != "All")
                {
                    filteredExamOnce.RowFilter = $"semester_id = '{this.semesterOpt}'";
                }

                // Apply department filter
                if (!string.IsNullOrEmpty(this.departmentOpt) && this.departmentOpt != "Department" && this.departmentOpt != "All")
                {
                    // Check if there's already a filter applied
                    if (!string.IsNullOrEmpty(filteredExamOnce.RowFilter))
                    {
                        filteredExamOnce.RowFilter += $" AND department_id = '{this.departmentOpt}'";
                    }
                    else
                    {
                        filteredExamOnce.RowFilter = $"department_id = '{this.departmentOpt}'";
                    }
                }

                // Update Exam_details_coordinator with the filtered results
                Exam_details_coordinator = filteredExamOnce.ToTable();

                // Return the filtered DataTable
                return Exam_details_coordinator;
            }
            catch (Exception ex)
            {
                // Handle the exception by creating an empty DataTable with the same structure
                Console.WriteLine($"Error: {ex.Message}");
                return new DataTable(); // Return an empty DataTable in case of error
            }
        }







    }
}
