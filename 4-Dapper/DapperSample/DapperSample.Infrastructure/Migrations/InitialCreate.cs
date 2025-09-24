using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperSample.Infrastructure.Migrations
{
    [Migration(202301010000)]
    public class InitialCreate : Migration
    {
        public override void Up()
        {
            Create.Table("Categories")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(100).NotNullable();

            Create.Table("Products")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Name").AsString(100).NotNullable()
                .WithColumn("Price").AsDecimal().NotNullable()
                .WithColumn("CategoryId").AsInt32().NotNullable().ForeignKey("Categories", "Id");

            Create.Table("Customers")
               .WithColumn("Id").AsInt32().PrimaryKey().Identity()
               .WithColumn("Name").AsString(100).NotNullable()
               .WithColumn("Email").AsString(100).NotNullable();

            Create.Table("Addresses")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Street").AsString(255).Nullable()
                .WithColumn("City").AsString(100).Nullable()
                .WithColumn("State").AsString(50).Nullable()
                .WithColumn("ZipCode").AsString(20).Nullable()
                .WithColumn("CustomerId").AsInt32().NotNullable().ForeignKey("Customers", "Id");

            Create.Table("Orders")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("OrderDate").AsDateTime().NotNullable()
                .WithColumn("CustomerId").AsInt32().NotNullable().ForeignKey("Customers", "Id");

            Create.Table("OrderDetails")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("OrderId").AsInt32().NotNullable().ForeignKey("Orders", "Id")
                .WithColumn("ProductId").AsInt32().NotNullable().ForeignKey("Products", "Id")
                .WithColumn("Quantity").AsInt32().NotNullable()
                .WithColumn("Price").AsDecimal().NotNullable();

            Create.Table("Tags")
               .WithColumn("Id").AsInt32().PrimaryKey().Identity()
               .WithColumn("Name").AsString(50).NotNullable();

            Create.Table("ProductTag")
         .WithColumn("ProductId").AsInt32().NotNullable().ForeignKey("Products", "Id")
         .WithColumn("TagId").AsInt32().NotNullable().ForeignKey("Tags", "Id")
         .PrimaryKey((new string[] { "ProductId", "TagId" }).ToString()); // کلید اصلی مرکب



            // Stored Procedure Sample
            Execute.Sql(@"
                CREATE PROCEDURE sp_UpdateProductPrice
                @ProductId INT
                AS
                BEGIN
                    UPDATE Products
                    SET Price = Price * 1.10
                    WHERE Id = @ProductId
                END
            ");

            // Insert Data
            Insert.IntoTable("Categories").Row(new { Name = "Electronics" });
            Insert.IntoTable("Categories").Row(new { Name = "Books" });
            Insert.IntoTable("Products").Row(new { Name = "Laptop", Price = 1200, CategoryId = 1 });
            Insert.IntoTable("Products").Row(new { Name = "The Lord of the Rings", Price = 30, CategoryId = 2 });
        }

        public override void Down()
        {
            Delete.Table("ProductTag");
            Delete.Table("Tags");
            Delete.Table("OrderDetails");
            Delete.Table("Orders");
            Delete.Table("Addresses");
            Delete.Table("Customers");
            Delete.Table("Products");
            Delete.Table("Categories");

            Execute.Sql("DROP PROCEDURE sp_UpdateProductPrice");


        }
    }
}