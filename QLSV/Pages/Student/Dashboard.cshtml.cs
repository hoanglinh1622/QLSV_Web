// Pages/Student/Dashboard.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Pages.Student
{
    public class DashboardModel : PageModel
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public StudentManagement.Models.Student? Student { get; set; }
        public List<Course> AvailableCourses { get; set; } = new();

        public DashboardModel(IStudentService studentService, ICourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Login");
            }

            Student = await _studentService.GetByUserIdAsync(userId);
            AvailableCourses = await _courseService.GetActiveCoursesAsync();

            return Page();
        }
    }
}
