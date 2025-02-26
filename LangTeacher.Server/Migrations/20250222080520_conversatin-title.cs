using LangTeacher.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangTeacher.Server.Migrations
{
    /// <inheritdoc />
    public partial class conversatintitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Conversations",
                type: "text",
                nullable: false,
            defaultValue: "");

            migrationBuilder.Sql(@"
            UPDATE ""Conversations"" c
            SET ""Title"" = subquery.title
            FROM (
                SELECT DISTINCT ON (m.""ConversationId"") 
                    m.""ConversationId"", 
                    LEFT(m.""Content"", 40) AS title
                FROM ""Messages"" m
                ORDER BY m.""ConversationId"", m.""CreatedAt"" ASC
            ) AS subquery
            WHERE c.""ConversationId"" = subquery.""ConversationId"";
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Conversations");
        }
    }
}
