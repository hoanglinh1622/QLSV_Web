using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Pages.Student
{
    public class MyCoursesModel : PageModel
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public List<CourseEnrollmentViewModel> EnrolledCourses { get; set; } = new();
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public MyCoursesModel(
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

            await LoadEnrolledCoursesAsync(student.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostUnenrollAsync(string enrollmentId)
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
                var success = await _enrollmentService.UnenrollAsync(enrollmentId);
                if (success)
                {
                    Message = "H?y ??ng ký khóa h?c thành công!";
                }
                else
                {
                    ErrorMessage = "Không th? h?y ??ng ký khóa h?c";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"L?i: {ex.Message}";
            }

            await LoadEnrolledCoursesAsync(student.Id);
            return Page();
        }

        private async Task LoadEnrolledCoursesAsync(string studentId)
        {
            var enrollments = await _enrollmentService.GetByStudentIdAsync(studentId);
            var allCourses = await _courseService.GetAllAsync();

            EnrolledCourses = enrollments.Select(e =>
            {
                var course = allCourses.FirstOrDefault(c => c.Id == e.CourseId);
                return new CourseEnrollmentViewModel
                {
                    Enrollment = e,
                    Course = course!
                };
            }).Where(x => x.Course != null).ToList();
        }
    }

    public class CourseEnrollmentViewModel
    {
        public Enrollment Enrollment { get; set; } = new();
        public Course Course { get; set; } = new();
    }
}