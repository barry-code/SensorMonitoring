﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SensorMonitoring.Api.Repository;

#nullable disable

namespace SensorMonitoring.Api.Migrations
{
    [DbContext(typeof(SensorContext))]
    partial class SensorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.21");

            modelBuilder.Entity("SensorMonitoring.Shared.Models.Sensor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("Delta")
                        .HasColumnType("REAL");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("SensorMonitoring.Shared.Models.SensorReading", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("DateTime")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SensorId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("SensorId");

                    b.ToTable("SensorReadings");
                });

            modelBuilder.Entity("SensorMonitoring.Shared.Models.SensorReading", b =>
                {
                    b.HasOne("SensorMonitoring.Shared.Models.Sensor", "Sensor")
                        .WithMany()
                        .HasForeignKey("SensorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sensor");
                });
#pragma warning restore 612, 618
        }
    }
}
