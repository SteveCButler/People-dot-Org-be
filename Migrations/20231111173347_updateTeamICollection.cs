using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace people_dot_org.Migrations
{
    public partial class updateTeamICollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonTeam_People_PersonsId",
                table: "PersonTeam");

            migrationBuilder.RenameColumn(
                name: "PersonsId",
                table: "PersonTeam",
                newName: "PeopleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonTeam_People_PeopleId",
                table: "PersonTeam",
                column: "PeopleId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonTeam_People_PeopleId",
                table: "PersonTeam");

            migrationBuilder.RenameColumn(
                name: "PeopleId",
                table: "PersonTeam",
                newName: "PersonsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonTeam_People_PersonsId",
                table: "PersonTeam",
                column: "PersonsId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
