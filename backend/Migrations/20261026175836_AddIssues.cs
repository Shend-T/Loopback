using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Organizations_OrganizationId",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Organizations_OrganizationId1",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Users_AssignedToId",
                table: "Issue");

            migrationBuilder.DropForeignKey(
                name: "FK_Issue_Users_ReportedById",
                table: "Issue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Issue",
                table: "Issue");

            migrationBuilder.RenameTable(
                name: "Issue",
                newName: "Issues");

            migrationBuilder.RenameIndex(
                name: "IX_Issue_ReportedById",
                table: "Issues",
                newName: "IX_Issues_ReportedById");

            migrationBuilder.RenameIndex(
                name: "IX_Issue_OrganizationId1",
                table: "Issues",
                newName: "IX_Issues_OrganizationId1");

            migrationBuilder.RenameIndex(
                name: "IX_Issue_OrganizationId_Status",
                table: "Issues",
                newName: "IX_Issues_OrganizationId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Issue_AssignedToId",
                table: "Issues",
                newName: "IX_Issues_AssignedToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Issues",
                table: "Issues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Organizations_OrganizationId",
                table: "Issues",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Organizations_OrganizationId1",
                table: "Issues",
                column: "OrganizationId1",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Users_AssignedToId",
                table: "Issues",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_Users_ReportedById",
                table: "Issues",
                column: "ReportedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Organizations_OrganizationId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Organizations_OrganizationId1",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Users_AssignedToId",
                table: "Issues");

            migrationBuilder.DropForeignKey(
                name: "FK_Issues_Users_ReportedById",
                table: "Issues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Issues",
                table: "Issues");

            migrationBuilder.RenameTable(
                name: "Issues",
                newName: "Issue");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_ReportedById",
                table: "Issue",
                newName: "IX_Issue_ReportedById");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_OrganizationId1",
                table: "Issue",
                newName: "IX_Issue_OrganizationId1");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_OrganizationId_Status",
                table: "Issue",
                newName: "IX_Issue_OrganizationId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Issues_AssignedToId",
                table: "Issue",
                newName: "IX_Issue_AssignedToId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Issue",
                table: "Issue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Organizations_OrganizationId",
                table: "Issue",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Organizations_OrganizationId1",
                table: "Issue",
                column: "OrganizationId1",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Users_AssignedToId",
                table: "Issue",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_Users_ReportedById",
                table: "Issue",
                column: "ReportedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
