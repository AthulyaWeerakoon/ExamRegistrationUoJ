using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.Identity.Client;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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
        public DataTable? departments { get; set; }
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
            await splitDeptsAndCourses(); // split courses in exam to departments and course tables for displaying
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
                    examTitleInput = (examDescription.Rows[0]["name"] == DBNull.Value) ? null:  Convert.ToString(examDescription.Rows[0]["name"]);
                    semesterOpt = (examDescription.Rows[0]["semester_id"] == DBNull.Value) ? null : Convert.ToString(examDescription.Rows[0]["semester_id"]);
                    batchInput = (examDescription.Rows[0]["batch"] == DBNull.Value) ? null : Convert.ToString(examDescription.Rows[0]["batch"]);
                    coordTimeExtentInput = (examDescription.Rows[0]["coordinator_approval_extension"] == DBNull.Value) ? null : Convert.ToInt32(examDescription.Rows[0]["coordinator_approval_extension"]);
                    adviTimeExtentInput = (examDescription.Rows[0]["advisor_approval_extension"] == DBNull.Value) ? null : Convert.ToInt32(examDescription.Rows[0]["advisor_approval_extension"]);
                    SelectedDate = (examDescription.Rows[0]["end_date"] == DBNull.Value) ? null : Convert.ToDateTime(examDescription.Rows[0]["end_date"]);
                }
            }

            // init for null values
            if (examTitleInput == null) examTitleInput = "";
            if (semesterOpt == null) semesterOpt = "Semester";
            if (batchInput == null) batchInput = "";
        }

        public string? areReqsForASaveMet()
        {
            string namePattern = @"^[a-zA-Z\s]+$";
            string batchPattern = @"^[A-Za-z] \d{2}$";

            if (examTitleInput == "") return "Exam title must not be empty";
            if (batchInput == "") return "Exam batch must not be empty. It must be a value with one letter and two following numbers.";
            if (semesterOpt == "Semester") return "A semester must be selected.";

            if (!Regex.IsMatch(examTitleInput, namePattern))
            {
                return "Invalid exam title. No special charcters are allowed.";
            }

            if (!Regex.IsMatch(batchInput, batchPattern))
            {
                return "Invalid batch. It must be a value with one letter and two following numbers.";
            }
            return null;
        }

        public string? regexExamDescription() {
            string? basicReqError = areReqsForASaveMet();
            if (basicReqError != null) return basicReqError;

            if (coordTimeExtentInput != null) if (coordTimeExtentInput > 16 || coordTimeExtentInput < 1) return "Time extension for coordinator approval must be a value between 1 to 16.";

            if (adviTimeExtentInput != null) if (adviTimeExtentInput > 16 || adviTimeExtentInput < 1) return "Time extension for advisor approval must be a value between 1 to 16.";

            if (SelectedDate != null) if (DateTime.Compare((DateTime)SelectedDate, DateTime.Now) <= 0) return "Cannot select Today or a Past date as the registration closing date.";

            return null;
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
            // a null error can be caused here
        }

        public async Task getCoordinators() 
        {
            this.coordinators = await db.getCoordinators();

            if (this.coordinators is null) {
                this.coordinators = new DataTable();
                this.coordinators.Columns.Add("id", typeof(uint));
                this.coordinators.Columns.Add("email", typeof(string));
            }
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
            deptCoursesTable.Columns.Add("coordinator_email", typeof(string));
            return deptCoursesTable;
        }

        private async Task<List<KeyValuePair<int, string>>> GetCourseIdsByDept(int departmentId) {

            DataTable coursesForDept = await db.getCoursesFromDepartment(departmentId);

            var resultCourseIds = coursesForDept.AsEnumerable()
                                               .Select(row => new KeyValuePair<int, string>(Convert.ToInt32(row["course_id"]), Convert.ToString(row["course_code"])))
                                               .ToList();

            return resultCourseIds;
        }

        public async Task splitDeptsAndCourses()
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
                    Object coordinatorId = row["coordinator_id"];

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
                    courseRow["coordinator_id"] = (coordinatorId == DBNull.Value) ? DBNull.Value : Convert.ToInt32(coordinatorId);
                    courseRow["coordinator_email"] = (coordinatorId == DBNull.Value) ? DBNull.Value : getCoordEmailFromId(Convert.ToInt32(coordinatorId));
                    deptCoursesDict[deptId].Rows.Add(courseRow);
                    i++;
                }

                // Add all department course DataTables to the coursesFromDepts list
                foreach (var deptCourses in deptCoursesDict)
                {
                    coursesFromDepts.Add(deptCourses.Value); // init each courses tables
                    deptOpts.Add(deptCourses.Key); // adds selection to dept opts
                    coursesAvailableFromDepts.Add(await GetCourseIdsByDept(int.Parse(deptCourses.Key)));
                }
            }
            // If no data is loaded, the lists are initialized to the defaults
            if (coursesFromDepts.Count < 1)
            {
                deptOpts.Add(null);
                coursesAvailableFromDepts.Add(null);
                coursesFromDepts.Add(null);
            }
        }

        public string? getDeptFromId(int id) {
            return Convert.ToString(departments.AsEnumerable().FirstOrDefault(row => Convert.ToInt32(row["id"]) == id)["name"]);
        }

        public string? getCoordEmailFromId(int id)
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

        public async Task applyChanges()
        {
            if (areReqsForASaveMet() != null) throw new InvalidOperationException("Exam description is not completed correctly");

            // Save exam description and get exam_id
            int? id = await db.addOrSaveExamDescription(this.examId,
                (this.examTitleInput == "") ? null : this.examTitleInput,
                (this.semesterOpt == "Semester") ? null : int.Parse(this.semesterOpt),
                (this.batchInput == "") ? null : this.batchInput,
                this.coordTimeExtentInput,
                this.adviTimeExtentInput);

            examId = (id is null)? examId : id;

            Console.WriteLine("Got here 1");
            if (coursesInExam == null && savedCoursesInExam == null) return;
            Console.WriteLine("Got here 2");

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


            Console.WriteLine("Got here 3");

            // await db.saveCourseChanges((int)examId,
            //    removeList.Count > 0 ? removeList : null, 
            //    updateList.Rows.Count > 0 ? updateList : null, 
            //    addList.Rows.Count > 0 ? addList : null);

            setSavedCoursesInExam();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(addList));
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(removeList));
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(updateList));
        }

        public void addDepartment() {
            if (deptOpts.Contains(null)) return;

            deptOpts.Add(null);
            coursesFromDepts.Add(null);
            coursesAvailableFromDepts.Add(null);
        }

        public async Task setDepartment(int idx, string deptOpt) {
            if (coursesFromDepts[idx] != null) throw new InvalidOperationException("Can't change department if courses are added");

            deptOpts[idx] = deptOpt;
            coursesAvailableFromDepts[idx] = await GetCourseIdsByDept(Convert.ToInt32(deptOpt));
        }

        public void addCourse(int deptIdx, int deptId, int courseId) {
            if (deptOpts[deptIdx] == null) throw new InvalidOperationException("Can't add course if the department is not selected");

            // Remove Course From selection
            coursesAvailableFromDepts[deptIdx] = coursesAvailableFromDepts[deptIdx].Where(kvp => kvp.Key != courseId).ToList();

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
            coursesFromDepts[deptIdx].Rows.Add(courseRow);
        }


        public int? doesCoordExist(string email) { 
            if(this.coordinators is null) return null;

            foreach (DataRow coord in coordinators.Rows) {
                if (Convert.ToString(coord["email"]) == email) return Convert.ToInt32(coord["id"]);
            }

            return null;
        }

        public async Task addNewCoordToCourse(DataRow courseRow, string email)
        {
            // Assert coord does not exist
            int coordId = await db.addCoordinator(email);

            // Add to coordinator list
            DataRow newRow = this.coordinators.NewRow();
            newRow["id"] = coordId;
            newRow["email"] = email;
            this.coordinators.Rows.Add(newRow);

            // Add to the view
            courseRow["coordinator_id"] = coordId;
            courseRow["coordinator_email"] = getCoordEmailFromId(coordId);
            coursesInExam.Rows[Convert.ToInt32(courseRow["idx"])]["coordinator_id"] = coordId;
        }

        public void removeCourse(int deptIdx, int rowIdx, int CIEIdx)
        {
            // Add removed course back into selection
            coursesAvailableFromDepts[deptIdx].Add(new KeyValuePair<int, string>(Convert.ToInt32(coursesFromDepts[deptIdx].Rows[rowIdx]["course_id"]), Convert.ToString(coursesFromDepts[deptIdx].Rows[rowIdx]["course_code"])));

            // remove from display tables and courseInExam table
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
            if (coursesFromDepts.Count < 2)
            {
                deptOpts[deptIdx] = null;
                coursesFromDepts[deptIdx] = null;
                coursesAvailableFromDepts[deptIdx] = null;
            }
            else {
                deptOpts.RemoveAt(deptIdx);
                coursesFromDepts.RemoveAt(deptIdx);
                coursesAvailableFromDepts.RemoveAt(deptIdx);
            }
        }

        public bool isExamCompleted() {
            if (regexExamDescription() is not null) return false;
            if (deptOpts.Contains(null) && coursesFromDepts.Contains(null)) return false;
            if (coursesFromDepts.Any(dept => dept.AsEnumerable().Any(row => row["coordinator_id"] == DBNull.Value)))
            {
                return false;
            }
            return true;
        }

        public async Task confirmExam() 
        {
            if (!isExamCompleted()) throw new InvalidOperationException("Exam creation form is not complete.");

            await applyChanges();
            await db.finalizeExam((int)this.examId);
        }
    }
}
