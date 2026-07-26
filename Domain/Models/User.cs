using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateOnly DateOfRegistration { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<ExamAttempt> ExamAttempts { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<EnrolledCourse> EnrolledCourses { get; set; }
    }
}
