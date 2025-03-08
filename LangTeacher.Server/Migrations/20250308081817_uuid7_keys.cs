using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LangTeacher.Server.Migrations
{
    /// <inheritdoc />
    public partial class uuid7_keys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""Conversations"" ADD COLUMN ""TempConversationId"" uuid not null default gen_random_uuid();");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" ADD COLUMN ""TempMessageId"" uuid not null default gen_random_uuid();");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" ADD COLUMN ""TempConversationId"" uuid;");

            migrationBuilder.Sql(@"update ""Messages"" m set ""TempConversationId"" = 
            (select ""TempConversationId"" from ""Conversations"" c where c.""ConversationId"" = m.""ConversationId"");");

            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" DROP CONSTRAINT ""FK_Messages_Conversations_ConversationId"";");

            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" DROP CONSTRAINT ""PK_Messages"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" DROP COLUMN ""AppMessageId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" RENAME COLUMN ""TempMessageId"" TO ""AppMessageId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" ADD PRIMARY KEY (""AppMessageId"");");

            migrationBuilder.Sql(@"ALTER TABLE ""Conversations"" DROP CONSTRAINT ""PK_Conversations"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Conversations"" DROP COLUMN ""ConversationId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Conversations"" RENAME COLUMN ""TempConversationId"" TO ""ConversationId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Conversations"" ADD PRIMARY KEY (""ConversationId"");");


            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" DROP COLUMN ""ConversationId"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Messages"" RENAME COLUMN ""TempConversationId"" TO ""ConversationId"";");

            migrationBuilder.Sql(@"ALTER TABLE ""Messages""
                ADD CONSTRAINT ""FK_Messages_Conversations_ConversationId""
                FOREIGN KEY (""ConversationId"") REFERENCES ""Conversations""(""ConversationId"");"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_uuidv7", ",,");

            migrationBuilder.AlterColumn<int>(
                name: "ConversationId",
                table: "Messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "AppMessageId",
                table: "Messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "ConversationId",
                table: "Conversations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
