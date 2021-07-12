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
        public virtual DbSet<DefaultConfiguration> DefaultConfiguration { get; set; }
        public virtual DbSet<Issue> Issue { get; set; }
        public virtual DbSet<IssueType> IssueType { get; set; }
        public virtual DbSet<ModelStatus> ModelStatus { get; set; }
        public virtual DbSet<ModelVersion> ModelVersion { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<UserNotification> UserNotification { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("IX_Account_UniqueEmail")
                    .IsUnique();

                entity.Property(e => e.Avatar).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.ResetPasswordToken)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ManagedByNavigation)
                    .WithMany(p => p.InverseManagedByNavigation)
                    .HasForeignKey(d => d.ManagedBy)
                    .HasConstraintName("FK_Account_Account1");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Role");
            });

            modelBuilder.Entity<AssignedCar>(entity =>
            {
                entity.Property(e => e.AssignedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RemoveAt).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AssignedCarAccount)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssignedCar_Account2");

                entity.HasOne(d => d.AssignedByNavigation)
                    .WithMany(p => p.AssignedCarAssignedByNavigation)
                    .HasForeignKey(d => d.AssignedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssignedCar_Account");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.AssignedCar)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssignedCar_Car");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasIndex(e => e.DeviceId)
                    .HasName("IX_Car")
                    .IsUnique();

                entity.Property(e => e.ConfigUrl).HasMaxLength(4000);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.DeviceId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Image).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name).HasMaxLength(4000);

                entity.HasOne(d => d.ManagedByNavigation)
                    .WithMany(p => p.Car)
                    .HasForeignKey(d => d.ManagedBy)
                    .HasConstraintName("FK_Car_Account");
            });

            modelBuilder.Entity<DefaultConfiguration>(entity =>
            {
                entity.Property(e => e.ConfigUrl)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Image).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Issue)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Issue_Car");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Issue)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Log_LogType");
            });

            modelBuilder.Entity<IssueType>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ModelStatus>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<ModelVersion>(entity =>
            {
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

                entity.HasOne(d => d.ModelStatus)
                    .WithMany(p => p.ModelVersion)
                    .HasForeignKey(d => d.ModelStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModelVersion_ModelStatus");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.IsAvailable)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("((getdate() AT TIME ZONE 'Pacific Standard Time'))");

                entity.Property(e => e.Message).HasMaxLength(4000);

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.UserNotification)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK_UserNotification_Account");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
