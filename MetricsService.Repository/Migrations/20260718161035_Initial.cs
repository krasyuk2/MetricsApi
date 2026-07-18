using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MetricsApi.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DeltaTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    FirstStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AvgDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ValueMean = table.Column<double>(type: "double precision", nullable: false),
                    ValueMedian = table.Column<double>(type: "double precision", nullable: false),
                    ValueMin = table.Column<double>(type: "double precision", nullable: false),
                    ValueMax = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.FileName);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_AvgDuration",
                table: "Results",
                column: "AvgDuration");

            migrationBuilder.CreateIndex(
                name: "IX_Results_FirstStartTime",
                table: "Results",
                column: "FirstStartTime");

            migrationBuilder.CreateIndex(
                name: "IX_Results_ValueMean",
                table: "Results",
                column: "ValueMean");

            migrationBuilder.CreateIndex(
                name: "IX_Values_FileName_StartTime",
                table: "Values",
                columns: new[] { "FileName", "StartTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Values");
        }
    }
}
