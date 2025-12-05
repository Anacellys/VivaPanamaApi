using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VivaPanamaApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "imagen",
                columns: table => new
                {
                    id_imagen = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_entidad = table.Column<string>(type: "text", nullable: false),
                    id_entidad = table.Column<int>(type: "integer", nullable: false),
                    url_imagen = table.Column<string>(type: "text", nullable: false),
                    descripcion_imagen = table.Column<string>(type: "text", nullable: false),
                    es_principal = table.Column<bool>(type: "boolean", nullable: false),
                    fecha_subida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imagen", x => x.id_imagen);
                });

            migrationBuilder.CreateTable(
                name: "lugar",
                columns: table => new
                {
                    id_Lugar = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    provincia = table.Column<string>(type: "text", nullable: false),
                    tipo_lugar = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lugar", x => x.id_Lugar);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_usuario = table.Column<string>(type: "text", nullable: false),
                    cedula_pasaporte = table.Column<string>(type: "text", nullable: false),
                    edad_usuario = table.Column<int>(type: "integer", nullable: true),
                    email_usuario = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "actividad_lugar",
                columns: table => new
                {
                    id_actividad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_lugar = table.Column<int>(type: "integer", nullable: false),
                    nombre_actividad = table.Column<string>(type: "text", nullable: false),
                    descripcion_actividad = table.Column<string>(type: "text", nullable: false),
                    costo_actividad = table.Column<decimal>(type: "numeric", nullable: true),
                    duracion_estimada = table.Column<int>(type: "integer", nullable: true),
                    horario_apertura = table.Column<TimeSpan>(type: "interval", nullable: true),
                    horario_cierre = table.Column<TimeSpan>(type: "interval", nullable: true),
                    dificultad_actividad = table.Column<string>(type: "text", nullable: false),
                    equipo_requerido = table.Column<string>(type: "text", nullable: false),
                    restricciones = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividad_lugar", x => x.id_actividad);
                    table.ForeignKey(
                        name: "FK_actividad_lugar_lugar_id_lugar",
                        column: x => x.id_lugar,
                        principalTable: "lugar",
                        principalColumn: "id_Lugar",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hotel",
                columns: table => new
                {
                    id_hotel = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_lugar = table.Column<int>(type: "integer", nullable: false),
                    nombre_hotel = table.Column<string>(type: "text", nullable: false),
                    descripcion_hotel = table.Column<string>(type: "text", nullable: false),
                    precio_noche = table.Column<decimal>(type: "numeric", nullable: true),
                    calificacion_promedio = table.Column<decimal>(type: "numeric", nullable: true),
                    servicios_hotel = table.Column<string>(type: "text", nullable: false),
                    telefono_hotel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel", x => x.id_hotel);
                    table.ForeignKey(
                        name: "FK_hotel_lugar_id_lugar",
                        column: x => x.id_lugar,
                        principalTable: "lugar",
                        principalColumn: "id_Lugar",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "restaurante",
                columns: table => new
                {
                    id_restaurante = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_lugar = table.Column<int>(type: "integer", nullable: false),
                    nombre_restaurante = table.Column<string>(type: "text", nullable: false),
                    descripcion_restaurante = table.Column<string>(type: "text", nullable: false),
                    tipo_cocina_restaurante = table.Column<string>(type: "text", nullable: false),
                    precio_promedio = table.Column<decimal>(type: "numeric", nullable: true),
                    calificacion_promedio = table.Column<decimal>(type: "numeric", nullable: true),
                    horario_apertura = table.Column<TimeSpan>(type: "interval", nullable: true),
                    horario_cierre = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurante", x => x.id_restaurante);
                    table.ForeignKey(
                        name: "FK_restaurante_lugar_id_lugar",
                        column: x => x.id_lugar,
                        principalTable: "lugar",
                        principalColumn: "id_Lugar",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "calificacion",
                columns: table => new
                {
                    id_calificacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    tipo_entidad = table.Column<string>(type: "text", nullable: false),
                    id_entidad = table.Column<int>(type: "integer", nullable: false),
                    puntuacion = table.Column<int>(type: "integer", nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: true),
                    fecha_calificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuarioid_usuario = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_calificacion", x => x.id_calificacion);
                    table.ForeignKey(
                        name: "FK_calificacion_usuario_usuarioid_usuario",
                        column: x => x.usuarioid_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "favorito",
                columns: table => new
                {
                    id_favorito = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    tipo_entidad = table.Column<string>(type: "text", nullable: false),
                    id_entidad = table.Column<int>(type: "integer", nullable: false),
                    fecha_agregado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notas = table.Column<string>(type: "text", nullable: false),
                    Usuarioid_usuario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorito", x => x.id_favorito);
                    table.ForeignKey(
                        name: "FK_favorito_usuario_Usuarioid_usuario",
                        column: x => x.Usuarioid_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "itinerario",
                columns: table => new
                {
                    id_itinerario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    nombre_itinerario = table.Column<string>(type: "text", nullable: false),
                    descripcion_itinerario = table.Column<string>(type: "text", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    presupuesto_total = table.Column<decimal>(type: "numeric", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: false),
                    es_publico = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itinerario", x => x.id_itinerario);
                    table.ForeignKey(
                        name: "FK_itinerario_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "preferencias_usuario",
                columns: table => new
                {
                    id_preferencia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    presupuesto_maximo = table.Column<decimal>(type: "numeric", nullable: true),
                    tipo_viajero = table.Column<string>(type: "text", nullable: true),
                    intereses = table.Column<string>(type: "text", nullable: true),
                    nivel_actividad = table.Column<string>(type: "text", nullable: true),
                    tipo_alojamiento_preferido = table.Column<string>(type: "text", nullable: true),
                    usuarioid_usuario = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_preferencias_usuario", x => x.id_preferencia);
                    table.ForeignKey(
                        name: "FK_preferencias_usuario_usuario_usuarioid_usuario",
                        column: x => x.usuarioid_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "registro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_Usuario = table.Column<int>(type: "integer", nullable: false),
                    Id_Lugar = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "dia_itinerario",
                columns: table => new
                {
                    id_dia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_itinerario = table.Column<int>(type: "integer", nullable: false),
                    numero_dia = table.Column<int>(type: "integer", nullable: false),
                    fecha_dia = table.Column<DateOnly>(type: "date", nullable: true),
                    presupuesto_dia = table.Column<decimal>(type: "numeric", nullable: true),
                    notas_dia = table.Column<string>(type: "text", nullable: false),
                    Itinerarioid_itinerario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dia_itinerario", x => x.id_dia);
                    table.ForeignKey(
                        name: "FK_dia_itinerario_itinerario_Itinerarioid_itinerario",
                        column: x => x.Itinerarioid_itinerario,
                        principalTable: "itinerario",
                        principalColumn: "id_itinerario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "actividad_itinerario",
                columns: table => new
                {
                    id_actividad_itinerario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_dia = table.Column<int>(type: "integer", nullable: false),
                    id_actividad = table.Column<int>(type: "integer", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    hora_inicio = table.Column<TimeSpan>(type: "interval", nullable: true),
                    hora_fin = table.Column<TimeSpan>(type: "interval", nullable: true),
                    notas_actividad = table.Column<string>(type: "text", nullable: false),
                    estado = table.Column<string>(type: "text", nullable: false),
                    costo_real = table.Column<decimal>(type: "numeric", nullable: true),
                    diaid_dia = table.Column<int>(type: "integer", nullable: false),
                    actividadid_actividad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actividad_itinerario", x => x.id_actividad_itinerario);
                    table.ForeignKey(
                        name: "FK_actividad_itinerario_actividad_lugar_actividadid_actividad",
                        column: x => x.actividadid_actividad,
                        principalTable: "actividad_lugar",
                        principalColumn: "id_actividad",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_actividad_itinerario_dia_itinerario_diaid_dia",
                        column: x => x.diaid_dia,
                        principalTable: "dia_itinerario",
                        principalColumn: "id_dia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hotel_itinerario",
                columns: table => new
                {
                    id_hotel_itinerario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_dia = table.Column<int>(type: "integer", nullable: false),
                    id_hotel = table.Column<int>(type: "integer", nullable: false),
                    fecha_checkin = table.Column<DateOnly>(type: "date", nullable: true),
                    fecha_checkout = table.Column<DateOnly>(type: "date", nullable: true),
                    numero_noches = table.Column<int>(type: "integer", nullable: true),
                    costo_total = table.Column<decimal>(type: "numeric", nullable: true),
                    notas_hospedaje = table.Column<string>(type: "text", nullable: true),
                    dia_itinerarioid_dia = table.Column<int>(type: "integer", nullable: true),
                    hotelid_hotel = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_itinerario", x => x.id_hotel_itinerario);
                    table.ForeignKey(
                        name: "FK_hotel_itinerario_dia_itinerario_dia_itinerarioid_dia",
                        column: x => x.dia_itinerarioid_dia,
                        principalTable: "dia_itinerario",
                        principalColumn: "id_dia");
                    table.ForeignKey(
                        name: "FK_hotel_itinerario_hotel_hotelid_hotel",
                        column: x => x.hotelid_hotel,
                        principalTable: "hotel",
                        principalColumn: "id_hotel");
                });

            migrationBuilder.CreateTable(
                name: "lugar_itinerario",
                columns: table => new
                {
                    id_lugar_itinerario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_dia = table.Column<int>(type: "integer", nullable: false),
                    id_lugar = table.Column<int>(type: "integer", nullable: false),
                    orden_visita = table.Column<int>(type: "integer", nullable: true),
                    tiempo_estimado_visita = table.Column<int>(type: "integer", nullable: true),
                    notas_visita = table.Column<string>(type: "text", nullable: true),
                    dia_itinerarioid_dia = table.Column<int>(type: "integer", nullable: true),
                    lugarid_Lugar = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lugar_itinerario", x => x.id_lugar_itinerario);
                    table.ForeignKey(
                        name: "FK_lugar_itinerario_dia_itinerario_dia_itinerarioid_dia",
                        column: x => x.dia_itinerarioid_dia,
                        principalTable: "dia_itinerario",
                        principalColumn: "id_dia");
                    table.ForeignKey(
                        name: "FK_lugar_itinerario_lugar_lugarid_Lugar",
                        column: x => x.lugarid_Lugar,
                        principalTable: "lugar",
                        principalColumn: "id_Lugar");
                });

            migrationBuilder.CreateTable(
                name: "restaurante_itinerario",
                columns: table => new
                {
                    id_restaurante_itinerario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_dia = table.Column<int>(type: "integer", nullable: false),
                    id_restaurante = table.Column<int>(type: "integer", nullable: false),
                    tipo_comida = table.Column<string>(type: "text", nullable: true),
                    hora_reserva = table.Column<TimeSpan>(type: "interval", nullable: true),
                    numero_personas = table.Column<int>(type: "integer", nullable: true),
                    costo_estimado = table.Column<decimal>(type: "numeric", nullable: true),
                    notas_restaurante = table.Column<string>(type: "text", nullable: true),
                    dia_itinerarioid_dia = table.Column<int>(type: "integer", nullable: true),
                    restauranteid_restaurante = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_restaurante_itinerario", x => x.id_restaurante_itinerario);
                    table.ForeignKey(
                        name: "FK_restaurante_itinerario_dia_itinerario_dia_itinerarioid_dia",
                        column: x => x.dia_itinerarioid_dia,
                        principalTable: "dia_itinerario",
                        principalColumn: "id_dia");
                    table.ForeignKey(
                        name: "FK_restaurante_itinerario_restaurante_restauranteid_restaurante",
                        column: x => x.restauranteid_restaurante,
                        principalTable: "restaurante",
                        principalColumn: "id_restaurante");
                });

            migrationBuilder.CreateIndex(
                name: "IX_actividad_itinerario_actividadid_actividad",
                table: "actividad_itinerario",
                column: "actividadid_actividad");

            migrationBuilder.CreateIndex(
                name: "IX_actividad_itinerario_diaid_dia",
                table: "actividad_itinerario",
                column: "diaid_dia");

            migrationBuilder.CreateIndex(
                name: "IX_actividad_lugar_id_lugar",
                table: "actividad_lugar",
                column: "id_lugar");

            migrationBuilder.CreateIndex(
                name: "IX_calificacion_usuarioid_usuario",
                table: "calificacion",
                column: "usuarioid_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_dia_itinerario_Itinerarioid_itinerario",
                table: "dia_itinerario",
                column: "Itinerarioid_itinerario");

            migrationBuilder.CreateIndex(
                name: "IX_favorito_Usuarioid_usuario",
                table: "favorito",
                column: "Usuarioid_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_id_lugar",
                table: "hotel",
                column: "id_lugar");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_itinerario_dia_itinerarioid_dia",
                table: "hotel_itinerario",
                column: "dia_itinerarioid_dia");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_itinerario_hotelid_hotel",
                table: "hotel_itinerario",
                column: "hotelid_hotel");

            migrationBuilder.CreateIndex(
                name: "IX_itinerario_id_usuario",
                table: "itinerario",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_lugar_itinerario_dia_itinerarioid_dia",
                table: "lugar_itinerario",
                column: "dia_itinerarioid_dia");

            migrationBuilder.CreateIndex(
                name: "IX_lugar_itinerario_lugarid_Lugar",
                table: "lugar_itinerario",
                column: "lugarid_Lugar");

            migrationBuilder.CreateIndex(
                name: "IX_preferencias_usuario_usuarioid_usuario",
                table: "preferencias_usuario",
                column: "usuarioid_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_registro_Id_Lugar",
                table: "registro",
                column: "Id_Lugar");

            migrationBuilder.CreateIndex(
                name: "IX_registro_Id_Usuario",
                table: "registro",
                column: "Id_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_restaurante_id_lugar",
                table: "restaurante",
                column: "id_lugar");

            migrationBuilder.CreateIndex(
                name: "IX_restaurante_itinerario_dia_itinerarioid_dia",
                table: "restaurante_itinerario",
                column: "dia_itinerarioid_dia");

            migrationBuilder.CreateIndex(
                name: "IX_restaurante_itinerario_restauranteid_restaurante",
                table: "restaurante_itinerario",
                column: "restauranteid_restaurante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "actividad_itinerario");

            migrationBuilder.DropTable(
                name: "calificacion");

            migrationBuilder.DropTable(
                name: "favorito");

            migrationBuilder.DropTable(
                name: "hotel_itinerario");

            migrationBuilder.DropTable(
                name: "imagen");

            migrationBuilder.DropTable(
                name: "lugar_itinerario");

            migrationBuilder.DropTable(
                name: "preferencias_usuario");

            migrationBuilder.DropTable(
                name: "registro");

            migrationBuilder.DropTable(
                name: "restaurante_itinerario");

            migrationBuilder.DropTable(
                name: "actividad_lugar");

            migrationBuilder.DropTable(
                name: "hotel");

            migrationBuilder.DropTable(
                name: "dia_itinerario");

            migrationBuilder.DropTable(
                name: "restaurante");

            migrationBuilder.DropTable(
                name: "itinerario");

            migrationBuilder.DropTable(
                name: "lugar");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
