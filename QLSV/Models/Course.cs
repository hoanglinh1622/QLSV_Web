// Models/Course.cs
namespace StudentManagement.Models
{
    public class Course
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Instructor { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public int EnrolledStudents { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsCompleted { get; set; } = false; // Thêm field mới
    }
}