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
        public List<string?>? deptOpts {  get; set; }
        public List<DataTable>? coursesFromDepts { get; set; }
        public List<List<int>>? coursesAvailableFromDepts { get; set; }
        public DataTable? semesters { get; set; }
        private DataTable? departments { get; set; }
        public DataTable? departmentSelect { get; set; }
        public DataTable? courses { get; set; }
        private DataTable? coursesInExam { get; set; }
        private DataTable? savedCoursesInExam { get; set; }

        public AdminNewExam(IDBServiceAdmin1 db, int? examId) {
            this.db = db;
            this.examId = examId;
        }
        public AdminNewExam(IDBServiceAdmin1 db)
        {
            this.db = db;
            this.examId = null;
        }

        public async Task init()
        {
            await checkIsFinalized();
            await getExamDescription(); // load exam description, contains a segment to initialize nullables
            await getDepartments();
            await getSemesters();
            await getCoursesInExam(); // load courses in exam stored so far
            initAvailableCoursesInDepartments(); // set up list to link courses and departments
            setSavedCoursesInExam(); // copy loaded courses as saved courses
            splitDeptsAndCourses(); // split courses in exam to departments and course tables for displaying
            updateDepartmentSelect(); // add only the unselect department options to the department select options
        }

        public async Task checkIsFinalized()
        {
            if (this.examId != null)
            {
                // if(db.isExamFinalized(this.examId)) { this.examId = null; }
            }
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
                    coordTimeExtentInput = Convert.ToInt32(examDescription.Rows[0]["coordinator_approval_extension"]);
                    adviTimeExtentInput = Convert.ToInt32(examDescription.Rows[0]["advisor_approval_extension"]);
                    SelectedDate = Convert.ToDateTime(examDescription.Rows[0]["end_date"]);
                }
            }

            // init for null values
            if (examTitleInput == null) examTitleInput = "";
            if (semesterOpt == null) semesterOpt = "Semester";
            if (batchInput == null) examTitleInput = "";
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
            else
            {
                this.coursesInExam = new DataTable();
                this.coursesInExam.Columns.Add("id", typeof(int));
                this.coursesInExam.Columns.Add("course_name", typeof(string));
                this.coursesInExam.Columns.Add("course_code", typeof(string));
                this.coursesInExam.Columns.Add("coordinator_id", typeof(int));
                this.coursesInExam.Columns.Add("dept_id", typeof(int));
            }
        }

        public async Task getCourses()
        { 
            this.courses = await db.getCourses();
        }

        private void initAvailableCoursesInDepartments()
        {
            this.coursesAvailableFromDepts = new List<List<int>>();
        }

        public DataTable newDeptCoursesTable()
        {
            DataTable deptCoursesTable = new DataTable();
            deptCoursesTable.Columns.Add("id", typeof(uint));
            deptCoursesTable.Columns.Add("course_name", typeof(string));
            deptCoursesTable.Columns.Add("course_code", typeof(string));
            deptCoursesTable.Columns.Add("coordinator_id", typeof(int));
            return deptCoursesTable;
        }

        private List<int> GetCourseIdsByDeptAndSemester(int departmentId, int? semesterId) {
            if (courses == null || courses.Rows.Count == 0 || coursesInExam == null || coursesInExam.Rows.Count == 0)
            {
                return new List<int>();
            }

            IEnumerable<DataRow> filteredCourses;
            if (semesterId.HasValue)
            {
                filteredCourses = courses.AsEnumerable()
                                         .Where(row => Convert.ToInt32(row["semester_id"]) == semesterId.Value);
            }
            else
            {
                filteredCourses = courses.AsEnumerable();
            }

            // Select relevant course IDs
            var courseIdsForSemester = filteredCourses
                                       .Select(row => Convert.ToInt32(row["id"]))
                                       .ToHashSet();

            // Filter coursesInExam by department ID and check if course ID is in the filtered course IDs
            var courseIds = coursesInExam.AsEnumerable()
                                         .Where(row => Convert.ToInt32(row["dept_id"]) == departmentId &&
                                                       courseIdsForSemester.Contains(Convert.ToInt32(row["course_code"])))
                                         .Select(row => Convert.ToInt32(row["id"]))
                                         .ToList();

            return courseIds;
        }

        public void splitDeptsAndCourses()
        {
            if (this.examId != null)
            {
                if (coursesInExam != null)
                {
                    // Initialize the lists
                    deptOpts = new List<string?>();
                    coursesFromDepts = new List<DataTable>();

                    // Create a dictionary to hold department IDs and corresponding courses
                    var deptCoursesDict = new Dictionary<string, DataTable>();

                    foreach (DataRow row in coursesInExam.Rows)
                    {
                        string id = Convert.ToString(row["id"]);
                        string deptId = Convert.ToString(row["dept_id"]);
                        string courseName = Convert.ToString(row["course_name"]);
                        string courseCode = Convert.ToString(row["course_code"]);
                        int coordinatorId = Convert.ToInt32(row["coordinator_id"]);

                        // Check if the department is already in the dictionary
                        if (!deptCoursesDict.ContainsKey(deptId))
                        {
                            // Add department to deptOpts
                            deptOpts.Add(deptId);
                            coursesAvailableFromDepts.Add(GetCourseIdsByDeptAndSemester(int.Parse(deptId), (semesterOpt=="Semester")? null: int.Parse(semesterOpt)));

                            // Create a new DataTable for this department
                            DataTable deptCoursesTable = newDeptCoursesTable();

                            // Add the DataTable to the dictionary
                            deptCoursesDict[deptId] = deptCoursesTable;
                        }

                        // Add the course information to the department's DataTable
                        DataRow courseRow = deptCoursesDict[deptId].NewRow();
                        courseRow["id"] = id;
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
            // If no data is loaded, the lists are initialized to the defaults
            else
            {
                deptOpts = new List<string?> { null };
                coursesFromDepts = new List<DataTable>();
                DataTable defaultEmptyTable = newDeptCoursesTable();
                coursesFromDepts.Add(defaultEmptyTable);
            }
        }

        private void setSavedCoursesInExam() {
            savedCoursesInExam = coursesInExam.Copy();
        }

        private void updateDepartmentSelect() {
            if (departments == null)
            {
                return;
            }

            if (deptOpts == null) {
                deptOpts = new List<string?>();
            }

            // Initialize departmentSelect with the same structure as departments
            departmentSelect = departments.Clone();

            // Convert deptOpts to a HashSet for efficient lookup
            HashSet<string?> deptOptsSet = new HashSet<string?>(deptOpts);

            // Filter the departments and add to departmentSelect
            foreach (DataRow row in departments.Rows)
            {
                if ((row["id"].ToString() != null)? !deptOptsSet.Contains(row["id"].ToString()) : true)
                {
                    departmentSelect.ImportRow(row);
                }
            }
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

            await db.saveChanges(this.examId,
                (this.examTitleInput == "") ? null : this.examTitleInput,
                (this.semesterOpt == "Semester")? null: int.Parse(this.semesterOpt),
                (this.batchInput == "") ? null : this.batchInput,
                this.coordTimeExtentInput,
                this.adviTimeExtentInput,
                removeList.Count > 0 ? removeList : null, 
                updateList.Rows.Count > 0 ? updateList : null, 
                addList.Rows.Count > 0 ? addList : null);
            setSavedCoursesInExam();
        }

        public void addCourse(int deptId, int courseId) {
            if (deptOpts == null)
            {
                deptOpts = [deptId.ToString()];
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
