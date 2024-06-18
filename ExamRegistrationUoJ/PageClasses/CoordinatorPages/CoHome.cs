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

        public DataTable? exams_semseter_and_exam_id_details { get; set; } 
        public DataTable? exams_all_details { get; set; }
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



        public async Task getexam_id_details_coordinator(string email)
        {
            this.nullData_count   = await db.getexam_id_details_coordinator(email);
        }


        public async Task get_exam_all_details_coordinator(string email)
        {
            this.exams_all_details = await db.get_exam_all_details_coordinator(email);
        }


        public async Task<DataTable> filter_exam(string email)
        {
            try
            {
                DataTable examDetails = await db.get_exam_all_details_coordinator(email);

                if (examDetails == null || examDetails.Rows.Count == 0)
                {
                    return new DataTable();
                }

                DataView filteredExamOnce = new DataView(examDetails);
                List<string> filters = new List<string>();

                if (!string.IsNullOrEmpty(this.semesterOpt) && this.semesterOpt != "Semester" && this.semesterOpt != "All")
                {
                    filters.Add($"semester_id = '{this.semesterOpt}'");
                }

                if (!string.IsNullOrEmpty(this.departmentOpt) && this.departmentOpt != "Department" && this.departmentOpt != "All")
                {
                    filters.Add($"department_id = '{this.departmentOpt}'");
                }

                if (filters.Count > 0)
                {
                    filteredExamOnce.RowFilter = string.Join(" AND ", filters);
                }

                exams_all_details = filteredExamOnce.ToTable();
                return exams_all_details;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new DataTable();
            }
        }

    }
}
