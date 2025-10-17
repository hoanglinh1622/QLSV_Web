using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-20);

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public RegisterModel(IUserService userService, IStudentService studentService)
        {
            _userService = userService;
            _studentService = studentService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin";
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp";
                return Page();
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự";
                return Page();
            }

            // Create user account
            var user = await _userService.RegisterAsync(Username, Password, "Student");

            if (user == null)
            {
                ErrorMessage = "Tên đăng nhập đã tồn tại";
                return Page();
            }

            // Create student profile
            var student = new StudentManagement.Models.Student
            {
                UserId = user.Id,
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                DateOfBirth = DateOfBirth,
                Address = Address,
                StudentCode = GenerateStudentCode()
            };

            await _studentService.CreateAsync(student);

            SuccessMessage = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToPage("/Login");
        }

        private string GenerateStudentCode()
        {
            return $"SV{DateTime.Now:yyyyMMddHHmmss}";
        }
    }
}