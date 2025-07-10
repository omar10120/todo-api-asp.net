using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ayagroup_SMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxPoints",
                table: "Assignments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPoints",
                table: "Assignments");
        }
    }
}
