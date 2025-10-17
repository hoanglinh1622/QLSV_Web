using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagement.Services;

namespace StudentManagement.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        public void OnGet()
        {
            // Check if already logged in
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                if (role == "Admin")
                    Response.Redirect("/Admin/Dashboard");
                else
                    Response.Redirect("/Student/Dashboard");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nh?p ??y ?? thông tin";
                return Page();
            }

            var user = await _userService.AuthenticateAsync(Username, Password);

            if (user == null)
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return Page();
            }

            // Set session
            HttpContext.Session.SetString("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect based on role
            if (user.Role == "Admin")
                return RedirectToPage("/Admin/Dashboard");
            else
                return RedirectToPage("/Student/Dashboard");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}