using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Pages.Admin
{
    public class StudentsModel : PageModel
    {
        private readonly IStudentService _studentService;

        public List<StudentManagement.Models.Student> Students { get; set; } = new();

        [BindProperty]
        public StudentManagement.Models.Student Student { get; set; } = new();

        public string? Message { get; set; }
        public string? SearchKeyword { get; set; }

        public StudentsModel(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> OnGetAsync(string? search)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                SearchKeyword = search;
                Students = await _studentService.SearchAsync(search);
            }
            else
            {
                Students = await _studentService.GetAllAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            await _studentService.DeleteAsync(id);
            Message = "Xóa sinh viên thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _studentService.UpdateAsync(Student);
            Message = success ? "Cập nhật thành công!" : "Cập nhật thất bại!";
            return RedirectToPage();
        }
    }
}