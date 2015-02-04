using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace CourseAider.Models
{
    public class CourseAiderContext : DbContext
    {
        public CourseAiderContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //this.Database.ExecuteSqlCommand("ALTER TABLE Courses ADD CONSTRAINT uc_Name UNIQUE(Name)");
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<File> Files { get; set; }
    }  
}
