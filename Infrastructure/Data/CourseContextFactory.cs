using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class CourseContextFactory : IDesignTimeDbContextFactory<CourseContext>
    {
        public CourseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CourseContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=Courses_Api;User Id=sa;Password=Pa55w.rd;TrustServerCertificate=true;");

            return new CourseContext(optionsBuilder.Options);
        }
    }
}
