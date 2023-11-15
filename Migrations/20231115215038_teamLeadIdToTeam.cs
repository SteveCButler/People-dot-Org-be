using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace people_dot_org.Migrations
{
    public partial class teamLeadIdToTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeamLeadId",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamLeadId",
                table: "Teams");
        }
    }
}
