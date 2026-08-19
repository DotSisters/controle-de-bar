using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeBar.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Permite_Excluir_Garcom_Com_Contas_Fechadas_E_Adiciona_NomeGarcom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBConta_TBGarcom_GarcomId",
                table: "TBConta");

            migrationBuilder.AddColumn<string>(
                name: "NomeGarcom",
                table: "TBConta",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE c SET NomeGarcom = g.Nome
                FROM TBConta c
                INNER JOIN TBGarcom g ON g.Id = c.GarcomId
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "GarcomId",
                table: "TBConta",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_TBConta_TBGarcom_GarcomId",
                table: "TBConta",
                column: "GarcomId",
                principalTable: "TBGarcom",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBConta_TBGarcom_GarcomId",
                table: "TBConta");

            migrationBuilder.DropColumn(
                name: "NomeGarcom",
                table: "TBConta");

            migrationBuilder.AlterColumn<Guid>(
                name: "GarcomId",
                table: "TBConta",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TBConta_TBGarcom_GarcomId",
                table: "TBConta",
                column: "GarcomId",
                principalTable: "TBGarcom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
