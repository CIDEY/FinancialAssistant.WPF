using FinancialAssistant.Classes;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FinancialAssistant.Models
{
    public partial class FinancialAssistantContext : DbContext
    {
        public FinancialAssistantContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public FinancialAssistantContext(DbContextOptions<FinancialAssistantContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public virtual DbSet<Account> Accounts { get; set; }

        public virtual DbSet<Budget> Budgets { get; set; }

        public virtual DbSet<Currency> Currencies { get; set; }

        public virtual DbSet<Goal> Goals { get; set; }

        public virtual DbSet<IncomeCategory> Incomecategories { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Transaction> Transactions { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=admin;Database=FinanceDB");


        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateOnly>()
                   .HaveConversion<DateOnlyConverter>()
                   .HaveColumnType("date");

            builder.Properties<DateTime>()
                    .HaveConversion<DateTimeToUtcConverter>()
                    .HaveColumnType("timestamp with time zone");

        }

        public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
        {
            public DateOnlyConverter() : base(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d))
            { }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("accounts_pkey");
                entity.ToTable("accounts");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Balance).HasColumnName("balance");
                entity.Property(e => e.CurrencyId).HasColumnName("currencyid");
                entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
                entity.Property(e => e.Type).HasMaxLength(255).HasColumnName("type");
                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.HasOne(a => a.Currency)
                      .WithMany(c => c.Accounts)
                      .HasForeignKey(a => a.CurrencyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("NOW()"); // Автоматическая установка времени при создании

                entity.HasOne(a => a.User)
                      .WithMany(u => u.Accounts)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.Date)
                    .HasColumnType("date")
                    .HasConversion<DateOnlyConverter>();
            });

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("budgets_pkey");
                entity.ToTable("budgets");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Amount).HasColumnName("amount");
                entity.Property(e => e.CurrentProgress).HasColumnName("currentprogress");
                entity.Property(e => e.EndDate).HasColumnType("timestamp(6) without time zone").HasColumnName("enddate");
                entity.Property(e => e.Period).HasMaxLength(255).HasColumnName("period");
                entity.Property(e => e.StartDate).HasColumnType("timestamp(6) without time zone").HasColumnName("startdate");
                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Budgets)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("currencies_pkey");
                entity.ToTable("currencies");
                entity.HasIndex(e => e.Code, "uk5r2dfxl1m7vus47ma0y05sflt").IsUnique();
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Code).HasMaxLength(255).HasColumnName("code");
                entity.Property(e => e.Rate).HasColumnName("rate");
                entity.Property(e => e.Symbol).HasMaxLength(255).HasColumnName("symbol");
            });

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)"); // Указываем точный тип для БД



            modelBuilder.Entity<Goal>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("goals_pkey");
                entity.ToTable("goals");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CreatedDate)
                      .HasColumnType("timestamp(6) without time zone")
                      .HasColumnName("createddate");
                entity.Property(e => e.CurrentProgress).HasColumnName("currentprogress");
                entity.Property(e => e.Deadline)
                      .HasColumnType("timestamp without time zone")
                      .HasColumnName("deadline");
                entity.Property(e => e.Description)
                      .HasMaxLength(255)
                      .HasColumnName("description");
                entity.Property(e => e.Name)
                      .HasMaxLength(255)
                      .HasColumnName("name");
                entity.Property(e => e.TargetAmount).HasColumnName("targetamount");
                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.HasOne(g => g.User)
                      .WithMany(u => u.Goals)
                      .HasForeignKey(g => g.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<IncomeCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("incomecategories_pkey");
                entity.ToTable("incomecategories");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Description).HasMaxLength(255).HasColumnName("description");
                entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
                entity.Property(e => e.UserId).HasColumnName("userid");

                entity.HasOne(ic => ic.User)
                      .WithMany(u => u.IncomeCategories)
                      .HasForeignKey(ic => ic.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");
                entity.ToTable("users");

                entity.HasIndex(e => e.Login, "ukow0gan20590jrb00upg3va2fn").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");
                entity.Property(e => e.Login)
                    .HasMaxLength(255)
                    .HasColumnName("login");
                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .HasColumnName("passwordhash");
                entity.Property(e => e.RegistrationDate)
                    .HasColumnType("timestamp(6) without time zone")
                    .HasColumnName("registrationdate");

                // Настройка связи через промежуточную сущность
                entity.HasMany(u => u.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId)
                    .HasConstraintName("user_roles_user_id_fkey");

                entity.Property(u => u.RegistrationDate)
                    .HasColumnType("timestamp with time zone") // Для PostgreSQL
                    .HasConversion(
                        v => v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id).HasName("roles_pkey");
                entity.ToTable("roles");

                // Настройка свойства Name
                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasMany(r => r.UserRoles)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .HasConstraintName("user_roles_role_id_fkey");

                // Добавляем начальные данные
                entity.HasData(
                    new Role { Id = 1, Name = "User" },
                    new Role { Id = 2, Name = "Admin" }
                );
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId })
                    .HasName("user_roles_pkey");

                entity.ToTable("user_roles");

                entity.Property(ur => ur.UserId)
                    .HasColumnName("user_id");

                entity.Property(ur => ur.RoleId)
                    .HasColumnName("role_id");
            });

        }



        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
