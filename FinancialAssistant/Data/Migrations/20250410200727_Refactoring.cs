using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinancialAssistant.Data.Migrations
{
    /// <inheritdoc />
    public partial class Refactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_budgets_expensecategories_expensecategoryid",
                table: "budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_accounts_account_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_expensecategories_expensecategoryid",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_incomecategories_incomecategoryid",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "expensecategories");

            migrationBuilder.DropPrimaryKey(
                name: "transactions_pkey",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "roles_name_key",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_budgets_expensecategoryid",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "name",
                table: "currencies");

            migrationBuilder.DropColumn(
                name: "expensecategoryid",
                table: "budgets");

            migrationBuilder.DropColumn(
                name: "description",
                table: "accounts");

            migrationBuilder.RenameTable(
                name: "transactions",
                newName: "Transactions");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "Transactions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "incomecategoryid",
                table: "Transactions",
                newName: "IncomeCategoryId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Transactions",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Transactions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "Transactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "Transactions",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "expensecategoryid",
                table: "Transactions",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_incomecategoryid",
                table: "Transactions",
                newName: "IX_Transactions_IncomeCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_expensecategoryid",
                table: "Transactions",
                newName: "IX_Transactions_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_account_id",
                table: "Transactions",
                newName: "IX_Transactions_AccountId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "roles",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<long>(
                name: "IncomeCategoryId",
                table: "Transactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "roles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "rate",
                table: "currencies",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "currencies",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "currencyid",
                table: "accounts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "balance",
                table: "accounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "accounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TransactionCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategory", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "name", "UserId" },
                values: new object[,]
                {
                    { 1L, "User", null },
                    { 2L, "Admin", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_roles_UserId",
                table: "roles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_users_UserId",
                table: "roles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionCategory_CategoryId",
                table: "Transactions",
                column: "CategoryId",
                principalTable: "TransactionCategory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_accounts_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_incomecategories_IncomeCategoryId",
                table: "Transactions",
                column: "IncomeCategoryId",
                principalTable: "incomecategories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_users_UserId",
                table: "roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionCategory_CategoryId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_accounts_AccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_incomecategories_IncomeCategoryId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_roles_UserId",
                table: "roles");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "accounts");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "transactions");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "transactions",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "IncomeCategoryId",
                table: "transactions",
                newName: "incomecategoryid");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "transactions",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "transactions",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "transactions",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "transactions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "transactions",
                newName: "account_id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "transactions",
                newName: "expensecategoryid");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_IncomeCategoryId",
                table: "transactions",
                newName: "IX_transactions_incomecategoryid");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_CategoryId",
                table: "transactions",
                newName: "IX_transactions_expensecategoryid");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_AccountId",
                table: "transactions",
                newName: "IX_transactions_account_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "roles",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "transactions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "incomecategoryid",
                table: "transactions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "transactions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "transactions",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "roles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<double>(
                name: "rate",
                table: "currencies",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "currencies",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "currencies",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "expensecategoryid",
                table: "budgets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "currencyid",
                table: "accounts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "balance",
                table: "accounts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "accounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "transactions_pkey",
                table: "transactions",
                column: "id");

            migrationBuilder.CreateTable(
                name: "expensecategories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("expensecategories_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_expensecategories_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "roles_name_key",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budgets_expensecategoryid",
                table: "budgets",
                column: "expensecategoryid");

            migrationBuilder.CreateIndex(
                name: "IX_expensecategories_userid",
                table: "expensecategories",
                column: "userid");

            migrationBuilder.AddForeignKey(
                name: "FK_budgets_expensecategories_expensecategoryid",
                table: "budgets",
                column: "expensecategoryid",
                principalTable: "expensecategories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_accounts_account_id",
                table: "transactions",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_expensecategories_expensecategoryid",
                table: "transactions",
                column: "expensecategoryid",
                principalTable: "expensecategories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_incomecategories_incomecategoryid",
                table: "transactions",
                column: "incomecategoryid",
                principalTable: "incomecategories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
