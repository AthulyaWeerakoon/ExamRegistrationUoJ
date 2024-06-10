using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Identity.Client;
using System.Collections;
using System.Data;

namespace AdminPages
{
    struct NewExamForm {
        public string? examTitle;
        public int? semester;
        public string? batch;
        public DateTime? endDate;
        public List<string?> depts;
        public List<DataTable?> coursesFromDepts;
    }
    public class AdminNewExam
    {
        private IDBServiceAdmin1 db;
        private int? examId;
        public string? examTitleInput { get; set; }
        public string? semesterOpt { get; set; }
        public string? batchInput { get; set; }
        public int? coordTimeExtentInput { get; set; }
        public int? adviTimeExtentInput { get; set; }
        public DateTime? SelectedDate { get; set; }
        public List<string>? deptOpts {  get; set; }
        public List<DataTable>? coursesFromDepts { get; set; }
        public List<DataTable>? coursesAvailableFromDepts { get; set; }
        public DataTable? semesters { get; set; }
        public DataTable? departments { get; set; }
        public DataTable? courses { get; set; }
        private DataTable? coursesInExam { get; set; }
        private DataTable? savedCoursesInExam { get; set; }

        public AdminNewExam(IDBServiceAdmin1 db, int examId) {
            this.db = db;
            this.examId = examId;
        }
        public AdminNewExam(IDBServiceAdmin1 db)
        {
            this.db = db;
            this.examId = null;
        }

        public async Task getExamDescription()
        {
            // if examId param is not null, load stored ExamDescription keeping in mind any value could be null
            if (this.examId != null) 
            {
                DataTable? examDescription = await db.getExamDescription((int)this.examId);
                if (examDescription != null) {
                    examTitleInput = Convert.ToString(examDescription.Rows[0]["name"]);
                    semesterOpt = Convert.ToString(examDescription.Rows[0]["semester_id"]);
                    batchInput = Convert.ToString(examDescription.Rows[0]["batch"]);
                    coordTimeExtentInput = Convert.ToInt32(examDescription.Rows[0]["coordinator_time_extention"]);
                    adviTimeExtentInput = Convert.ToInt32(examDescription.Rows[0]["advisor_time_extention"]);
                }
            }

            // init for null values
            if (examTitleInput == null) examTitleInput = "";
            if (semesterOpt == null) semesterOpt = "Semester";
            if (batchInput == null) examTitleInput = "";
            if (coordTimeExtentInput == null) examTitleInput = "";
            if (adviTimeExtentInput == null) examTitleInput = "";
        }

        public async Task init()
        {
            await getExamDescription(); // load exam description
            await getDepartments();
            await getSemesters();
            await getCoursesInExam(); // load courses in exam stored so far
            await getCoursesFromDepartments(); // link courses and departments
            setSavedCoursesInExam(); // copy loaded courses as saved courses
            getDeptsAndExams(); // split courses in exam to departments and course tables for displaying
        }

        public async Task getDepartments()
        {
            this.departments = await db.getDepartments();
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
        }

        public async Task getCoursesInExam()
        {
            if (this.examId != null)
            {
                this.coursesInExam = await db.getCoursesInExam((int)this.examId);
            }
        }

        public async Task getCourses()
        { 
            this.courses = await db.getCourses();
        }

        public void getDeptsAndExams()
        {
            if (this.examId != null)
            {
                if (coursesInExam != null)
                {
                    // Initialize the lists
                    deptOpts = new List<string>();
                    coursesFromDepts = new List<DataTable>();

                    // Create a dictionary to hold department IDs and corresponding courses
                    var deptCoursesDict = new Dictionary<string, DataTable>();

                    foreach (DataRow row in coursesInExam.Rows)
                    {
                        string deptId = Convert.ToString(row["dept_id"]);
                        string courseName = Convert.ToString(row["course_name"]);
                        string courseCode = Convert.ToString(row["course_code"]);
                        int coordinatorId = Convert.ToInt32(row["coordinator_id"]);

                        // Check if the department is already in the dictionary
                        if (!deptCoursesDict.ContainsKey(deptId))
                        {
                            // Add department to deptOpts
                            deptOpts.Add(deptId);

                            // Create a new DataTable for this department
                            DataTable deptCoursesTable = new DataTable();
                            deptCoursesTable.Columns.Add("course_name", typeof(string));
                            deptCoursesTable.Columns.Add("course_code", typeof(string));
                            deptCoursesTable.Columns.Add("coordinator_id", typeof(int));

                            // Add the DataTable to the dictionary
                            deptCoursesDict[deptId] = deptCoursesTable;
                        }

                        // Add the course information to the department's DataTable
                        DataRow courseRow = deptCoursesDict[deptId].NewRow();
                        courseRow["course_name"] = courseName;
                        courseRow["course_code"] = courseCode;
                        courseRow["coordinator_id"] = coordinatorId;
                        deptCoursesDict[deptId].Rows.Add(courseRow);
                    }

                    // Add all department course DataTables to the coursesFromDepts list
                    foreach (var deptCourses in deptCoursesDict.Values)
                    {
                        coursesFromDepts.Add(deptCourses);
                    }
                }
            }
        }

        private void setSavedCoursesInExam() {
            savedCoursesInExam = coursesInExam;
        }

        public async Task applyChanges()
        {
            if (coursesInExam == null || savedCoursesInExam == null) return;

            List<int> removeList = new List<int>();
            DataTable updateList = new DataTable();
            updateList.Columns.Add("course_in_exam_id", typeof(int));
            updateList.Columns.Add("coordinator_id", typeof(int));

            DataTable addList = new DataTable();
            addList.Columns.Add("exam_id", typeof(int));
            addList.Columns.Add("course_id", typeof(int));
            addList.Columns.Add("department_id", typeof(int));
            addList.Columns.Add("coordinator_id", typeof(int));

            int savedIndex = 0;
            int currentIndex = 0;

            // Track updates and removals by iterating through the saved and current coursesInExam
            while (savedIndex < savedCoursesInExam.Rows.Count && currentIndex < coursesInExam.Rows.Count)
            {
                DataRow savedRow = savedCoursesInExam.Rows[savedIndex];
                DataRow currentRow = coursesInExam.Rows[currentIndex];

                int savedId = Convert.ToInt32(savedRow["id"]);
                int currentId = Convert.ToInt32(currentRow["id"]);

                if (savedId == currentId)
                {
                    // Check for updates
                    if (Convert.ToInt32(savedRow["coordinator_id"]) != Convert.ToInt32(currentRow["coordinator_id"]))
                    {
                        DataRow updateRow = updateList.NewRow();
                        updateRow["course_in_exam_id"] = currentId;
                        updateRow["coordinator_id"] = Convert.ToInt32(currentRow["coordinator_id"]);
                        updateList.Rows.Add(updateRow);
                    }
                    savedIndex++;
                    currentIndex++;
                }
                else if (savedId < currentId)
                {
                    // Mark for removal
                    removeList.Add(savedId);
                    savedIndex++;
                }
                else
                {
                    // Mark for addition
                    DataRow addRow = addList.NewRow();
                    addRow["exam_id"] = this.examId;
                    addRow["course_id"] = Convert.ToInt32(currentRow["course_id"]);
                    addRow["department_id"] = Convert.ToInt32(currentRow["dept_id"]);
                    addRow["coordinator_id"] = Convert.ToInt32(currentRow["coordinator_id"]);
                    addList.Rows.Add(addRow);
                    currentIndex++;
                }
            }

            // Handle remaining removals
            while (savedIndex < savedCoursesInExam.Rows.Count)
            {
                removeList.Add(Convert.ToInt32(savedCoursesInExam.Rows[savedIndex]["id"]));
                savedIndex++;
            }

            // Handle remaining additions
            while (currentIndex < coursesInExam.Rows.Count)
            {
                DataRow currentRow = coursesInExam.Rows[currentIndex];
                DataRow addRow = addList.NewRow();
                addRow["exam_id"] = this.examId;
                addRow["course_id"] = Convert.ToInt32(currentRow["course_id"]);
                addRow["department_id"] = Convert.ToInt32(currentRow["dept_id"]);
                addRow["coordinator_id"] = Convert.ToInt32(currentRow["coordinator_id"]);
                addList.Rows.Add(addRow);
                currentIndex++;
            }

            // await saveChanges(this.examId.Value, null, null, null, null, null, removeList, updateList.Rows.Count > 0 ? updateList : null, addList.Rows.Count > 0 ? addList : null);
            setSavedCoursesInExam();
        }

        public void addCourse(int deptId, int courseId) {
            if (deptOpts == null)
            {
                deptOpts = new List<string>();
                deptOpts.Add(deptId.ToString());

            }
            else if (!deptOpts.Contains(deptId.ToString()))
            {
                deptOpts.Add(deptId.ToString());
            }
            else {
                if(coursesInExam == null) { 
                    coursesInExam = new DataTable();
                    coursesInExam.Columns.Add("dept_id", typeof(string));
                    coursesInExam.Columns.Add("course_code", typeof(string));
                    coursesInExam.Columns.Add("course_name", typeof(string));
                    coursesInExam.Columns.Add("coordinator_id", typeof(int));
                }

                DataRow newCourse = coursesInExam.NewRow();
                newCourse["dept_id"] = deptId.ToString();

                bool found = false;
                foreach (DataRow row in courses.Rows)
                {
                    if (Convert.ToString(row["id"]) == courseId.ToString())
                    {
                        newCourse["course_code"] = row["code"];
                        newCourse["course_name"] = row["name"];
                        found = true;
                    }
                }
                if (!found) { throw new MissingFieldException($"The specified course with id = {courseId} was missing"); }
            }
        }
    }
}
