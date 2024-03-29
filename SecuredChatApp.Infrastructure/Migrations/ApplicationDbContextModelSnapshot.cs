﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SecuredChatApp.Infrastructure;

namespace SecuredChatApp.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("SecuredChatApp.Core.Entities.FriendEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Creator")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRequest")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ReceiverID")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SenderUserID")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("SecuredChatApp.Core.Entities.MessageBoxEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Creator")
                        .HasColumnType("text");

                    b.Property<string>("FromPublicKey")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("NumberG")
                        .HasColumnType("text");

                    b.Property<string>("NumberP")
                        .HasColumnType("text");

                    b.Property<Guid>("Sender")
                        .HasColumnType("uuid");

                    b.Property<Guid>("To")
                        .HasColumnType("uuid");

                    b.Property<string>("ToPublicKey")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("MessageBoxes");
                });

            modelBuilder.Entity("SecuredChatApp.Core.Entities.MessageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Creator")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.Property<bool>("Read")
                        .HasColumnType("boolean");

                    b.Property<Guid>("Sender")
                        .HasColumnType("uuid");

                    b.Property<Guid>("To")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SecuredChatApp.Core.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Creator")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("ProfileImageUrl")
                        .HasColumnType("text");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RefreshTokenExpireDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
