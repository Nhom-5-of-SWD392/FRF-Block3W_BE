using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updatequizresulttablev1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuizId",
                table: "QuizResult",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_QuizResult_QuizId",
                table: "QuizResult",
                column: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResult_Quiz_QuizId",
                table: "QuizResult",
                column: "QuizId",
                principalTable: "Quiz",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResult_Quiz_QuizId",
                table: "QuizResult");

            migrationBuilder.DropIndex(
                name: "IX_QuizResult_QuizId",
                table: "QuizResult");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "QuizResult");
        }
    }
}
