//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace DapperSample.Infrastructure.Migrations
//{
//    public partial class inittt : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.CreateTable(
//                name: "Categories",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Categories", x => x.Id);
//                });

//            migrationBuilder.CreateTable(
//                name: "Tag",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Tag", x => x.Id);
//                });

//            migrationBuilder.CreateTable(
//                name: "Products",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
//                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
//                    CategoryId = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_Products", x => x.Id);
//                    table.ForeignKey(
//                        name: "FK_Products_Categories_CategoryId",
//                        column: x => x.CategoryId,
//                        principalTable: "Categories",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateTable(
//                name: "ProductTag",
//                columns: table => new
//                {
//                    ProductsId = table.Column<int>(type: "int", nullable: false),
//                    TagsId = table.Column<int>(type: "int", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ProductTag", x => new { x.ProductsId, x.TagsId });
//                    table.ForeignKey(
//                        name: "FK_ProductTag_Products_ProductsId",
//                        column: x => x.ProductsId,
//                        principalTable: "Products",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_ProductTag_Tag_TagsId",
//                        column: x => x.TagsId,
//                        principalTable: "Tag",
//                        principalColumn: "Id",
//                        onDelete: ReferentialAction.Cascade);
//                });

//            migrationBuilder.CreateIndex(
//                name: "IX_Products_CategoryId",
//                table: "Products",
//                column: "CategoryId");

//            migrationBuilder.CreateIndex(
//                name: "IX_ProductTag_TagsId",
//                table: "ProductTag",
//                column: "TagsId");
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "ProductTag");

//            migrationBuilder.DropTable(
//                name: "Products");

//            migrationBuilder.DropTable(
//                name: "Tag");

//            migrationBuilder.DropTable(
//                name: "Categories");
//        }
//    }
//}
