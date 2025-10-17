using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Pages.Student
{
    public class CoursesModel : PageModel
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public List<CourseViewModel> Courses { get; set; } = new();
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public CoursesModel(
            IStudentService studentService,
            ICourseService courseService,
            IEnrollmentService enrollmentService)
        {
            _studentService = studentService;
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                return RedirectToPage("/Login");
            }

            await LoadCoursesAsync(student.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostEnrollAsync(string courseId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                return RedirectToPage("/Login");
            }

            try
            {
                await _enrollmentService.EnrollAsync(student.Id, courseId);
                Message = " Đăng kí khóa học thành công!  ";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            await LoadCoursesAsync(student.Id);
            return Page();
        }

        private async Task LoadCoursesAsync(string studentId)
        {
            var allCourses = await _courseService.GetActiveCoursesAsync();
            var enrollments = await _enrollmentService.GetByStudentIdAsync(studentId);

            Courses = allCourses.Select(c => new CourseViewModel
            {
                Course = c,
                IsEnrolled = enrollments.Any(e => e.CourseId == c.Id)
            }).ToList();
        }
    }

    public class CourseViewModel
    {
        public Course Course { get; set; } = new();
        public bool IsEnrolled { get; set; }
        public bool IsCompleted { get; set; }
    }
}