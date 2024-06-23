using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Identity.Client;
using System.Collections;
using System.Data;
using System.Linq;

namespace AdminPages
{
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
        public List<DataTable?>? coursesFromDepts { get; set; }
        public List<List<KeyValuePair<int, string>>?>? coursesAvailableFromDepts { get; set; }
        public DataTable? semesters { get; set; }
        private DataTable? departments { get; set; }
        public DataTable? departmentSelect { get; set; }
        private DataTable? courses { get; set; }
        private DataTable? coursesInExam { get; set; }
        private DataTable? savedCoursesInExam { get; set; }
        public DataTable? coordinators { get; set; }

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
            await getCourses();
            await getCoordinators();
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
                if(await db.isExamFinalized(Convert.ToInt32(this.examId))) { this.examId = null; }
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
            // a null error can be caused here
        }

        public async Task getSemesters()
        {
            this.semesters = await db.getSemesters();
            // a null error can be caused here
        }

        public async Task getCourses()
        {
            this.courses = await db.getAllCourses();
        }

        public async Task getCoordinators() 
        {
            this.coordinators = await db.getCoordinators();
        }

        public DataTable newCoursesInExamTable()
        {
            coursesInExam = new DataTable();
            coursesInExam.Columns.Add("dept_id", typeof(string));
            coursesInExam.Columns.Add("course_id", typeof(string));
            coursesInExam.Columns.Add("course_code", typeof(string));
            coursesInExam.Columns.Add("course_name", typeof(string));
            coursesInExam.Columns.Add("coordinator_id", typeof(int));
            return coursesInExam;
        }

        public async Task getCoursesInExam()
        {
            if (this.examId != null)
            {
                this.coursesInExam = await db.getCoursesInExam((int)this.examId);
            }
            else
            {
                this.coursesInExam = null;
            }

            // init if null
            if (this.coursesInExam == null)
            {
                this.coursesInExam = newCoursesInExamTable();
            }
        }

        private void initAvailableCoursesInDepartments()
        {
            this.coursesAvailableFromDepts = new List<List<KeyValuePair<int, string>>>();
        }

        public DataTable newDeptCoursesTable()
        {
            DataTable deptCoursesTable = new DataTable();
            deptCoursesTable.Columns.Add("id", typeof(uint));
            deptCoursesTable.Columns.Add("idx", typeof(uint)); // indexing for accessing relevant course in exam rows
            deptCoursesTable.Columns.Add("course_id", typeof(uint));
            deptCoursesTable.Columns.Add("course_name", typeof(string));
            deptCoursesTable.Columns.Add("course_code", typeof(string));
            deptCoursesTable.Columns.Add("coordinator_id", typeof(int));
            return deptCoursesTable;
        }

        private List<KeyValuePair<int, string>> GetCourseIdsByDept(int departmentId) {
            if (courses == null || courses.Rows.Count == 0 || coursesInExam == null || coursesInExam.Rows.Count == 0)
            {
                return new List<KeyValuePair<int, string>>();
            }

            var courseIds = courses.AsEnumerable()
                           .Select(row => Convert.ToInt32(row["id"]))
                           .ToHashSet();

            var resultCourseIds = coursesInExam.AsEnumerable()
                                               .Where(row => Convert.ToInt32(row["dept_id"]) == departmentId &&
                                                             courseIds.Contains(Convert.ToInt32(row["course_code"])))
                                               .Select(row => new KeyValuePair<int, string>(Convert.ToInt32(row["id"]), Convert.ToString(row["course_id"])))
                                               .ToList();
            
            return resultCourseIds;
        }

        public void splitDeptsAndCourses()
        {
            deptOpts = new List<string?>();
            coursesAvailableFromDepts = new List<List<KeyValuePair<int, string>>?>();
            coursesFromDepts = new List<DataTable>();

            if (this.examId != null && coursesInExam != null)
                {
                // Initialize the lists
                deptOpts = new List<string?>();

                // Create a dictionary to hold department IDs and corresponding courses
                var deptCoursesDict = new Dictionary<string, DataTable>();

                int i = 0;
                foreach (DataRow row in coursesInExam.Rows)
                {
                    string id = Convert.ToString(row["id"]);
                    string deptId = Convert.ToString(row["dept_id"]);
                    string courseId = Convert.ToString(row["course_id"]);
                    string courseName = Convert.ToString(row["course_name"]);
                    string courseCode = Convert.ToString(row["course_code"]);
                    int coordinatorId = Convert.ToInt32(row["coordinator_id"]);

                    // Check if the department is already in the dictionary
                    if (!deptCoursesDict.ContainsKey(deptId))
                    {
                        // Create a new DataTable for this department
                        DataTable deptCoursesTable = newDeptCoursesTable();

                        // Add the DataTable to the dictionary
                        deptCoursesDict[deptId] = deptCoursesTable;
                    }

                    // Add the course information to the department's DataTable
                    DataRow courseRow = deptCoursesDict[deptId].NewRow();
                    courseRow["id"] = id;
                    courseRow["idx"] = i;
                    courseRow["course_id"] = courseId;
                    courseRow["course_name"] = courseName;
                    courseRow["course_code"] = courseCode;
                    courseRow["coordinator_id"] = coordinatorId;
                    deptCoursesDict[deptId].Rows.Add(courseRow);
                    i++;
                }

                // Add all department course DataTables to the coursesFromDepts list
                foreach (var deptCourses in deptCoursesDict)
                {
                    coursesFromDepts.Add(deptCourses.Value); // init each courses tables
                    deptOpts.Add(deptCourses.Key); // adds selection to dept opts
                    coursesAvailableFromDepts.Add(GetCourseIdsByDept(int.Parse(deptCourses.Key)));
                }
            }
            // If no data is loaded, the lists are initialized to the defaults
            else
            {
                deptOpts.Add(null);
                coursesAvailableFromDepts.Add(null);
                coursesFromDepts.Add(null);
            }
        }

        private string? getDeptFromId(int id) {
            return Convert.ToString(departments.AsEnumerable().FirstOrDefault(row => Convert.ToInt32(row["id"]) == id)["name"]);
        }

        private string? getCoordEmailFromId(int id)
        {
            return Convert.ToString(coordinators.AsEnumerable().FirstOrDefault(row => Convert.ToInt32(row["id"]) == id)["email"]);
        }

        private void setSavedCoursesInExam() {
            if(coursesInExam == null)
            {
                coursesInExam = newCoursesInExamTable();
            }
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

        public void addDepartment() {
            if (deptOpts.Contains(null)) return;

            deptOpts.Add(null);
            coursesFromDepts.Add(null);
            coursesAvailableFromDepts.Add(null);
        }

        public void setDepartment(int idx, string deptOpt) {
            if (coursesFromDepts[idx] != null) throw new InvalidOperationException("Can't change department if courses are added");

            deptOpts[idx] = deptOpt;
            coursesAvailableFromDepts.Add(GetCourseIdsByDept(Convert.ToInt32(deptOpt)));
        }

        public void addCourse(int deptIdx, int deptId, int courseId) {
            if (deptOpts[deptIdx] == null) throw new InvalidOperationException("Can't add course if the department is not selected");

            // Add course to coursesInExam
            DataRow newCourse = coursesInExam.NewRow();
            newCourse["dept_id"] = deptId.ToString();

            bool found = false;
            foreach (DataRow row in courses.Rows)
            {
                if (Convert.ToString(row["id"]) == courseId.ToString())
                {
                    newCourse["course_id"] = row["id"];
                    newCourse["course_code"] = row["code"];
                    newCourse["course_name"] = row["name"];
                    found = true;
                    break;
                }
            }
            if (!found) { throw new MissingFieldException($"The specified course with id = {courseId} was missing"); }

            coursesInExam.Rows.Add(newCourse);

            // Add course to coursesFromDepts
            if (coursesFromDepts[deptIdx] is null) {
                coursesFromDepts[deptIdx] = newDeptCoursesTable();
            }

            DataRow courseRow = coursesFromDepts[deptIdx].NewRow(); // no need to include id since it's only required if course in exam is already in the database, but remember to save and quit
            courseRow["idx"] = coursesInExam.Rows.Count - 1;
            courseRow["course_id"] = courseId;
            courseRow["course_name"] = newCourse["course_name"];
            courseRow["course_code"] = newCourse["course_code"];
            coursesFromDepts[deptId].Rows.Add(courseRow);
        }


        public int? doesCoordExist(string email) { 
            if(this.coordinators is null) return null;

            foreach (DataRow coord in coordinators.Rows) {
                if (Convert.ToString(coord["email"]) == email) return Convert.ToInt32(coord["id"]);
            }

            return null;
        }

        public async Task addCoordToCourse(int deptIdx, int rowIdx, int CIEIdx, string email)
        {
            int? coordId = doesCoordExist(email);

            // Add to the database if doesn't exist
            if(coordId == null)
            {
                coordId = await db.addCoordinator(email);
            }

            // Add to the view
            coursesFromDepts[deptIdx].Rows[rowIdx]["coordinator_id"] = coordId;
            coursesInExam.Rows[CIEIdx]["coordinator_id"] = coordId;
        }

        public void removeCourse(int deptIdx, int rowIdx, int CIEIdx)
        {
            coursesInExam.Rows.RemoveAt(CIEIdx);
            coursesFromDepts[deptIdx].Rows.RemoveAt(rowIdx);

            // if coursesFromDepts is empty after
            if (coursesFromDepts[deptIdx].Rows.Count < 1)
            {
                coursesFromDepts[deptIdx] = null;
            }
        }

        public void removeDept(int deptIdx)
        {
            if (coursesFromDepts[deptIdx] != null) throw new InvalidOperationException("Remove all courses from department before attempting to remove the department itself.");

            if (coursesFromDepts.Count < 2)
            {
                coursesFromDepts[deptIdx] = null;
                coursesAvailableFromDepts = null;
            }
            else {
                coursesFromDepts.RemoveAt(deptIdx);
                coursesAvailableFromDepts.RemoveAt(deptIdx);
                coursesFromDepts.RemoveAt(deptIdx);
            }
        }

        public void confirmExam() { 
            
        }
    }
}
