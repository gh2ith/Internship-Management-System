using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using WebApplication1.Models;

namespace WebApplication1.Data;

public partial class MydbContext : DbContext
{
    public MydbContext()
    {
    }

    public MydbContext(DbContextOptions<MydbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<College> Colleges { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<InternshipOpportunity> InternshipOpportunities { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Supervisor> Supervisors { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PRIMARY");

            entity.ToTable("application");

            entity.HasIndex(e => e.ApplicationId, "application_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CompId, "fk_application_comp");

            entity.HasIndex(e => e.InternshipId, "fk_application_internship");

            entity.HasIndex(e => e.StdId, "fk_application_std");

            entity.HasIndex(e => e.SuperId, "fk_application_super");

            entity.Property(e => e.ApplicationId)
                .ValueGeneratedNever()
                .HasColumnName("application_id");
            entity.Property(e => e.CompId).HasColumnName("comp_id");
            entity.Property(e => e.InternshipId).HasColumnName("internship_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StdId).HasColumnName("std_id");
            entity.Property(e => e.SuperId).HasColumnName("super_id");

            entity.HasOne(d => d.Comp).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CompId)
                .HasConstraintName("fk_application_comp");

            entity.HasOne(d => d.Internship).WithMany(p => p.Applications)
                .HasForeignKey(d => d.InternshipId)
                .HasConstraintName("fk_application_internship");

            entity.HasOne(d => d.Std).WithMany(p => p.Applications)
                .HasForeignKey(d => d.StdId)
                .HasConstraintName("fk_application_std");

            entity.HasOne(d => d.Super).WithMany(p => p.Applications)
                .HasForeignKey(d => d.SuperId)
                .HasConstraintName("fk_application_super");
        });

        modelBuilder.Entity<College>(entity =>
        {
            entity.HasKey(e => e.CollegeId).HasName("PRIMARY");

            entity.ToTable("college");

            entity.HasIndex(e => e.CollegeId, "college_id_UNIQUE").IsUnique();

            entity.Property(e => e.CollegeId)
                .ValueGeneratedNever()
                .HasColumnName("college_id");
            entity.Property(e => e.CollegeName)
                .HasMaxLength(45)
                .HasColumnName("college_name");
            entity.Property(e => e.UniName)
                .HasMaxLength(45)
                .HasColumnName("uni-name");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PRIMARY");

            entity.ToTable("company");

            entity.HasIndex(e => e.CompanyId, "company_ID_UNIQUE").IsUnique();

            entity.Property(e => e.CompanyId)
                .ValueGeneratedNever()
                .HasColumnName("company_id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(45)
                .HasColumnName("company_name");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(45)
                .HasColumnName("contact_info");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasColumnName("description");
            entity.Property(e => e.Location)
                .HasMaxLength(45)
                .HasColumnName("location");
        });

        modelBuilder.Entity<InternshipOpportunity>(entity =>
        {
            entity.HasKey(e => e.InternshipId).HasName("PRIMARY");

            entity.ToTable("internship_opportunity");

            entity.HasIndex(e => e.CompanyId, "fk_internship_comp");

            entity.HasIndex(e => e.StdId, "fk_internship_std");

            entity.HasIndex(e => e.SuperId, "fk_internship_super");

            entity.HasIndex(e => e.InternshipId, "internship_id_UNIQUE").IsUnique();

            entity.Property(e => e.InternshipId)
                .ValueGeneratedNever()
                .HasColumnName("internship_id");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasMaxLength(45)
                .HasColumnName("description");
            entity.Property(e => e.Requirments)
                .HasMaxLength(45)
                .HasColumnName("requirments");
            entity.Property(e => e.StdId).HasColumnName("std_id");
            entity.Property(e => e.IsClosed).HasColumnName("is_closed");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.SuperId).HasColumnName("super_id");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");

            entity.HasOne(d => d.Company).WithMany(p => p.InternshipOpportunities)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("fk_internship_comp");

            entity.HasOne(d => d.Std).WithMany(p => p.InternshipOpportunities)
                .HasForeignKey(d => d.StdId)
                .HasConstraintName("fk_internship_std");

            entity.HasOne(d => d.Super).WithMany(p => p.InternshipOpportunities)
                .HasForeignKey(d => d.SuperId)
                .HasConstraintName("fk_internship_super");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PRIMARY");

            entity.ToTable("report");

            entity.HasIndex(e => e.ReportId, "evaluation_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CompId, "fk_report_comp");

            entity.HasIndex(e => e.SuperId, "fk_report_super");

            entity.Property(e => e.ReportId)
                .ValueGeneratedNever()
                .HasColumnName("report_id");
            entity.Property(e => e.CompId).HasColumnName("comp_id");
            entity.Property(e => e.NumberOfHours).HasColumnName("number_of_hours");
            entity.Property(e => e.SuperId).HasColumnName("super_id");
            entity.Property(e => e.Task)
                .HasMaxLength(45)
                .HasColumnName("task");
            entity.Property(e => e.Tools)
                .HasMaxLength(45)
                .HasColumnName("tools");

            entity.HasOne(d => d.Comp).WithMany(p => p.Reports)
                .HasForeignKey(d => d.CompId)
                .HasConstraintName("fk_report_comp");

            entity.HasOne(d => d.Super).WithMany(p => p.Reports)
                .HasForeignKey(d => d.SuperId)
                .HasConstraintName("fk_report_super");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PRIMARY");

            entity.ToTable("student");

            entity.HasIndex(e => e.CollegeId, "fk_student_college");

            entity.HasIndex(e => e.ReportId, "fk_student_report");

            entity.HasIndex(e => e.SuperId, "fk_student_super");

            entity.HasIndex(e => e.StudentId, "student_id_UNIQUE").IsUnique();

            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("student_id");
            entity.Property(e => e.CollegeId).HasColumnName("college_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(45)
                .HasColumnName("full_name");
            entity.Property(e => e.Gpa).HasColumnName("gpa");
            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.Resume).HasColumnName("resume");
            entity.Property(e => e.StDepartment)
                .HasMaxLength(45)
                .HasColumnName("st_department");
            entity.Property(e => e.SuperId).HasColumnName("super_id");

            entity.HasOne(d => d.College).WithMany(p => p.Students)
                .HasForeignKey(d => d.CollegeId)
                .HasConstraintName("fk_student_college");

            entity.HasOne(d => d.Report).WithMany(p => p.Students)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("fk_student_report");

            entity.HasOne(d => d.Super).WithMany(p => p.Students)
                .HasForeignKey(d => d.SuperId)
                .HasConstraintName("fk_student_super");
        });

        modelBuilder.Entity<Supervisor>(entity =>
        {
            entity.HasKey(e => e.SupervisorId).HasName("PRIMARY");

            entity.ToTable("supervisor");

            entity.HasIndex(e => e.CollegeId, "fk_supervisor_college");

            entity.HasIndex(e => e.SupervisorId, "supervisor_id_UNIQUE").IsUnique();

            entity.Property(e => e.SupervisorId)
                .ValueGeneratedNever()
                .HasColumnName("supervisor_id");
            entity.Property(e => e.CollegeId).HasColumnName("college_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(45)
                .HasColumnName("full_name");
            entity.Property(e => e.SuperDepartment)
                .HasMaxLength(45)
                .HasColumnName("super_department");

            entity.HasOne(d => d.College).WithMany(p => p.Supervisors)
                .HasForeignKey(d => d.CollegeId)
                .HasConstraintName("fk_supervisor_college");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.CollegeId, "fk_user_college");

            entity.HasIndex(e => e.UserId, "user_id_UNIQUE").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.CollegeId).HasColumnName("college_id");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(45)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(45)
                .HasColumnName("role");

            entity.HasOne(d => d.College).WithMany(p => p.Users)
                .HasForeignKey(d => d.CollegeId)
                .HasConstraintName("fk_user_college");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
