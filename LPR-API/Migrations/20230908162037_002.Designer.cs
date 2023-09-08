﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Prom.LPR.Api.DBContext;

#nullable disable

namespace LPR_API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20230908162037_002")]
    partial class _002
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Prom.LPR.Api.Models.MOrganization", b =>
                {
                    b.Property<Guid?>("OrgId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("org_id");

                    b.Property<DateTime?>("OrgCreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("org_created_date");

                    b.Property<string>("OrgDescription")
                        .HasColumnType("text")
                        .HasColumnName("org_description");

                    b.Property<string>("OrgName")
                        .HasColumnType("text")
                        .HasColumnName("org_name");

                    b.HasKey("OrgId");

                    b.ToTable("Organizations");
                });
#pragma warning restore 612, 618
        }
    }
}
