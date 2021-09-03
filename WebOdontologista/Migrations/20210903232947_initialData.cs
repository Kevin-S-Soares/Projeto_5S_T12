using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebOdontologista.Migrations
{
    public partial class initialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dentist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    TelephoneNumber = table.Column<string>(maxLength: 15, nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dentist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DurationInMinutes = table.Column<int>(nullable: false),
                    Patient = table.Column<string>(maxLength: 60, nullable: false),
                    TelephoneNumber = table.Column<string>(maxLength: 15, nullable: false),
                    AppointmentType = table.Column<string>(maxLength: 60, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Time = table.Column<TimeSpan>(nullable: false),
                    DentistId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_Dentist_DentistId",
                        column: x => x.DentistId,
                        principalTable: "Dentist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DentistId",
                table: "Appointment",
                column: "DentistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Dentist");
        }
    }
}
