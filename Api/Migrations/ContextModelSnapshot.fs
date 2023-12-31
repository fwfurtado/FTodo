﻿// <auto-generated />
namespace Api.Migrations

open System
open Data.Todo
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Infrastructure
open Microsoft.EntityFrameworkCore.Metadata
open Microsoft.EntityFrameworkCore.Migrations
open Microsoft.EntityFrameworkCore.Storage.ValueConversion

[<DbContext(typeof<Context>)>]
type ContextModelSnapshot() =
    inherit ModelSnapshot()

    override this.BuildModel(modelBuilder: ModelBuilder) =
        modelBuilder.HasAnnotation("ProductVersion", "6.0.19") |> ignore

        modelBuilder.Entity("Data.Todo+DB+TodoDB", (fun b ->

            b.Property<int>("Id")
                .IsRequired(true)
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasColumnName("id") |> ignore

            b.Property<string>("Description")
                .IsRequired(true)
                .HasColumnType("TEXT")
                .HasColumnName("description") |> ignore

            b.Property<string>("State")
                .IsRequired(true)
                .HasColumnType("TEXT")
                .HasColumnName("state") |> ignore

            b.Property<string>("Title")
                .IsRequired(true)
                .HasColumnType("TEXT")
                .HasColumnName("title") |> ignore

            b.HasKey("Id")
                |> ignore


            b.ToTable("todos") |> ignore

        )) |> ignore

