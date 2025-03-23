using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestSystem.Database;

public partial class DataBaseContext : DbContext
{
    public DataBaseContext()
    {
        Database.EnsureCreated();
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Diagnose> Diagnoses { get; set; }

    public virtual DbSet<FinishedModule> FinishedModules { get; set; }

    public virtual DbSet<FinishedTest> FinishedTests { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<RegisteredUser> RegisteredUsers { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=DataBase.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasOne(d => d.QuestionNavigation).WithMany(p => p.Answers)
                .HasForeignKey(d => d.Question)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Diagnose>(entity =>
        {
            entity.HasOne(d => d.TestNavigation).WithMany(p => p.Diagnoses)
                .HasForeignKey(d => d.Test)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FinishedModule>(entity =>
        {
            entity.HasOne(d => d.FinishedTestNavigation).WithMany(p => p.FinishedModules)
                .HasForeignKey(d => d.FinishedTest)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasOne(d => d.TestNavigation).WithMany(p => p.Modules)
                .HasForeignKey(d => d.Test)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasOne(d => d.ModuleNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.Module)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RegisteredUser>(entity =>
        {
            entity.HasKey(e => e.Login);

            entity.Property(e => e.Password).HasColumnType("INTEGER");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.Author)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
