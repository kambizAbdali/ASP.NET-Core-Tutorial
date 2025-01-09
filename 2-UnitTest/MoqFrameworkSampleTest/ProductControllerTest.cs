using Moq;
using System.Collections;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MoqFrameworkSample.Services;
using MoqFrameworkSample.Models;
using MoqFrameworkSample;

namespace MoqFrameworkSampleTest
{
    public class ProductControllerTest
    {
        private Mock<IProductService> mockService;
        private ProductController controller;

        private List<Product> initialProducts;
        private int productCount;

        public ProductControllerTest()
        {
            mockService = new Mock<IProductService>();
            controller = new ProductController(mockService.Object);

            initialProducts = new List<Product>(); // لیست محصولات اولیه  
            productCount = 0; // شمارش محصولات اولیه  

            // تنظیم ویژگی CountProduct در Mock  
            mockService.SetupProperty(service => service.CountProduct, productCount);

            // تنظیم رفتار GetAllProducts  
            mockService.Setup(service => service.GetAllProducts()).Returns(initialProducts);

            // تنظیم رفتار AddProduct  
            mockService.Setup(service => service.AddProduct(It.IsAny<Product>())).Callback<Product>(product =>
            {
                initialProducts.Add(product);
                productCount++; // بروز رسانی شمارش  
                mockService.SetupProperty(service => service.CountProduct, productCount); // به روز رسانی مقدار  
            });
        }
        //MockBehavior.Loose allows for flexible mock setups where unconfigured members return default values
        [Fact]
        public void Index_ShouldReturnAllProducts_WhenCalled_WithLooseMock()
        {
            // Arrange  
            var mockService = new Mock<IProductService>(MockBehavior.Loose);
            mockService.Setup(service => service.GetAllProducts())
                       .Returns(new List<Product>{
                       new Product { Id = 1, Name = "Product1", Price = 10.00m, Description = "A sample product" }
                       });

            // Use mockService.Object instead of mockService  
            var controller = new ProductController(mockService.Object);

            // Act  
            var result = controller.Index() as ViewResult;

            // Assert  
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(result.ViewData.Model);
            Assert.Single(products);
        }

        //MockBehavior.Strict enforces strict setups requiring all interactions to be explicitly defined, throwing exceptions for unexpected calls.
        [Fact]
        public void Index_ShouldReturnAllProducts_WhenCalled_WithStrictMock()
        {
            // Arrange  
            var mockService = new Mock<IProductService>(MockBehavior.Strict);
            mockService.Setup(service => service.GetAllProducts())
                       .Returns(new List<Product>{
                       new Product { Id = 1, Name = "Product1", Price = 10.00m, Description = "A sample product" }
                       });

            // Use mockService.Object  
            var controller = new ProductController(mockService.Object);

            // Act  
            var result = controller.Index() as ViewResult;

            // Assert  
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(result.ViewData.Model);
            Assert.Single(products);

            // Verify that the method was called  
            mockService.Verify(service => service.GetAllProducts(), Times.Once());
        }

        [Fact]
        public void Details_ShouldReturnNotFound_WhenProductIsNull_WithStrictMock()
        {
            // Arrange  
            var mockService = new Mock<IProductService>(MockBehavior.Strict);
            mockService.Setup(service => service.GetProductById(It.IsAny<int>()))
                       .Returns((Product)null); // Return null for any ID  

            // Use mockService.Object  
            var controller = new ProductController(mockService.Object);

            // Act  
            var result = controller.Details(1);

            // Assert  
            Assert.IsType<NotFoundResult>(result);

            // Verify that the method was called  
            mockService.Verify(service => service.GetProductById(1), Times.Once());
        }

        [Fact]
        public void AddProduct_ShouldIncreaseCountProduct_WhenNewProductIsAdded()
        {
            // Arrange  
            var newProduct = new Product { Id = 1, Name = "Product1", Price = 10.00m, Description = "A sample product", InStock = true };

            // Act  
            controller.AddProduct(newProduct); // فرض، اینکه اکشن AddProduct در Controller داریم  

            // Assert  
            var count = mockService.Object.CountProduct; // دسترسی به شمارش محصولات  
            Assert.Equal(1, count); // بررسی اینکه تعداد به درستی افزایش یافته است  
        }

        // New unit test using It.IsAny and It.IsInRange
        [Fact]
        public void AddProduct_ShouldCallAddProduct_WithValidProduct_WhenCalled()
        {
            // Arrange
            var newProduct = new Product { Id = 1, Name = "Product1", Price = 15.00m, Description = "A valid product", InStock = true };

            // Act
            controller.AddProduct(newProduct);

            // Assert
            mockService.Verify(service => service.AddProduct(It.Is<Product>(p =>
                p.Name == "Product1" &&
                p.Price > 0 &&
                p.Price < 100)), Times.Once());
        }
    }
}