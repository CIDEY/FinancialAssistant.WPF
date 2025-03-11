using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinancialAssistant.Models;

public partial class FinancialAssistantContext : DbContext
{
    public FinancialAssistantContext()
    {
    }

    public FinancialAssistantContext(DbContextOptions<FinancialAssistantContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Budget> Budgets { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Expensecategory> Expensecategories { get; set; }

    public virtual DbSet<Goal> Goals { get; set; }

    public virtual DbSet<Incomecategory> Incomecategories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=admin;Database=FinancialAssistant");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("accounts_pkey");

            entity.ToTable("accounts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.Currencyid).HasColumnName("currencyid");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("budgets_pkey");

            entity.ToTable("budgets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Currentprogress).HasColumnName("currentprogress");
            entity.Property(e => e.Enddate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("enddate");
            entity.Property(e => e.Expensecategoryid).HasColumnName("expensecategoryid");
            entity.Property(e => e.Period)
                .HasMaxLength(255)
                .HasColumnName("period");
            entity.Property(e => e.Startdate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("startdate");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("currencies_pkey");

            entity.ToTable("currencies");

            entity.HasIndex(e => e.Code, "uk5r2dfxl1m7vus47ma0y05sflt").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(255)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Symbol)
                .HasMaxLength(255)
                .HasColumnName("symbol");
        });

        modelBuilder.Entity<Expensecategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("expensecategories_pkey");

            entity.ToTable("expensecategories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("goals_pkey");

            entity.ToTable("goals");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createddate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Currentprogress).HasColumnName("currentprogress");
            entity.Property(e => e.Deadline)
                .HasMaxLength(255)
                .HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Targetamount).HasColumnName("targetamount");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<Incomecategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("incomecategories_pkey");

            entity.ToTable("incomecategories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Expensecategoryid).HasColumnName("expensecategoryid");
            entity.Property(e => e.Incomecategoryid).HasColumnName("incomecategoryid");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
        });

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
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Registrationdate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("registrationdate");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("user_roles_role_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("user_roles_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("user_roles_pkey");
                        j.ToTable("user_roles");
                        j.IndexerProperty<long>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<long>("RoleId").HasColumnName("role_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
