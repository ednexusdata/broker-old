using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OregonNexus.Broker.Data.Migrations.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddEdOrgConnectorSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducationOrganizationConnectorSettings",
                columns: table => new
                {
                    EducationOrganizationConnectorSettingsId = table.Column<Guid>(type: "uuid", nullable: false),
                    EducationOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Connector = table.Column<string>(type: "text", nullable: false),
                    Settings = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationOrganizationConnectorSettings", x => x.EducationOrganizationConnectorSettingsId);
                    table.ForeignKey(
                        name: "FK_EducationOrganizationConnectorSettings_EducationOrganizatio~",
                        column: x => x.EducationOrganizationId,
                        principalTable: "EducationOrganizations",
                        principalColumn: "EducationOrganizationId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationOrganizationConnectorSettings_EducationOrganizatio~",
                table: "EducationOrganizationConnectorSettings",
                columns: new[] { "EducationOrganizationId", "Connector" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationOrganizationConnectorSettings");
        }
    }
}
