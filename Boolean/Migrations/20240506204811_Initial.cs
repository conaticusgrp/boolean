using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Boolean.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "servers",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    snowflake = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ServerSnowflake = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    administrator = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_members_servers_ServerSnowflake",
                        column: x => x.ServerSnowflake,
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "special_channels",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServerSnowflake = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    snowflake = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    purpose = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_members_ServerSnowflake",
                table: "members",
                column: "ServerSnowflake");

            migrationBuilder.CreateIndex(
                name: "IX_special_channels_ServerSnowflake",
                table: "special_channels",
                column: "ServerSnowflake");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "special_channels");

            migrationBuilder.DropTable(
                name: "servers");
        }
    }
}
