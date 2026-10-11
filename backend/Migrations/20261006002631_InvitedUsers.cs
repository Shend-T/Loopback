using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InvitedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InviteUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "citext", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    InvitedById = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    ExpiryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InviteUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InviteUsers_Users_InvitedById",
                        column: x => x.InvitedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InviteUsers_InvitedById",
                table: "InviteUsers",
                column: "InvitedById");

            migrationBuilder.CreateIndex(
                name: "IX_InviteUsers_OrganizationId_Email",
                table: "InviteUsers",
                columns: new[] { "OrganizationId", "Email" },
                unique: true,
                filter: "\"AcceptedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InviteUsers_TokenHash",
                table: "InviteUsers",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InviteUsers");
        }
    }
}
