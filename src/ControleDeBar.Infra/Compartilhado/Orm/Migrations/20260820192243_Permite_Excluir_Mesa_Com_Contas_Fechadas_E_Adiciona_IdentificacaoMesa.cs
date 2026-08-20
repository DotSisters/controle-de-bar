using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeBar.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Permite_Excluir_Mesa_Com_Contas_Fechadas_E_Adiciona_IdentificacaoMesa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBConta_TBMesa_MesaId",
                table: "TBConta");

            migrationBuilder.AddColumn<string>(
                name: "IdentificacaoMesa",
                table: "TBConta",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE c SET IdentificacaoMesa = m.Identificacao
                FROM TBConta c
                INNER JOIN TBMesa m ON m.Id = c.MesaId
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "MesaId",
                table: "TBConta",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_TBConta_TBMesa_MesaId",
                table: "TBConta",
                column: "MesaId",
                principalTable: "TBMesa",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBConta_TBMesa_MesaId",
                table: "TBConta");

            migrationBuilder.DropColumn(
                name: "IdentificacaoMesa",
                table: "TBConta");

            migrationBuilder.AlterColumn<Guid>(
                name: "MesaId",
                table: "TBConta",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TBConta_TBMesa_MesaId",
                table: "TBConta",
                column: "MesaId",
                principalTable: "TBMesa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
