using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Data;

public class StudentHomePage
{
    private readonly IDBServiceStudentHome db;
    private readonly int studentId;

    public DataTable departments { get; private set; }
    public DataTable semesters { get; private set; }
    public DataTable displayExams { get; set; }
    public DataTable registeredExams { get; private set; }


    // Dictionary to store courses for each exam
    public Dictionary<int, DataTable> examCourses { get; private set; } = new Dictionary<int, DataTable>();

    public string department { get; set; }
    public string semester { get; set; }
    public string status { get; set; }

    public StudentHomePage(IDBServiceStudentHome db, int studentId)
    {
        this.db = db;
        this.studentId = studentId;
    }

    public async Task Init()
    {
        try
        {
            departments = await db.getDepartments();
            semesters = await db.getSemesters();
            registeredExams = await db.getRegisteredExams(studentId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to initialize page: {ex.Message}");
        }
    }

    public async Task filterExam()
    {
        try
        {
            int departmentId = string.IsNullOrEmpty(department) ? 0 : int.Parse(department);
            int semesterId = string.IsNullOrEmpty(semester) ? 0 : int.Parse(semester);
            int statusId = string.IsNullOrEmpty(status) ? 0 : int.Parse(status);

            displayExams = await db.getFilteredExams(departmentId, semesterId, statusId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to filter exams: {ex.Message}");
        }
    }

    public async Task<bool> registerForExam(int examId)
    {
        try
        {
            return await db.registerForExam(studentId, examId);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to register for exam: {ex.Message}");
        }
    }

    public async Task getCoursesForExam(int examId)
    {
        try
        {
            if (!examCourses.ContainsKey(examId))
            {
                DataTable courses = await db.getCoursesForExam(examId);
                examCourses[examId] = courses;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to fetch courses for the exam: {ex.Message}");
        }
    }
}
