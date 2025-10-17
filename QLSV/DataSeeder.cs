using StudentManagement.Models;
using StudentManagement.Services;

namespace StudentManagement
{
    public class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var csvService = serviceProvider.GetRequiredService<ICsvService>();
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var studentService = serviceProvider.GetRequiredService<IStudentService>();
            var courseService = serviceProvider.GetRequiredService<ICourseService>();

            // Seed Admin User
            var adminUser = await userService.GetByUsernameAsync("admin");
            if (adminUser == null)
            {
                await userService.RegisterAsync("admin", "admin123", "Admin");
            }

            // Seed Sample Student
            var studentUser = await userService.GetByUsernameAsync("student1");
            if (studentUser == null)
            {
                studentUser = await userService.RegisterAsync("student1", "student123", "Student");

                if (studentUser != null)
                {
                    var student = new Student
                    {
                        UserId = studentUser.Id,
                        FullName = "Nguyễn Văn Toàn ",
                        Email = "nguyenvantoan@gmail.com",
                        Phone = "0987654321",
                        DateOfBirth = new DateTime(2003, 5, 15),
                        Address = "111 Đường LK, Quận 11, TP.HCM",
                        StudentCode = "SV124563"
                    };
                    await studentService.CreateAsync(student);
                }
            }

            // Seed Sample Courses
            var courses = await courseService.GetAllAsync();
            if (courses.Count == 0)
            {
                var sampleCourses = new List<Course>
                {
                    new Course
                    {
                        CourseCode = "CS101",
                        CourseName = "Nhập môn Lập trình",
                        Description = "Học các khái niệm cơ bản về lập trình máy tính",
                        Credits = 3,
                        Instructor = "TS. Trần Văn B",
                        MaxStudents = 40,
                        EnrolledStudents = 15,
                        StartDate = DateTime.Now.AddDays(30),
                        EndDate = DateTime.Now.AddDays(120),
                        IsActive = true
                    },
                    new Course
                    {
                        CourseCode = "CS102",
                        CourseName = "Cấu trúc Dữ liệu và Giải thuật",
                        Description = "Nghiên cứu các cấu trúc dữ liệu và giải thuật cơ bản",
                        Credits = 4,
                        Instructor = "PGS.TS. Lê Thị C",
                        MaxStudents = 35,
                        EnrolledStudents = 20,
                        StartDate = DateTime.Now.AddDays(45),
                        EndDate = DateTime.Now.AddDays(135),
                        IsActive = true
                    },
                    new Course
                    {
                        CourseCode = "CS201",
                        CourseName = "Lập trình Hướng đối tượng",
                        Description = "Học các nguyên lý lập trình hướng đối tượng với C#",
                        Credits = 3,
                        Instructor = "ThS. Phạm Văn D",
                        MaxStudents = 30,
                        EnrolledStudents = 25,
                        StartDate = DateTime.Now.AddDays(15),
                        EndDate = DateTime.Now.AddDays(105),
                        IsActive = true
                    },
                    new Course
                    {
                        CourseCode = "CS301",
                        CourseName = "Cơ sở Dữ liệu",
                        Description = "Thiết kế và quản lý cơ sở dữ liệu",
                        Credits = 4,
                        Instructor = "TS. Hoàng Văn E",
                        MaxStudents = 40,
                        EnrolledStudents = 30,
                        StartDate = DateTime.Now.AddDays(60),
                        EndDate = DateTime.Now.AddDays(150),
                        IsActive = true
                    },
                    new Course
                    {
                        CourseCode = "CS401",
                        CourseName = "Phát triển Web",
                        Description = "Xây dựng ứng dụng web với ASP.NET Core",
                        Credits = 3,
                        Instructor = "ThS. Võ Thị F",
                        MaxStudents = 35,
                        EnrolledStudents = 35,
                        StartDate = DateTime.Now.AddDays(10),
                        EndDate = DateTime.Now.AddDays(100),
                        IsActive = true
                    }
                };

                foreach (var course in sampleCourses)
                {
                    await courseService.CreateAsync(course);
                }
            }
        }
    }
}