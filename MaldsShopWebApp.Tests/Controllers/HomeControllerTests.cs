using FakeItEasy;
using FluentAssertions;
using MaldsShopWebApp.Controllers;
using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Models;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MaldsShopWebApp.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IShippingCartRepository _shippingCartRepository;
        private readonly IUserRepository _userRepository;
        private readonly CartCountSession _cartCountSession;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _logger = A.Fake<ILogger<HomeController>>();
            _productRepository = A.Fake<IProductRepository>();
            _shippingCartRepository = A.Fake<IShippingCartRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _cartCountSession = A.Fake<CartCountSession>();

            _controller = new HomeController(_logger, _productRepository, _shippingCartRepository, _userRepository, _cartCountSession);
        }

        [Fact]
        public async Task Index_WhenCalled_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var fakePaginatedResult = new PaginatedResult<Product>
            {
                Items = A.CollectionOfFake<Product>(10),
                TotalCount = 100
            };
            A.CallTo(() => _productRepository.GetAllPaginatedAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored))
                .Returns(Task.FromResult(fakePaginatedResult));

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            viewResult.Model.Should().BeOfType<IndexProductViewModel>();
            var viewModel = viewResult.Model as IndexProductViewModel;
            viewModel.Products.Should().BeEquivalentTo(fakePaginatedResult.Items);
            viewModel.TotalPages.Should().Be((int)Math.Ceiling(fakePaginatedResult.TotalCount / 12.0)); // Assuming default pageSize = 12
        }
        [Fact]
        public void Privacy_ReturnsView()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public void Error_ReturnsErrorViewWithCorrectModel()
        {
            // Arrange
            var expectedRequestId = "1234";
            var controllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            controllerContext.HttpContext.Items["TraceIdentifier"] = expectedRequestId;
            _controller.ControllerContext = controllerContext;
            _controller.ControllerContext.HttpContext.TraceIdentifier = expectedRequestId;


            // Act
            var result = _controller.Error();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as ErrorViewModel;

            model.Should().NotBeNull();
            model.RequestId.Should().Be(expectedRequestId); // Adjust according to your actual logic
        }
        [Fact]
        public async Task Index_AuthenticatedUser_ReturnsViewWithUserCart()
        {
            // Arrange
            var session = A.Fake<ISession>();
            var fakeContext = new DefaultHttpContext { Session = session };
            _controller.ControllerContext = new ControllerContext { HttpContext = fakeContext };

            var user = A.Fake<AppUser>();
            var cart = A.Fake<ShippingCart>();
            user.ShippingCart = cart;
            var products = A.CollectionOfFake<Product>(10);
            var paginatedResult = new PaginatedResult<Product>
            {
                Items = products,
                TotalCount = 100
            };

            A.CallTo(() => _productRepository.GetAllPaginatedAsync(A<int>.Ignored, A<int>.Ignored, A<string>.Ignored))
                .Returns(Task.FromResult(paginatedResult));
            A.CallTo(() => _userRepository.GetByEmail("test@example.com")).Returns(Task.FromResult(user));

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test@example.com"), };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext.User = claimsPrincipal;

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<IndexProductViewModel>(viewResult.Model);

            Assert.NotNull(model);
            Assert.Equal(cart, model.ShippingCart);
            Assert.Equal(products, model.Products);
            Assert.Equal(9, model.TotalPages);
        }
        [Fact]
        public async Task Index_DifferentPageAndPageSize_ReturnsCorrectPagination()
        {
            // Arrange
            var page = 2;
            var pageSize = 5;
            var sortBy = "Price";

            var fakePaginatedResult = new PaginatedResult<Product>
            {
                Items = A.CollectionOfFake<Product>(pageSize),
                TotalCount = 50
            };
            A.CallTo(() => _productRepository.GetAllPaginatedAsync(page, pageSize, sortBy))
                .Returns(Task.FromResult(fakePaginatedResult));

            // Act
            var result = await _controller.Index(page, pageSize, sortBy);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var viewModel = viewResult.Model as IndexProductViewModel;
            viewModel.CurrentPage.Should().Be(page);
            viewModel.TotalPages.Should().Be((int)Math.Ceiling(fakePaginatedResult.TotalCount / (double)pageSize));
            viewModel.SortBy.Should().Be(sortBy);
            viewModel.Products.Count().Should().Be(pageSize);
        }
    }
}
