﻿// <auto-generated />
using AaronUrkinCodeExample.DataAccessLayer.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace AaronUrkinCodeExample.DataAccessLayer.Logger.Migrations
{
    [DbContext(typeof(LoggerDbContext))]
    partial class LoggerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("exmpl_log")
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AaronUrkinCodeExample.DataAccessLayer.Logger.Entities.LogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("Logger")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Message");

                    b.Property<string>("StackTrace");

                    b.HasKey("Id");

                    b.ToTable("LogEntries");
                });
#pragma warning restore 612, 618
        }
    }
}