using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers.APIControllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests.APITests
{
    [TestFixture]
    internal class OrderedProductControllerApiTest
    {
        private Mock<IOrderedProductRepository> _mockOrdersProductRepository;
        private OrderedProductControllerApi _ordersProductControllerApi;

        [SetUp]
        public void SetUp()
        {
            _mockOrdersProductRepository = new Mock<IOrderedProductRepository>();
            _ordersProductControllerApi = new OrderedProductControllerApi(_mockOrdersProductRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockOrdersProductRepository.Reset();
        }

        [Test]
        public async Task GetProductsByOrder_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _ordersProductControllerApi.GetProductsByOrder(0);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
            Assert.That((actionResult as BadRequestObjectResult).Value, Is.EqualTo("Invalid orderId!"));
        }

        [Test]
        public async Task GetProductsByOrder_ShouldPass_WhenOrderHasNoProducts()
        {
            //arrange
            List<Product> products = new List<Product>();

            _mockOrdersProductRepository.Setup(x => x.GetProductsFromOrder(It.IsAny<int>())).Returns(Task.FromResult(products));

            //act
            var actionResult = await _ordersProductControllerApi.GetProductsByOrder(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.That((actionResult as OkObjectResult).Value, Is.EqualTo($"No products for orderId:{1}!"));
        }

        [Test]
        public async Task GetProductsByOrder_ShouldPass_WhenOrderHasProducts()
        {
            //arrange
            List<Product> products = new()
            {
                new Product()
                {
                    Id=1,
                    Price=200,
                    ExpirationDate=DateTime.Today,
                    Name="Test"
                }
            };

            _mockOrdersProductRepository.Setup(x => x.GetProductsFromOrder(It.IsAny<int>())).Returns(Task.FromResult(products));

            //act
            var actionResult = await _ordersProductControllerApi.GetProductsByOrder(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
        }
    }
}
