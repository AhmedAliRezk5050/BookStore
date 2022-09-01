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
                    values: new object[] { Guid.NewGuid().ToString(), SD.Role_User_Indi, SD.Role_User_Indi, Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), SD.Role_User_Comp, SD.Role_User_Comp, Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), SD.Role_Admin, SD.Role_Admin, Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                    table: "AspNetRoles",
                    columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                    values: new object[] { Guid.NewGuid().ToString(), SD.Role_Employee, SD.Role_Employee, Guid.NewGuid().ToString() }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [dbo].[Roles]");
        }
    }
}
