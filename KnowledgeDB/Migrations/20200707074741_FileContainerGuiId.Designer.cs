﻿// <auto-generated />
using System;
using KnowledgeDB.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KnowledgeDB.Migrations
{
    [DbContext(typeof(EFContext))]
    [Migration("20200707074741_FileContainerGuiId")]
    partial class FileContainerGuiId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("KnowledgeDB.Models.Article", b =>
                {
                    b.Property<int>("ArticleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ArticleImagePath")
                        .HasColumnType("text");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<string>("TeaserImagePath")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ArticleId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("KnowledgeDB.Models.ArticleTag", b =>
                {
                    b.Property<int>("ArticleTagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("ArticleTagId");

                    b.ToTable("ArticleTags");
                });

            modelBuilder.Entity("KnowledgeDB.Models.FileContainer", b =>
                {
                    b.Property<int>("FileContainerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FileDescription")
                        .HasColumnType("text");

                    b.Property<string>("FileDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("FilePathFull")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileType")
                        .HasColumnType("text");

                    b.Property<string>("GuiId")
                        .HasColumnType("text");

                    b.HasKey("FileContainerId");

                    b.ToTable("FileContainers");
                });

            modelBuilder.Entity("KnowledgeDB.Models.RefArticleArticleTag", b =>
                {
                    b.Property<int>("ArticleTagId")
                        .HasColumnType("integer");

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.HasKey("ArticleTagId", "ArticleId");

                    b.HasIndex("ArticleId");

                    b.ToTable("RefArticleArticleTag");
                });

            modelBuilder.Entity("KnowledgeDB.Models.RefArticleArticleTag", b =>
                {
                    b.HasOne("KnowledgeDB.Models.Article", "Article")
                        .WithMany("RefToTags")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KnowledgeDB.Models.ArticleTag", "ArticelTag")
                        .WithMany("RefToArticles")
                        .HasForeignKey("ArticleTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
