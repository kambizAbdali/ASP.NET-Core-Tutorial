using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inheritance.Migrations
{
    public partial class InitInheritanceTPH : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                });

            migrationBuilder.CreateTable(
                name: "BankTransferPayment",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTransferPayment", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_BankTransferPayment_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "PaymentId");
                });

            migrationBuilder.CreateTable(
                name: "CreditCardPayment",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cvv = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPayment", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "PaymentId");
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankTransferPayment");

            migrationBuilder.DropTable(
                name: "CreditCardPayment");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
