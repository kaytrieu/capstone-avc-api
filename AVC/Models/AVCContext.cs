using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AVC.Models
{
    public partial class AVCContext : DbContext
    {
        public AVCContext()
        {
        }

        public AVCContext(DbContextOptions<AVCContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AssignedCar> AssignedCar { get; set; }
        public virtual DbSet<Car> Car { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Gender> Gender { get; set; }
        public virtual DbSet<Issue> Issue { get; set; }
        public virtual DbSet<IssueType> IssueType { get; set; }
        public virtual DbSet<ModelStatus> ModelStatus { get; set; }
        public virtual DbSet<ModelVersion> ModelVersion { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<SoftwareVersion> SoftwareVersion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Address).HasMaxLength(4000);

                entity.Property(e => e.Avatar).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InverseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Account_Account");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK_Account_Gender");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<AssignedCar>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.CarId });

                entity.Property(e => e.AccountId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CarId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.AssignedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.AssignedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AssignedCarAccount)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssignedCar_Account");

                entity.HasOne(d => d.AssignedByNavigation)
                    .WithMany(p => p.AssignedCarAssignedByNavigation)
                    .HasForeignKey(d => d.AssignedBy)
                    .HasConstraintName("FK_AssignedCar_Account1");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.AssignedCar)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssignedCar_Car");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ConfigId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Image).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ModelId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(4000);

                entity.Property(e => e.SoftVersion)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Config)
                    .WithMany(p => p.Car)
                    .HasForeignKey(d => d.ConfigId)
                    .HasConstraintName("FK_Car_Configuration");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Car)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Car_Account");

                entity.HasOne(d => d.Model)
                    .WithMany(p => p.Car)
                    .HasForeignKey(d => d.ModelId)
                    .HasConstraintName("FK_Car_Model");

                entity.HasOne(d => d.SoftVersionNavigation)
                    .WithMany(p => p.Car)
                    .HasForeignKey(d => d.SoftVersion)
                    .HasConstraintName("FK_Car_SoftwareVersion");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ConfigUrl).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CarId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Image).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Issue)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK_Log_Car");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Issue)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Log_LogType");
            });

            modelBuilder.Entity<IssueType>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ModelStatus>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<ModelVersion>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.ImageFolderUrl).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Map).HasColumnName("MAP");

                entity.Property(e => e.ModelUrl).HasMaxLength(4000);

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.StatisticUrl).HasMaxLength(4000);

                entity.Property(e => e.TrainedBy)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.ModelStatus)
                    .WithMany(p => p.ModelVersion)
                    .HasForeignKey(d => d.ModelStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModelVersion_ModelStatus");

                entity.HasOne(d => d.TrainedByNavigation)
                    .WithMany(p => p.ModelVersion)
                    .HasForeignKey(d => d.TrainedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Model_Account");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SoftwareVersion>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Version)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
