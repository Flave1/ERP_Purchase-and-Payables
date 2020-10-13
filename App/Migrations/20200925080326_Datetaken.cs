using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puchase_and_payables.Migrations
{
    public partial class Datetaken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTaken",
                table: "purch_plpo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTaken",
                table: "purch_plpo");
        }
    }
}
