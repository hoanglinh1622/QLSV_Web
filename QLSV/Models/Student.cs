// Models/Student.cs
namespace StudentManagement.Models
{
    public class Student
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    }
}