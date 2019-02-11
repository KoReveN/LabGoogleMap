using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarkerIcons",
                columns: table => new
                {
                    MarkerIconId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Color = table.Column<string>(nullable: true),
                    IconUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkerIcons", x => x.MarkerIconId);
                });

            migrationBuilder.CreateTable(
                name: "Point",
                columns: table => new
                {
                    PointId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Lat = table.Column<float>(nullable: false),
                    Lng = table.Column<float>(nullable: false),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point", x => x.PointId);
                });

            migrationBuilder.CreateTable(
                name: "RouteLegs",
                columns: table => new
                {
                    RouteLegId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartPoint = table.Column<int>(nullable: false),
                    EndPoint = table.Column<int>(nullable: false),
                    Distance = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    Polyline = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteLegs", x => x.RouteLegId);
                });

            migrationBuilder.CreateTable(
                name: "Markers",
                columns: table => new
                {
                    MarkerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<int>(nullable: false),
                    PointId = table.Column<int>(nullable: false),
                    MarkerIconId = table.Column<int>(nullable: false),
                    MarkerType = table.Column<int>(nullable: false),
                    Index = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markers", x => x.MarkerId);
                    table.ForeignKey(
                        name: "FK_Markers_MarkerIcons_MarkerIconId",
                        column: x => x.MarkerIconId,
                        principalTable: "MarkerIcons",
                        principalColumn: "MarkerIconId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Markers_Point_PointId",
                        column: x => x.PointId,
                        principalTable: "Point",
                        principalColumn: "PointId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Markers_MarkerIconId",
                table: "Markers",
                column: "MarkerIconId");

            migrationBuilder.CreateIndex(
                name: "IX_Markers_PointId",
                table: "Markers",
                column: "PointId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Markers");

            migrationBuilder.DropTable(
                name: "RouteLegs");

            migrationBuilder.DropTable(
                name: "MarkerIcons");

            migrationBuilder.DropTable(
                name: "Point");
        }
    }
}
