using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangTeacher.Server.Migrations
{
    /// <inheritdoc />
    public partial class messages_cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" ALTER COLUMN ""ConversationId"" SET NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" DROP CONSTRAINT IF EXISTS ""FK_Messages_Conversations_ConversationId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" 
                ADD CONSTRAINT ""FK_Messages_Conversations_ConversationId"" 
                FOREIGN KEY (""ConversationId"") 
                REFERENCES ""Conversations""(""ConversationId"") 
                ON DELETE CASCADE;"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
