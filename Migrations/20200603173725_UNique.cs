using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhotoParse.Migrations
{
    public partial class UNique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniquePhoto",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Location = table.Column<string>(nullable: true),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    Ext = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    HResolution = table.Column<string>(nullable: true),
                    VResolution = table.Column<string>(nullable: true),
                    CameraMaker = table.Column<string>(nullable: true),
                    CameraModel = table.Column<string>(nullable: true),
                    FocalLength = table.Column<string>(nullable: true),
                    MeteringMode = table.Column<string>(nullable: true),
                    Flash = table.Column<string>(nullable: true),
                    ThumbnailLength = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Longitude = table.Column<string>(nullable: true),
                    Size = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniquePhoto", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniquePhoto");
        }
    }
}
