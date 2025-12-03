using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixTrainerServiceManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Trainers_TrainerId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_TrainerId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TrainerId",
                table: "Services");

            migrationBuilder.CreateTable(
                name: "ServiceTrainer",
                columns: table => new
                {
                    SpecialtiesId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTrainer", x => new { x.SpecialtiesId, x.TrainerId });
                    table.ForeignKey(
                        name: "FK_ServiceTrainer_Services_SpecialtiesId",
                        column: x => x.SpecialtiesId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceTrainer_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTrainer_TrainerId",
                table: "ServiceTrainer",
                column: "TrainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceTrainer");

            migrationBuilder.AddColumn<int>(
                name: "TrainerId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_TrainerId",
                table: "Services",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Trainers_TrainerId",
                table: "Services",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id");
        }
    }
}
