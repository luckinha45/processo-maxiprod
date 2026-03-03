using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class CategoriaInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Idade_Range",
                table: "Pessoas");

            migrationBuilder.AlterColumn<int>(
                name: "Finalidade",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Finalidade",
                table: "Categorias",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Idade_Range",
                table: "Pessoas",
                sql: "Idade >= 0 AND Idade <= 150");
        }
    }
}
