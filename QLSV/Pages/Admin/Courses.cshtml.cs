using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Models;
using StudentManagement.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Pages.Admin
{
    public class CoursesModel : PageModel
    {
        private readonly string courseFile = "Data/courses.csv";
        private readonly IEnrollmentService _enrollmentService;

        public CoursesModel(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        public List<Course> Courses { get; set; } = new();

        [BindProperty]
        public Course NewCourse { get; set; } = new();

        [TempData]
        public string Message { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            Courses = LoadCourses();
        }

        // 🟢 Thêm khóa học mới
        public IActionResult OnPostAddCourse()
        {
            try
            {
                var courses = LoadCourses();

                NewCourse.Id = Guid.NewGuid().ToString();
                NewCourse.EnrolledStudents = 0;
                NewCourse.IsActive = true;
                NewCourse.IsCompleted = false;

                courses.Add(NewCourse);
                SaveCourses(courses);

                Message = $"✅ Đã thêm khóa học thành công!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"❌ Lỗi khi thêm khóa học: {ex.Message}";
                Courses = LoadCourses();
                return Page();
            }
        }
    
        public IActionResult OnPostUpdateCourse()
        {
            var courses = LoadCourses();
            var existingCourse = courses.FirstOrDefault(c => c.Id == NewCourse.Id);

            if (existingCourse != null)
            {
                existingCourse.CourseName = NewCourse.CourseName;
                existingCourse.CourseCode = NewCourse.CourseCode;
                existingCourse.Description = NewCourse.Description;
                existingCourse.Credits = NewCourse.Credits;
                existingCourse.Instructor = NewCourse.Instructor;
                existingCourse.MaxStudents = NewCourse.MaxStudents;
                existingCourse.StartDate = NewCourse.StartDate;
                existingCourse.EndDate = NewCourse.EndDate;

                SaveCourses(courses);
                Message = $"✅ Đã cập nhật khóa học '{NewCourse.CourseName}' thành công!";
            }

            Courses = LoadCourses();
            return Page();
        }



        // 🟡 Bật/tắt trạng thái hoàn thành (IsCompleted)
        public async Task<IActionResult> OnPostToggleStatus(string id)
        {
            var courses = LoadCourses();
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course != null)
            {
                course.IsCompleted = !course.IsCompleted;
                SaveCourses(courses);

                await _enrollmentService.UpdateStatusByCourseAsync(course.Id, course.IsCompleted);

                Message = course.IsCompleted
                    ? $"Khóa học '{course.CourseName}' đã được đánh dấu hoàn thành."
                    : $"Khóa học '{course.CourseName}' được đặt lại thành chưa hoàn thành.";
            }

            return RedirectToPage(new { Message });
        }

        // 🧩 Đọc dữ liệu từ CSV
        private List<Course> LoadCourses()
        {
            var list = new List<Course>();
            if (!System.IO.File.Exists(courseFile))
                return list;

            var lines = System.IO.File.ReadAllLines(courseFile).Skip(1);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length < 12) continue;

                list.Add(new Course
                {
                    Id = parts[0],
                    CourseName = parts[1],
                    CourseCode = parts[2],
                    Description = parts[3],
                    Credits = int.Parse(parts[4]),
                    Instructor = parts[5],
                    MaxStudents = int.Parse(parts[6]),
                    EnrolledStudents = int.Parse(parts[7]),
                    StartDate = DateTime.Parse(parts[8]),
                    EndDate = DateTime.Parse(parts[9]),
                    IsActive = bool.Parse(parts[10]),
                    IsCompleted = bool.Parse(parts[11])
                });
            }

            return list;
        }

        // 💾 Ghi dữ liệu xuống CSV
        private void SaveCourses(List<Course> courses)
        {
            var lines = new List<string>
            {
                "Id,CourseName,CourseCode,Description,Credits,Instructor,MaxStudents,EnrolledStudents,StartDate,EndDate,IsActive,IsCompleted"
            };

            foreach (var c in courses)
            {
                lines.Add($"{c.Id},{c.CourseName},{c.CourseCode},{c.Description},{c.Credits},{c.Instructor},{c.MaxStudents},{c.EnrolledStudents},{c.StartDate},{c.EndDate},{c.IsActive},{c.IsCompleted}");
            }

            System.IO.File.WriteAllLines(courseFile, lines);
        }
    }
}
