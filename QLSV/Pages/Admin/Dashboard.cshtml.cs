using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Services;

namespace StudentManagement.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }

        public DashboardModel(IStudentService studentService, ICourseService courseService)
        {
            _studentService = studentService;
            _courseService = courseService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            var students = await _studentService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();

            TotalStudents = students.Count;
            TotalCourses = courses.Count;
            ActiveCourses = courses.Count(c => c.IsActive);

            return Page();
        }
    }
}