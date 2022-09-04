using BookStore.Utility;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.DataAccess.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), SD.IndividualUserRole, SD.IndividualUserRole, Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), SD.CompanyUserRole, SD.CompanyUserRole, Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), SD.AdminRole, SD.AdminRole, Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), SD.EmployeeRole, SD.EmployeeRole, Guid.NewGuid().ToString() }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[Roles]");
        }
    }
}
