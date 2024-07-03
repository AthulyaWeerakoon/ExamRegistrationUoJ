using ExamRegistrationUoJ.Services.DBInterfaces;
using System.Data;

namespace ExamRegistrationUoJ.PageClasses.AdminPages
{
    public class RegistrationForm
    {
        private int exam_id;
        private int student_id;

        private string fullName;
        private string semester;
        private string examName;
        private string enumber;
        private DataTable studentCourses;
        private IDBRegistrationFetchService db;

        public RegistrationForm(int exam_id, int student_id, IDBRegistrationFetchService db) { 
            this.db = db;
            this.exam_id = exam_id;
            this.student_id = student_id;
        }

        public async Task init()
        {
            DataTable regDescription = await db.getRegDescription(exam_id, student_id);
            this.fullName = Convert.ToString(regDescription.Rows[0]["name"]);
            this.semester = Convert.ToString(regDescription.Rows[0]["semester"]);
            this.examName = Convert.ToString(regDescription.Rows[0]["exam_name"]);
            this.enumber = Convert.ToString(regDescription.Rows[0]["email"]).Split('@')[0].ToUpper();

            this.studentCourses = await db.getRegCourses(exam_id, student_id);
        }
    }
}
