using FakeItEasy;
using FluentAssertions;
using MaldsShopWebApp.Controllers;
using MaldsShopWebApp.Helpers;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MaldsShopWebApp.Tests.Controllers
{
    public class ShippingCartControllerTests
    {
        private readonly ShippingCartController _controller;
        private readonly IUserRepository _userRepository = A.Fake<IUserRepository>();
        private readonly IShippingCartRepository _shippingCartRepository = A.Fake<IShippingCartRepository>();
        private readonly IProductRepository _productRepository = A.Fake<IProductRepository>();
        private readonly CartCountSession _cartCountSession = A.Fake<CartCountSession>();
        private readonly DefaultHttpContext _httpContext = new DefaultHttpContext();

        public ShippingCartControllerTests()
        {
            var session = A.Fake<ISession>();
            var sessionItems = new Dictionary<string, byte[]>();

            A.CallTo(session)
                .Where(call => call.Method.Name == "Set")
                .Invokes((call) =>
                {
                    var key = call.GetArgument<string>(0);
                    var value = call.GetArgument<byte[]>(1);
                    sessionItems[key] = value;
                });

            var httpContext = new DefaultHttpContext { Session = session };
            _controller = new ShippingCartController(_userRepository, _shippingCartRepository, _productRepository, _cartCountSession)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }
        [Fact]
        public async Task AddToCart_ValidProduct_RedirectsToCartIndex()
        {
            // Arrange
            var fakeUser = new AppUser { Email = "user@example.com", Id = "userId", ShippingCart = new ShippingCart() };
            var fakeProduct = new Product { ProductId = 1 };
            A.CallTo(() => _userRepository.GetByEmailAsync("user@example.com")).Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _productRepository.GetByIdAsync(1)).Returns(Task.FromResult(fakeProduct));

            // Act
            var result = await _controller.AddToCart(1, false);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = (RedirectToActionResult)result;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("ShippingCart");
        }
        [Fact]
        public async Task RemoveFromCart_ValidProduct_RedirectsToCartIndex()
        {
            // Arrange
            var productId = 1;
            var fakeUser = new AppUser { Email = "user@example.com", Id = "userId", ShippingCart = new ShippingCart() };
            var fakeProduct = new Product { ProductId = productId };
            var fakeCartItem = new ShippingCartItem { ProductId = productId, Quantity = 1, ShippingCartId = fakeUser.ShippingCart.ShippingCartId };
            fakeUser.ShippingCart.ShippingCartItems.Add(fakeCartItem);

            A.CallTo(() => _userRepository.GetByEmailAsync("user@example.com")).Returns(Task.FromResult(fakeUser));
            A.CallTo(() => _productRepository.GetByIdAsync(productId)).Returns(Task.FromResult(fakeProduct));
            A.CallTo(() => _shippingCartRepository.GetShippingCartByUserEmail("user@example.com")).Returns(Task.FromResult(fakeUser.ShippingCart));

            // Act
            var result = await _controller.RemoveFromCart(productId, false);

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
    }
}
