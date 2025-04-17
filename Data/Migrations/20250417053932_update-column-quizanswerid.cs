﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updatecolumnquizanswerid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizDetail_QuizAnswer_QuizAnswerId",
                table: "QuizDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuizAnswerId",
                table: "QuizDetail",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizDetail_QuizAnswer_QuizAnswerId",
                table: "QuizDetail",
                column: "QuizAnswerId",
                principalTable: "QuizAnswer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizDetail_QuizAnswer_QuizAnswerId",
                table: "QuizDetail");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuizAnswerId",
                table: "QuizDetail",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizDetail_QuizAnswer_QuizAnswerId",
                table: "QuizDetail",
                column: "QuizAnswerId",
                principalTable: "QuizAnswer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
