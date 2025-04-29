using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialAssistant.Data.Migrations
{
    /// <inheritdoc />
    public partial class change_type_deadline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Очистка некорректных данных (пример)
            migrationBuilder.Sql("UPDATE goals SET deadline = NULL WHERE deadline !~ '^\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}$'");

            // 2. Явное преобразование типа через RAW SQL
            migrationBuilder.Sql(@"
                ALTER TABLE goals 
                ALTER COLUMN deadline TYPE timestamp(6) without time zone 
                USING deadline::timestamp(6) without time zone;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Преобразование обратно в строку с форматированием
            migrationBuilder.Sql(@"
                ALTER TABLE goals 
                ALTER COLUMN deadline TYPE character varying(255) 
                USING TO_CHAR(deadline, 'YYYY-MM-DD HH24:MI:SS');
            ");
        }
    }
}
