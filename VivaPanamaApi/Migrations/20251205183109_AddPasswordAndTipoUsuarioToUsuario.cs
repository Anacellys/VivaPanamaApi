using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VivaPanamaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordAndTipoUsuarioToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_calificacion_usuario_usuarioid_usuario",
                table: "calificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_favorito_usuario_Usuarioid_usuario",
                table: "favorito");

            migrationBuilder.DropForeignKey(
                name: "FK_preferencias_usuario_usuario_usuarioid_usuario",
                table: "preferencias_usuario");

            migrationBuilder.DropTable(
                name: "registro");

            migrationBuilder.DropIndex(
                name: "IX_preferencias_usuario_usuarioid_usuario",
                table: "preferencias_usuario");

            migrationBuilder.DropIndex(
                name: "IX_favorito_Usuarioid_usuario",
                table: "favorito");

            migrationBuilder.DropIndex(
                name: "IX_calificacion_usuarioid_usuario",
                table: "calificacion");

            migrationBuilder.DropColumn(
                name: "usuarioid_usuario",
                table: "preferencias_usuario");

            migrationBuilder.DropColumn(
                name: "Usuarioid_usuario",
                table: "favorito");

            migrationBuilder.DropColumn(
                name: "usuarioid_usuario",
                table: "calificacion");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "usuario",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tipo_usuario",
                table: "usuario",
                type: "text",
                nullable: false,
                defaultValue: "cliente");

            migrationBuilder.AlterColumn<string>(
                name: "notas",
                table: "favorito",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_nombre_usuario",
                table: "usuario",
                column: "nombre_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_preferencias_usuario_id_usuario",
                table: "preferencias_usuario",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_favorito_id_usuario",
                table: "favorito",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_calificacion_id_usuario",
                table: "calificacion",
                column: "id_usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_calificacion_usuario_id_usuario",
                table: "calificacion",
                column: "id_usuario",
                principalTable: "usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorito_usuario_id_usuario",
                table: "favorito",
                column: "id_usuario",
                principalTable: "usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_preferencias_usuario_usuario_id_usuario",
                table: "preferencias_usuario",
                column: "id_usuario",
                principalTable: "usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_calificacion_usuario_id_usuario",
                table: "calificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_favorito_usuario_id_usuario",
                table: "favorito");

            migrationBuilder.DropForeignKey(
                name: "FK_preferencias_usuario_usuario_id_usuario",
                table: "preferencias_usuario");

            migrationBuilder.DropIndex(
                name: "IX_usuario_nombre_usuario",
                table: "usuario");

            migrationBuilder.DropIndex(
                name: "IX_preferencias_usuario_id_usuario",
                table: "preferencias_usuario");

            migrationBuilder.DropIndex(
                name: "IX_favorito_id_usuario",
                table: "favorito");

            migrationBuilder.DropIndex(
                name: "IX_calificacion_id_usuario",
                table: "calificacion");

            migrationBuilder.DropColumn(
                name: "password",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "tipo_usuario",
                table: "usuario");

            migrationBuilder.AddColumn<int>(
                name: "usuarioid_usuario",
                table: "preferencias_usuario",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "notas",
                table: "favorito",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Usuarioid_usuario",
                table: "favorito",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "usuarioid_usuario",
                table: "calificacion",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "registro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Lugar = table.Column<int>(type: "integer", nullable: false),
                    Id_Usuario = table.Column<int>(type: "integer", nullable: false),
                    Fecha_Entrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Fecha_Salida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registro_Lugar",
                        column: x => x.Id_Lugar,
                        principalTable: "lugar",
                        principalColumn: "id_Lugar",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registro_Usuario",
                        column: x => x.Id_Usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_preferencias_usuario_usuarioid_usuario",
                table: "preferencias_usuario",
                column: "usuarioid_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_favorito_Usuarioid_usuario",
                table: "favorito",
                column: "Usuarioid_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_calificacion_usuarioid_usuario",
                table: "calificacion",
                column: "usuarioid_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_registro_Id_Lugar",
                table: "registro",
                column: "Id_Lugar");

            migrationBuilder.CreateIndex(
                name: "IX_registro_Id_Usuario",
                table: "registro",
                column: "Id_Usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_calificacion_usuario_usuarioid_usuario",
                table: "calificacion",
                column: "usuarioid_usuario",
                principalTable: "usuario",
                principalColumn: "id_usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_favorito_usuario_Usuarioid_usuario",
                table: "favorito",
                column: "Usuarioid_usuario",
                principalTable: "usuario",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_preferencias_usuario_usuario_usuarioid_usuario",
                table: "preferencias_usuario",
                column: "usuarioid_usuario",
                principalTable: "usuario",
                principalColumn: "id_usuario");
        }
    }
}
