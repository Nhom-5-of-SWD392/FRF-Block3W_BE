using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabasev3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResult_User_EvaluateById",
                table: "QuizResult");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResult_User_QuizMadeById",
                table: "QuizResult");

            migrationBuilder.DropIndex(
                name: "IX_QuizResult_QuizMadeById",
                table: "QuizResult");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "User");

            migrationBuilder.DropColumn(
                name: "QuizMadeById",
                table: "QuizResult");

            migrationBuilder.AddColumn<Guid>(
                name: "QuizResultId",
                table: "ModeratorApplication",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorApplication_QuizResultId",
                table: "ModeratorApplication",
                column: "QuizResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModeratorApplication_QuizResult_QuizResultId",
                table: "ModeratorApplication",
                column: "QuizResultId",
                principalTable: "QuizResult",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResult_User_EvaluateById",
                table: "QuizResult",
                column: "EvaluateById",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModeratorApplication_QuizResult_QuizResultId",
                table: "ModeratorApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResult_User_EvaluateById",
                table: "QuizResult");

            migrationBuilder.DropIndex(
                name: "IX_ModeratorApplication_QuizResultId",
                table: "ModeratorApplication");

            migrationBuilder.DropColumn(
                name: "QuizResultId",
                table: "ModeratorApplication");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuizMadeById",
                table: "QuizResult",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_QuizResult_QuizMadeById",
                table: "QuizResult",
                column: "QuizMadeById");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResult_User_EvaluateById",
                table: "QuizResult",
                column: "EvaluateById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResult_User_QuizMadeById",
                table: "QuizResult",
                column: "QuizMadeById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
