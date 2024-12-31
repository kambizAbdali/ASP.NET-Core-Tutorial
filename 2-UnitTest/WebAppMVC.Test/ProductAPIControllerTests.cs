using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using WebAppMVC.Controllers;
using WebAppMVC.Models.Entites;
using WebAppMVC.Models.Services;

namespace WebAppMVC.Test
{
    public class ProductAPIControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductApiController _controller;

        public ProductAPIControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductApiController(_mockProductService.Object);
        }

        List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1" },
                new Product { Id = 2, Name = "Product2" }
            };

        [Fact]
        public void Get_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange  
            _mockProductService.Setup(service => service.GetAll()).Returns(products);

            // Act  
            var result = _controller.Get();

            // Assert  
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsAssignableFrom<List<Product>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count);
        }

        [Theory]
        [InlineData(1, "Product1")]
        [InlineData(2, "Product2")]
        public void Get_ExistingId_ReturnsOkResult_WithProduct(int id, string expectedName)
        {
            // Arrange  
            var product = new Product { Id = id, Name = expectedName };
            _mockProductService.Setup(service => service.GetById(id)).Returns(product);

            // Act  
            var result = _controller.Get(id);

            // Assert  
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(expectedName, returnedProduct.Name);
        }

        [Theory]
        [InlineData(999)]
        [InlineData(0)]
        public void Get_NonExistingId_ReturnsNotFound(int id)
        {
            // Arrange  
            _mockProductService.Setup(service => service.GetById(id)).Returns((Product)null);

            // Act  
            var result = _controller.Get(id);

            // Assert  
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Post_ValidProduct_ReturnsOkResult()
        {
            // Arrange  
            var product = new Product { Id = 1, Name = "Product1" };
            _mockProductService.Setup(service => service.Add(product)).Returns(product);

            // Act  
            var result = _controller.Post(product);

            // Assert  
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal("Product1", returnedProduct.Name);
        }

        [Fact]
        public void Put_ExistingProduct_ReturnsNoContent()
        {
            // Arrange  
            var existingProduct = new Product { Id = 1, Name = "Product1" };
            var updatedProduct = new Product { Id = 1, Name = "Updated Product", Price = 100 };
            _mockProductService.Setup(service => service.GetById(1)).Returns(existingProduct);

            // Act  
            var result = _controller.Put(1, updatedProduct);

            // Assert  
            Assert.IsType<NoContentResult>(result);
            _mockProductService.Verify(service => service.Update(existingProduct), Times.Once);
            Assert.Equal("Updated Product", existingProduct.Name); // Verify the update  
        }

        [Fact]
        public void Put_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange  
            var updatedProduct = new Product { Id = 999, Name = "Non-existing Product" };
            _mockProductService.Setup(service => service.GetById(999)).Returns((Product)null);

            // Act  
            var result = _controller.Put(999, updatedProduct);

            // Assert  
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product not found.", notFoundResult.Value);
        }

        [Fact]
        public void Put_ProductIdMismatch_ReturnsBadRequest()
        {
            // Arrange  
            var product = new Product { Id = 1, Name = "Product1" };
            _mockProductService.Setup(service => service.GetById(1)).Returns(product);

            // Act  
            var result = _controller.Put(2, product); // Note the mismatch in ID  

            // Assert  
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product ID mismatch or invalid product.", badRequestResult.Value);
        }

        [Fact]
        public void Delete_ExistingProduct_ReturnsOkResult()
        {
            // Arrange  
            var productId = 1;
            _mockProductService.Setup(service => service.Remove(productId));

            // Act  
            var result = _controller.Delete(productId);

            // Assert  
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value); // Verify that result is true  
            _mockProductService.Verify(service => service.Remove(productId), Times.Once);
        }

        [Fact]
        public void Delete_NonExistingProduct_ReturnsOkResult()
        {
            // Arrange  
            var productId = 999; // Assuming product ID does not exist.  
            // Not setting up any return value, simulating a non-existing product scenario.  

            // Act  
            var result = _controller.Delete(productId);

            // Assert  
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value); // Check if the response is still a successful OK  
            _mockProductService.Verify(service => service.Remove(productId), Times.Once);
        }
    }
}