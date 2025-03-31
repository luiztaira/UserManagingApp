﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserManagingApp.Migrations
{
    [DbContext(typeof(UserManagerContext))]
    partial class DatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UserManagingApp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<string>("Privileges")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            Email = "luiztaira@example.com",
                            LastUpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Luiz Taira",
                            PhoneNumber = "+1234567890",
                            Privileges = "admin"
                        },
                        new
                        {
                            Id = 2,
                            CreatedAt = new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                            Email = "yagamitaichi@example.com",
                            LastUpdatedAt = new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Yagami Taichi",
                            PhoneNumber = "+9876543210",
                            Privileges = "user"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
