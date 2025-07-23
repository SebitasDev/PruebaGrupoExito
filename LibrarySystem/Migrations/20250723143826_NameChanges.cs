using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class NameChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BibliographicMaterials_ISBN",
                table: "Loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Loans",
                table: "Loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BibliographicMaterials",
                table: "BibliographicMaterials");

            migrationBuilder.RenameTable(
                name: "Loans",
                newName: "loans");

            migrationBuilder.RenameTable(
                name: "BibliographicMaterials",
                newName: "bibliographic_materials");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_UserId",
                table: "loans",
                newName: "IX_loans_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_Status",
                table: "loans",
                newName: "IX_loans_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_RequestDate",
                table: "loans",
                newName: "IX_loans_RequestDate");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_ISBN",
                table: "loans",
                newName: "IX_loans_ISBN");

            migrationBuilder.RenameIndex(
                name: "IX_BibliographicMaterials_Type",
                table: "bibliographic_materials",
                newName: "IX_bibliographic_materials_Type");

            migrationBuilder.RenameIndex(
                name: "IX_BibliographicMaterials_IsAvailable",
                table: "bibliographic_materials",
                newName: "IX_bibliographic_materials_IsAvailable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_loans",
                table: "loans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bibliographic_materials",
                table: "bibliographic_materials",
                column: "ISBN");

            migrationBuilder.AddForeignKey(
                name: "FK_loans_bibliographic_materials_ISBN",
                table: "loans",
                column: "ISBN",
                principalTable: "bibliographic_materials",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_loans_bibliographic_materials_ISBN",
                table: "loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_loans",
                table: "loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bibliographic_materials",
                table: "bibliographic_materials");

            migrationBuilder.RenameTable(
                name: "loans",
                newName: "Loans");

            migrationBuilder.RenameTable(
                name: "bibliographic_materials",
                newName: "BibliographicMaterials");

            migrationBuilder.RenameIndex(
                name: "IX_loans_UserId",
                table: "Loans",
                newName: "IX_Loans_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_loans_Status",
                table: "Loans",
                newName: "IX_Loans_Status");

            migrationBuilder.RenameIndex(
                name: "IX_loans_RequestDate",
                table: "Loans",
                newName: "IX_Loans_RequestDate");

            migrationBuilder.RenameIndex(
                name: "IX_loans_ISBN",
                table: "Loans",
                newName: "IX_Loans_ISBN");

            migrationBuilder.RenameIndex(
                name: "IX_bibliographic_materials_Type",
                table: "BibliographicMaterials",
                newName: "IX_BibliographicMaterials_Type");

            migrationBuilder.RenameIndex(
                name: "IX_bibliographic_materials_IsAvailable",
                table: "BibliographicMaterials",
                newName: "IX_BibliographicMaterials_IsAvailable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Loans",
                table: "Loans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BibliographicMaterials",
                table: "BibliographicMaterials",
                column: "ISBN");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BibliographicMaterials_ISBN",
                table: "Loans",
                column: "ISBN",
                principalTable: "BibliographicMaterials",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
