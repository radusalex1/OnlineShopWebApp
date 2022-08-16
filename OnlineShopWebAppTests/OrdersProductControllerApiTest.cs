using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers.APIControllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    internal class OrdersProductControllerApiTest
    {
        private Mock<IOrdersProductRepository> _mockOrdersProductRepository;
        private OrdersProductControllerApi _ordersProductControllerApi;

        [SetUp]
        public void SetUp()
        {
            _mockOrdersProductRepository = new Mock<IOrdersProductRepository>();
            _ordersProductControllerApi = new OrdersProductControllerApi(_mockOrdersProductRepository.Object);
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
            Assert.AreEqual("Invalid orderId!", (actionResult as BadRequestObjectResult).Value);
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
            Assert.AreEqual($"No products for orderId:{1}!", (actionResult as OkObjectResult).Value);

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
