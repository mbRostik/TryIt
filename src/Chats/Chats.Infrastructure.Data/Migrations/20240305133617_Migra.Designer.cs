﻿// <auto-generated />
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Chats.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chats.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ChatDbContext))]
    [Migration("20240305133617_Migra")]
    partial class Migra
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Chats.Domain.Entities.CFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Data")
                        .IsRequired()
                        .HasMaxLength(10800333)
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("Id");

                    b.ToTable("CFiles");
                });

            modelBuilder.Entity("Chats.Domain.Entities.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("LastActivity")
                        .HasColumnType("datetime2");

                    b.Property<int>("LastMessageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LastMessageId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("Chats.Domain.Entities.ChatParticipant", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ChatId");

                    b.HasIndex("ChatId");

                    b.ToTable("ChatParticipants");
                });

            modelBuilder.Entity("Chats.Domain.Entities.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Chats.Domain.Entities.MessageWithFile", b =>
                {
                    b.Property<int>("FileId")
                        .HasColumnType("int");

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("FileId", "MessageId");

                    b.HasIndex("MessageId");

                    b.ToTable("MessageWithFiles");
                });

            modelBuilder.Entity("Chats.Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Chats.Domain.Entities.Chat", b =>
                {
                    b.HasOne("Chats.Domain.Entities.Message", "LastMessage")
                        .WithMany("Chats")
                        .HasForeignKey("LastMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LastMessage");
                });

            modelBuilder.Entity("Chats.Domain.Entities.ChatParticipant", b =>
                {
                    b.HasOne("Chats.Domain.Entities.Chat", "Chat")
                        .WithMany("ChatParticipants")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chats.Domain.Entities.User", "User")
                        .WithMany("ChatParticipants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Chats.Domain.Entities.Message", b =>
                {
                    b.HasOne("Chats.Domain.Entities.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Chats.Domain.Entities.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Chats.Domain.Entities.MessageWithFile", b =>
                {
                    b.HasOne("Chats.Domain.Entities.CFile", "File")
                        .WithMany("MessageWithFiles")
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chats.Domain.Entities.Message", "Message")
                        .WithMany("MessageWithFiles")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");

                    b.Navigation("Message");
                });

            modelBuilder.Entity("Chats.Domain.Entities.CFile", b =>
                {
                    b.Navigation("MessageWithFiles");
                });

            modelBuilder.Entity("Chats.Domain.Entities.Chat", b =>
                {
                    b.Navigation("ChatParticipants");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Chats.Domain.Entities.Message", b =>
                {
                    b.Navigation("Chats");

                    b.Navigation("MessageWithFiles");
                });

            modelBuilder.Entity("Chats.Domain.Entities.User", b =>
                {
                    b.Navigation("ChatParticipants");

                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}