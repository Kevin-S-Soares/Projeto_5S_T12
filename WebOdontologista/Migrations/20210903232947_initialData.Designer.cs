﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebOdontologista.Data;

namespace WebOdontologista.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210903232947_initialData")]
    partial class initialData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebOdontologista.Models.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AppointmentType")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.Property<DateTime>("Date");

                    b.Property<int>("DentistId");

                    b.Property<int>("DurationInMinutes");

                    b.Property<string>("Patient")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.Property<string>("TelephoneNumber")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.Property<TimeSpan>("Time");

                    b.HasKey("Id");

                    b.HasIndex("DentistId");

                    b.ToTable("Appointment");
                });

            modelBuilder.Entity("WebOdontologista.Models.Dentist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.Property<string>("TelephoneNumber")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.HasKey("Id");

                    b.ToTable("Dentist");
                });

            modelBuilder.Entity("WebOdontologista.Models.Appointment", b =>
                {
                    b.HasOne("WebOdontologista.Models.Dentist", "Dentist")
                        .WithMany()
                        .HasForeignKey("DentistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
