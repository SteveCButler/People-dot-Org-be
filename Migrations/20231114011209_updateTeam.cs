using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace people_dot_org.Migrations
{
    public partial class updateTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Plans_PlanId",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "Teams",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Plans_PlanId",
                table: "Teams",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Plans_PlanId",
                table: "Teams");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Plans_PlanId",
                table: "Teams",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
