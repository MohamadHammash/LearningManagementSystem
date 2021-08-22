using Microsoft.EntityFrameworkCore.Migrations;

namespace Lms.MVC.Data.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Assignment",
                table: "ApplicationFile",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assignment",
                table: "ApplicationFile");
        }
    }
}
