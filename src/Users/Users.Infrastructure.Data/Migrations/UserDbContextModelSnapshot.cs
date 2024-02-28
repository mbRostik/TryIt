﻿// <auto-generated />
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Users.Infrastructure.Data;

#nullable disable

namespace Users.Infrastructure.Data.Migrations
{
    [DbContext(typeof(UserDbContext))]
    partial class UserDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Users.Domain.Entities.BannedUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BanReason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModeratorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ModeratorId");

                    b.HasIndex("UserId");

                    b.ToTable("BannedUsers");
                });

            modelBuilder.Entity("Users.Domain.Entities.Follow", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FollowerId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "FollowerId");

                    b.HasIndex("FollowerId");

                    b.ToTable("Follows");
                });

            modelBuilder.Entity("Users.Domain.Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Users.Domain.Entities.SavedPost", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PostId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "PostId");

                    b.HasIndex("PostId");

                    b.ToTable("SavedPosts");
                });

            modelBuilder.Entity("Users.Domain.Entities.Sex", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("DatabaseGenerated", DatabaseGeneratedOption.Identity);

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SexType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Sexs");
                });

            modelBuilder.Entity("Users.Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Bio")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Photo")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("SexId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SexId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Users.Domain.Entities.BannedUser", b =>
                {
                    b.HasOne("Users.Domain.Entities.User", "BannedByUser")
                        .WithMany("BannedBy")
                        .HasForeignKey("ModeratorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Users.Domain.Entities.User", "User")
                        .WithMany("BannedUsers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("BannedByUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Users.Domain.Entities.Follow", b =>
                {
                    b.HasOne("Users.Domain.Entities.User", "Follower")
                        .WithMany("Followers")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Users.Domain.Entities.User", "User")
                        .WithMany("Follows")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Follower");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Users.Domain.Entities.SavedPost", b =>
                {
                    b.HasOne("Users.Domain.Entities.Post", "Post")
                        .WithMany("SavedPosts")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Users.Domain.Entities.User", "User")
                        .WithMany("SavedPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Users.Domain.Entities.User", b =>
                {
                    b.HasOne("Users.Domain.Entities.Sex", "Sex")
                        .WithMany("Users")
                        .HasForeignKey("SexId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Sex");
                });

            modelBuilder.Entity("Users.Domain.Entities.Post", b =>
                {
                    b.Navigation("SavedPosts");
                });

            modelBuilder.Entity("Users.Domain.Entities.Sex", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Users.Domain.Entities.User", b =>
                {
                    b.Navigation("BannedBy");

                    b.Navigation("BannedUsers");

                    b.Navigation("Followers");

                    b.Navigation("Follows");

                    b.Navigation("SavedPosts");
                });
#pragma warning restore 612, 618
        }
    }
}
