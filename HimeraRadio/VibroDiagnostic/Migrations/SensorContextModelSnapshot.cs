﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VibroDiagnostic.Data.Contexts;

#nullable disable

namespace VibroDiagnostic.Migrations
{
    [DbContext(typeof(SensorContext))]
    partial class SensorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.Equipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EquipmentId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EquipmentId");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.SensorFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsLatest")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SensorId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("SensorFiles");
                });

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("isActive")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FirstName = "System",
                            LastName = "",
                            Password = "System",
                            Username = "System",
                            isActive = false
                        });
                });

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.Sensor", b =>
                {
                    b.HasOne("VibroDiagnostic.Core.Entities.Equipment", "Equipment")
                        .WithMany("Sensors")
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equipment");
                });

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.SensorFile", b =>
                {
                    b.HasOne("VibroDiagnostic.Core.Entities.Sensor", "Sensor")
                        .WithMany("Files")
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sensor");
                });

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.Equipment", b =>
                {
                    b.Navigation("Sensors");
                });

            modelBuilder.Entity("VibroDiagnostic.Core.Entities.Sensor", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
