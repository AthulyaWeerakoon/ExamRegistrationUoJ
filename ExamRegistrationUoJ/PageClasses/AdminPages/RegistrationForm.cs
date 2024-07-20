using DinkToPdf;
using ExamRegistrationUoJ.Services.DBInterfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Http.Headers;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace ExamRegistrationUoJ.PageClasses.AdminPages
{
    public class RegistrationForm
	{
		private readonly IWebHostEnvironment environment;
		private readonly IDBRegistrationFetchService db;
		private readonly NavigationManager navigationManager;

		private int exam_id;
        private int student_id;

        private string fullName;
        private string semester;
        private string examName;
        private string enumber;
        private DataTable studentCourses;

        private Document? RegForm = null;
		private bool appended = false;

        public RegistrationForm(int exam_id, int student_id, IDBRegistrationFetchService db, IWebHostEnvironment environment, NavigationManager navigationManager) { 
            this.db = db;
            this.exam_id = exam_id;
            this.student_id = student_id;
			this.environment = environment;
			this.navigationManager = navigationManager;
		}

        public int min(int a, int b) { 
            return (a > b)? b: a;
        }

        public async Task init()
        {
            DataTable regDescription = await db.getRegDescription(exam_id, student_id);
            this.fullName = Convert.ToString(regDescription.Rows[0]["name"]);
            this.semester = Convert.ToString(regDescription.Rows[0]["semester"]);
            this.examName = Convert.ToString(regDescription.Rows[0]["exam_name"]);
            this.enumber = Convert.ToString(regDescription.Rows[0]["email"]).Split('@')[0].ToUpper();

            this.studentCourses = await db.getRegCourses(exam_id, student_id);

			await updateDoc();
        }

        private async Task updateDoc()
        {
            if (RegForm is null && !appended)
            {
                string loadPath = $"{navigationManager.BaseUri}/Assets/Docs/Exam-Enry-Form-form-for-General-program.docx";

                RegForm = DocX.Load(loadPath);

                // Create a copy of the original document for filling in each page
                Document template = RegForm.Copy();

                // Load QR code
                byte[] imageData = await DownloadImageAsync($"https://quickchart.io/qr?text={Uri.EscapeDataString($"{navigationManager.BaseUri}admin-print-view/{exam_id}/{student_id}")}");

                Document RegFormFilled = RegForm.Copy();
                int pageCount = (int)Math.Ceiling((double)studentCourses.Rows.Count / 6);
                for (int i = 0; i < pageCount; i++)
                {
                    Document newCopy = template.Copy();
                    newCopy.Bookmarks["examname"].Paragraph.Append(examName);
                    newCopy.Bookmarks["fullname"].Paragraph.Append(fullName);
                    newCopy.Bookmarks["regno"].Paragraph.Append(enumber);
                    newCopy.Bookmarks["semester"].Paragraph.Append(semester);

                    // Add anti-forgery QR
                    InsertImageAtBookmark("verifyqr", imageData, newCopy);

                    Table courseTable = newCopy.Tables[1];
                    for (int entry = i * 6; entry < Math.Min(studentCourses.Rows.Count, (i + 1) * 6); entry++)
                    {
                        DataRow row = studentCourses.Rows[entry];

                        // Create a new row
                        var newRow = courseTable.InsertRow();
                        newRow.Height = 12 * 2.835; // Set row height to 12 mm (1 point = 0.3528 mm, hence 12 mm ≈ 34 points)

                        for (int cellIndex = 0; cellIndex < newRow.Cells.Count; cellIndex++)
                        {
                            var cell = newRow.Cells[cellIndex];
                            cell.Paragraphs[0].FontSize(12); // Set font size to 12
                            cell.MarginTop = 2 * 2.835; // Set cell top margin to 0.25 mm
                            cell.MarginBottom = 2 * 2.835; // Set cell bottom margin to 0.25 mm
                            cell.MarginLeft = 2 * 2.835; // Set cell left margin to 0.25 mm
                            cell.MarginRight = 2 * 2.835; // Set cell right margin to 0.25 mm
                            cell.VerticalAlignment = VerticalAlignment.Center;
                        }

                        // Populate the row with data
                        newRow.Cells[0].Paragraphs[0].Append(row["code"].ToString());
                        newRow.Cells[1].Paragraphs[0].Append(row["name"].ToString());

                        // is_proper column
                        var isProper = row["is_proper"].ToString() == "1" ? "Proper" : "Repeat";
                        newRow.Cells[2].Paragraphs[0].Append(isProper);
                        newRow.Cells[2].Paragraphs[0].Alignment = Alignment.center;

                        newRow.Cells[3].Paragraphs[0].Append(row["attendance"].ToString());
                        newRow.Cells[3].Paragraphs[0].Alignment = Alignment.center;

                        // is_approved column
                        var isApproved = row["is_approved"].ToString() == "1" ? "Approved" : "Not Approved";
                        newRow.Cells[4].Paragraphs[0].Append(isApproved);
                        newRow.Cells[4].Paragraphs[0].Alignment = Alignment.center;
                    }

                    if (i > 0)
                    {
                        // Insert a page break before adding new content
                        newCopy.InsertParagraph().InsertPageBreakAfterSelf();
                        RegFormFilled.InsertDocument(newCopy);
                    }
                    else
                    {
                        RegFormFilled = newCopy;
                    }
                }

                RegForm = RegFormFilled;
            }
            else if (appended)
            {
                throw new InvalidOperationException("Cannot update an appended registration form that contains multiple forms");
            }
        }

        public void addFirstPage() {
			if (RegForm is not null)
			{
				RegForm.Paragraphs[0].InsertPageBreakBeforeSelf();
			}
		}

        public void InsertImageAtBookmark(string bookmarkName, byte[] imageData, Document document)
        {
            if (imageData != null)
            {
                // Find the bookmark
                var bookmark = document.Bookmarks[bookmarkName];

                if (bookmark != null)
                {
                    // Add the image to the document from a stream
                    using (var imageStream = new MemoryStream(imageData))
                    {
                        var image = document.AddImage(imageStream);
                        var picture = image.CreatePicture(100, 100);

                        // Insert the image at the bookmark
                        bookmark.Paragraph.InsertPicture(picture, 0);
                    }
                }
                else
                {
                    throw new InvalidExpressionException($"Bookmark '{bookmarkName}' not found.");
                }
            }
            else
            {
                throw new InvalidOperationException("Failed to download image.");
            }
        }

        private async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    return await httpClient.GetByteArrayAsync(imageUrl);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading image: {ex.Message}");
                    return null;
                }
            }
        }

		public void append(RegistrationForm regForm)
		{
			RegForm.InsertDocument(regForm.RegForm);
			appended = true;
		}

		private string saveFormAsDocx() {
			int i = 0;

			if(RegForm is null) { throw new NullReferenceException($"Registration form for exam of id {exam_id} has not been initialized"); }

			string savePath = $"/Assets/Docs/Exam_entry_form" + ((appended)? "s" : "");
			
			while (true)
			{   // Check for conflicts
				string newPath = savePath + ((i == 0) ? "" : $"_{i.ToString()}") + ".docx";
				if (File.Exists(environment.WebRootPath + newPath)) {
					// If the old file is more than an hour old, delete it
					if (DateTime.Compare((new FileInfo(environment.WebRootPath + newPath)).CreationTime.AddHours(1), DateTime.Now) < 0)
					{
						removeFile(environment.WebRootPath + newPath);
						savePath = newPath;
						break;
					}
					else {
						i++;
					}
				}
				else
				{
					savePath = newPath;
					break;
				}
			}

			RegForm.SaveAs(environment.WebRootPath + savePath);
			return navigationManager.BaseUri + savePath;
		}

		private void removeFile(string path)
		{
			if (File.Exists(path)) { File.Delete(path); }
			else { throw new FileNotFoundException($"File {path} to remove was not found"); }
		}

		public string downloadForm() {
			return saveFormAsDocx();
		}
	}
}
