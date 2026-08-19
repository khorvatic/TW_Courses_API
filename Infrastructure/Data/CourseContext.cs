using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class CourseContext : DbContext
    {
        public CourseContext(DbContextOptions<CourseContext> options)
            : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<EnrolledCourse> EnrolledCourses { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<ExamQuestionAnswer> ExamQuestionAnswers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- UserRole ---
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // --- ExamQuestionAnswer ---
            modelBuilder.Entity<ExamQuestionAnswer>()
                .HasKey(eqa => new { eqa.AttemptId, eqa.QuestionId, eqa.AnswerId });

            modelBuilder.Entity<ExamQuestionAnswer>()
                .HasOne(eqa => eqa.Answer)
                .WithMany(a => a.ExamQuestionAnswers)
                .HasForeignKey(eqa => eqa.AnswerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamQuestionAnswer>()
                .HasOne(eqa => eqa.Attempt)
                .WithMany(ea => ea.ExamQuestionAnswers)
                .HasForeignKey(eqa => eqa.AttemptId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ExamQuestionAnswer>()
                .HasOne(eqa => eqa.Question)
                .WithMany(q => q.ExamQuestionAnswers)
                .HasForeignKey(eqa => eqa.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            // --- ExamAttempt ---
            modelBuilder.Entity<ExamAttempt>()
                .HasOne(ea => ea.User)
                .WithMany(u => u.ExamAttempts)
                .OnDelete(DeleteBehavior.Restrict);

            // --- User ---
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.EnrolledCourses)
                .WithOne(ec => ec.User)
                .OnDelete(DeleteBehavior.Cascade);


            // --- Course ---
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Chapters)
                .WithOne(ch => ch.Course)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Exam ---
            modelBuilder.Entity<Exam>()
                .HasMany(e => e.Questions)
                .WithOne(q => q.Exam)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Role ---
            modelBuilder.Entity<Role>()
                .HasData(
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" }
                );
        }
    }
}
