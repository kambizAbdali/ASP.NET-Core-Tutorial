using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMVC.Controllers;
using WebAppMVC.Models.Entites;
using WebAppMVC.Models.Services;

namespace WebAppMVC.Test
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfProducts()
        {
            // Arrange  
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1" },
                new Product { Id = 2, Name = "Product2" }
            };
            _mockProductService.Setup(service => service.GetAll()).Returns(products);

            // Act  
            var result = _controller.Index();

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Details_ExistingId_ReturnsViewResult_WithProduct()
        {
            // Arrange  
            var product = new Product { Id = 1, Name = "Product1" };
            _mockProductService.Setup(service => service.GetById(1)).Returns(product);

            // Act  
            var result = _controller.Details(1);

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.ViewData.Model);
            Assert.Equal("Product1", model.Name);
        }

        [Fact]
        public void Details_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange  
            _mockProductService.Setup(service => service.GetById(999)).Returns((Product)null);

            // Act  
            var result = _controller.Details(999);

            // Assert  
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_ValidProduct_ReturnsRedirectToActionResult()
        {
            // Arrange  
            var product = new Product { Id = 1, Name = "Product1" };
            _mockProductService.Setup(service => service.Add(product));

            // Act  
            var result = _controller.Create(product);

            // Assert  
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockProductService.Verify(service => service.Add(product), Times.Once);
        }

        [Fact]
        public void Create_InvalidProduct_ReturnsViewResult()
        {
            // Arrange  
            var product = new Product();
            _controller.ModelState.AddModelError("Name", "Required");

            // Act  
            var result = _controller.Create(product);

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.ViewData.Model);
            Assert.Equal(product, model);
        }

        [Fact]
        public void Edit_ExistingId_ReturnsViewResult_WithProduct()
        {
            // Arrange  
            var product = new Product { Id = 1, Name = "Product1" };
            _mockProductService.Setup(service => service.GetById(1)).Returns(product);

            // Act  
            var result = _controller.Edit(1);

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.ViewData.Model);
            Assert.Equal("Product1", model.Name);
        }

        [Fact]
        public void Edit_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange  
            _mockProductService.Setup(service => service.GetById(999)).Returns((Product)null);

            // Act  
            var result = _controller.Edit(999);

            // Assert  
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ValidProduct_ReturnsRedirectToActionResult()
        {
            // Arrange  
            var product = new Product { Id = 1, Name = "Updated Product" };
            _mockProductService.Setup(service => service.Update(product));

            // Act  
            var result = _controller.Edit(1, product);

            // Assert  
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockProductService.Verify(service => service.Update(product), Times.Once);
        }

        [Fact]
        public void Delete_ExistingId_ReturnsViewResult_WithProduct()
        {
            // Arrange  
            var product = new Product { Id = 1, Name = "Product1" };
            _mockProductService.Setup(service => service.GetById(1)).Returns(product);

            // Act  
            var result = _controller.Delete(1);

            // Assert  
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.ViewData.Model);
            Assert.Equal("Product1", model.Name);
        }

        [Fact]
        public void Delete_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange  
            _mockProductService.Setup(service => service.GetById(999)).Returns((Product)null);

            // Act  
            var result = _controller.Delete(999);

            // Assert  
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteConfirmed_ExistingId_ReturnsRedirectToActionResult()
        {
            // Arrange  
            int productId = 1;
            _mockProductService.Setup(service => service.Remove(productId));

            // Act  
            var result = _controller.DeleteConfirmed(productId);

            // Assert  
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockProductService.Verify(service => service.Remove(productId), Times.Once);
        }

        [Fact]
        public void Product_NameIsRequired()
        {
            // Arrange  
            var product = new Product
            {
                Id = 1
                // Name is not set, simulating the required validation scenario.  
            };

            // Act  
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(product);
            bool isValid = Validator.TryValidateObject(product, validationContext, validationResults, true);

            // Assert  
            Assert.False(isValid);
            Assert.Single(validationResults); // Ensure that there's exactly one validation error  
            Assert.Equal("The Name is required.", validationResults[0].ErrorMessage);
        }
    }
}