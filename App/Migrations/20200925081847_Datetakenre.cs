using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puchase_and_payables.Migrations
{
    public partial class Datetakenre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTaken",
                table: "purch_plpo");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCompleted",
                table: "purch_plpo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCompleted",
                table: "purch_plpo");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTaken",
                table: "purch_plpo",
                type: "datetime2",
                nullable: true);
        }
    }
}
