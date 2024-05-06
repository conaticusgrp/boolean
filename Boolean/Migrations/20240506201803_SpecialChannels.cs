using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Boolean.Migrations
{
    /// <inheritdoc />
    public partial class SpecialChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_members_servers_ServerId",
                table: "members");

            migrationBuilder.DropColumn(
                name: "snowflake",
                table: "servers");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "members",
                newName: "ServerSnowflake");

            migrationBuilder.RenameIndex(
                name: "IX_members_ServerId",
                table: "members",
                newName: "IX_members_ServerSnowflake");

            migrationBuilder.CreateTable(
                name: "special_channels",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServerSnowflake = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    snowflake = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    purpose = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_special_channels", x => x.id);
                    table.ForeignKey(
                        name: "FK_special_channels_servers_ServerSnowflake",
                        column: x => x.ServerSnowflake,
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_special_channels_ServerSnowflake",
                table: "special_channels",
                column: "ServerSnowflake");

            migrationBuilder.AddForeignKey(
                name: "FK_members_servers_ServerSnowflake",
                table: "members",
                column: "ServerSnowflake",
                principalTable: "servers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_members_servers_ServerSnowflake",
                table: "members");

            migrationBuilder.DropTable(
                name: "special_channels");

            migrationBuilder.RenameColumn(
                name: "ServerSnowflake",
                table: "members",
                newName: "ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_members_ServerSnowflake",
                table: "members",
                newName: "IX_members_ServerId");

            migrationBuilder.AddColumn<decimal>(
                name: "snowflake",
                table: "servers",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_members_servers_ServerId",
                table: "members",
                column: "ServerId",
                principalTable: "servers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
