using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using IBSVF.Web.Data;

#nullable disable

namespace IBSVF.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250806000001_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "participantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    comparecimento = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participantes", x => x.id);
                    table.CheckConstraint("CK_participantes_comparecimento", "comparecimento IN ('yes', 'no')");
                });

            migrationBuilder.CreateTable(
                name: "acompanhantes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    participante_id = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_acompanhantes", x => x.id);
                    table.ForeignKey(
                        name: "FK_acompanhantes_participantes_participante_id",
                        column: x => x.participante_id,
                        principalTable: "participantes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Inserir usuário padrão
            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "username", "password" },
                values: new object[] { "admin", "admin123" }
            );

            // Criar índices
            migrationBuilder.CreateIndex(
                name: "IX_acompanhantes_participante_id",
                table: "acompanhantes",
                column: "participante_id");

            migrationBuilder.CreateIndex(
                name: "IX_participantes_nome",
                table: "participantes",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "IX_participantes_comparecimento",
                table: "participantes",
                column: "comparecimento");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_username",
                table: "usuarios",
                column: "username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "acompanhantes");

            migrationBuilder.DropTable(
                name: "participantes");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
