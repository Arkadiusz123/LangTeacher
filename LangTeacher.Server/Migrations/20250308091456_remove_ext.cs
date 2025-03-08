using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangTeacher.Server.Migrations
{
    /// <inheritdoc />
    public partial class remove_ext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_uuidv7", ",,");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_uuidv7", ",,");
        }
    }
}
