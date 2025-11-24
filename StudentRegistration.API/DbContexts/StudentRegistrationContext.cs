using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.API.DbEntities;

namespace StudentRegistration.API.DbContexts;

public partial class StudentRegistrationContext : DbContext
{
    public StudentRegistrationContext(DbContextOptions<StudentRegistrationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PRIMARY");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments).HasConstraintName("FK_ENROLLMENT_COURSE");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments).HasConstraintName("FK_ENROLLMENT_STUDENT");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PRIMARY");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
