using CloudinaryDotNet.Actions;
using FakeItEasy;
using FluentAssertions;
using MaldsShopWebApp.Controllers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Repository;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldsShopWebApp.Tests.Controllers
{
    public class ProductControllerTests
    {
        private IProductRepository _productRepository;
        private IUserRepository _userRepository;
        private IPhotoService _photoService;
        private ProductController _controller;
        public ProductControllerTests()
        {
            _productRepository = A.Fake<IProductRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _photoService = A.Fake<IPhotoService>();

            _controller = new ProductController(_productRepository, _userRepository, _photoService);
        }
        [Fact]
        public async void ProductController_IndexGet_ReturnSuccess()
        {
            //Arrange
            var products = A.Fake<IEnumerable<Product>>();
            IndexProductViewModel productsVM = A.Fake<IndexProductViewModel>();
            A.CallTo(() => _productRepository.GetAllAsync()).Returns(products);

            //Act
            var result = await _controller.Index();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<IndexProductViewModel>();
            var model = viewResult.Model as IndexProductViewModel;
            model.Products.Should().BeEquivalentTo(products);
        }
        [Fact]
        public async void ProductController_CreateGet_ReturnSuccess()
        {
            //Arrange


            //Act
            var result = _controller.Create();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async void ProductController_CreatePost_ReturnSuccess()
        {
            //Arrange
            var photoResult = A.Fake<ImageUploadResult>();
            var productVM = A.Fake<CreateProductViewModel>();
            A.CallTo(() => _photoService.AddPhotoAsync(productVM.ImageUrl)).Returns(photoResult);
            var product = A.Fake<Product>();
            A.CallTo(() => _productRepository.AddAsync(product));

            //Act
            var result = _controller.Create(productVM);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
        }
        [Fact]
        public async void ProductController_CreatePost_ReturnsViewWithModel()
        {
            // Arrange
            var productVM = A.Dummy<CreateProductViewModel>();
            _controller.ModelState.AddModelError("Test", "Test Error");

            // Act
            var result = await _controller.Create(productVM);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeSameAs(productVM);
        }
        [Fact]
        public async void ProductController_CreatePost_AddsProductAndRedirects()
        {
            // Arrange
            var productVM = A.Fake<CreateProductViewModel>();
            var mockImage = A.Fake<IFormFile>();
            productVM.ImageUrl = mockImage;
            var photoResult = new ImageUploadResult { Url = new System.Uri("http://example.com/image.jpg") };
            A.CallTo(() => _photoService.AddPhotoAsync(A<IFormFile>.Ignored)).Returns(photoResult);

            // Act
            var result = await _controller.Create(productVM);

            // Assert
            A.CallTo(() => _productRepository.AddAsync(A<Product>.Ignored)).MustHaveHappenedOnceExactly();
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result as RedirectToActionResult;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Home");
        }
        [Fact]
        public async void ProductController_DetailsGet_ReturnSuccess()
        {
            //Arrange
            var id = 1;
            var product = A.Fake<Product>();
            A.CallTo(() => _productRepository.GetByIdAsync(id)).Returns(product);
            var detailsVM = A.Fake<DetailsProductViewModel>();

            //Act
            var result = _controller.Details(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
        }
        [Fact]
        public async void ProductController_EditGet_ReturnSuccess()
        {
            //Arrange
            var id = 1;
            var product = A.Fake<Product>();
            A.CallTo(() => _productRepository.GetByIdAsync(id)).Returns(product);
            var editVM = A.Fake<EditProductViewModel>();

            //Act
            var result = _controller.Edit(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
        }
        public async void ProductController_EditGet_ReturnsViewWithProduct()
        {
            // Arrange
            var id = 1;
            var product = A.Dummy<Product>();
            A.CallTo(() => _productRepository.GetByIdAsync(id)).Returns(product);

            // Act
            var result = await _controller.Edit(id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<EditProductViewModel>();
        }
        [Fact]
        public async void ProductController_EditPost_ReturnSuccess()
        {
            //Arrange
            var id = 1;
            var editVM = A.Fake<EditProductViewModel>();
            var oldProduct = A.Fake<Product>();
            A.CallTo(() => _productRepository.GetByIdAsync(id)).Returns(oldProduct);
            var photoResult = A.Fake<ImageUploadResult>();
            A.CallTo(() => _photoService.AddPhotoAsync(editVM.Image)).Returns(photoResult);
            A.CallTo(() => _photoService.DeletePhotoAsync(oldProduct.ImageUrl));

            //Act
            var result = _controller.Edit(id, editVM);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
        }
        [Fact]
        public async void ProductController_EditPost_InvalidModel()
        {
            // Arrange
            var editVM = A.Dummy<EditProductViewModel>();
            _controller.ModelState.AddModelError("Test", "Test Error");

            // Act
            var result = await _controller.Edit(editVM.ProductId, editVM);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeSameAs(editVM);
        }
        [Fact]
        public async void ProductController_EditPost_ValidModel()
        {
            // Arrange
            var id = 1;
            var editVM = A.Fake<EditProductViewModel>();
            var oldProduct = A.Dummy<Product>();
            A.CallTo(() => _productRepository.GetByIdAsync(id)).Returns(oldProduct);
            var photoResult = new ImageUploadResult();
            A.CallTo(() => _photoService.AddPhotoAsync(editVM.Image)).Returns(photoResult);

            // Act
            var result = await _controller.Edit(id, editVM);

            // Assert
            A.CallTo(() => _productRepository.UpdateAsync(A<Product>.Ignored)).MustHaveHappenedOnceExactly();
            result.Should().BeOfType<RedirectToActionResult>();
        }
        [Fact]
        public async void ProductController_DeleteProduct_ReturnSuccess()
        {
            //Arrange
            var id = 1;
            var productDetails = A.Fake<Product>();
            var deletionResult = A.Fake<Task<DeletionResult>>();
            A.CallTo(() => _productRepository.GetByIdAsync(id)).Returns(productDetails);
            A.CallTo(() => _photoService.DeletePhotoAsync(productDetails.ImageUrl)).Returns(deletionResult);

            //Act
            var result = _controller.DeleteProduct(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<IActionResult>>();
        }
        [Fact]
        public async void ProductController_DeletesProductAndRedirects()
        {
            // Arrange
            var id = 1;
            var productDetails = A.Dummy<Product>();
            A.CallTo(() => _productRepository.GetByIdAsync(id)).Returns(productDetails);

            // Act
            var result = await _controller.DeleteProduct(id);

            // Assert
            A.CallTo(() => _productRepository.DeleteAsync(A<Product>.Ignored)).MustHaveHappenedOnceExactly();
            result.Should().BeOfType<RedirectToActionResult>();
        }
    }
}