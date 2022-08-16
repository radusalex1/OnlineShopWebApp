using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers.APIControllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    public class OrdersControllerApiTest
    {
        private Mock<IOrderRepository> _mockOrderRepository;
        private OrdersControllerApi _ordersControllerApi;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _ordersControllerApi = new OrdersControllerApi(_mockOrderRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockOrderRepository.Reset();
        }

        [Test]
        public async Task CancelOrderById_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _ordersControllerApi.CancelOrderById(0);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
            Assert.AreEqual($"Invalid OrderId!", (actionResult as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task CancelOrderById_ShoundPass_WhenCallingByUnexsitingId()
        {

            //arrange
            _mockOrderRepository.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _ordersControllerApi.CancelOrderById(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            Assert.AreEqual($"Order not found!", (actionResult as NotFoundObjectResult).Value);
        }

        [Test]
        public async Task CancelOrderById_ShouldPass_WhenCallingByValidId()
        {

            //arrange
            _mockOrderRepository.Setup(m => m.CancelOrderById(It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _ordersControllerApi.CancelOrderById(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual($"The order {1} was canceled successfully!", (actionResult as OkObjectResult).Value);
        }

        [Test]
        public async Task UnCancelOrderById_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _ordersControllerApi.UnCancelOrderById(0);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
            Assert.AreEqual($"Invalid OrderId!", (actionResult as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task UnCancelOrderById_ShoundPass_WhenCallingByUnexsitingId()
        {
            //arrange
            _mockOrderRepository.Setup(m => m.UnCancelOrderById(It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _ordersControllerApi.UnCancelOrderById(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundObjectResult>(actionResult);
            Assert.AreEqual($"Order not found!", (actionResult as NotFoundObjectResult).Value);
        }

        [Test]
        public async Task UnCancelOrderById_ShouldPass_WhenCallingByValidId()
        {

            //arrange
            _mockOrderRepository.Setup(m => m.UnCancelOrderById(It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _ordersControllerApi.UnCancelOrderById(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual($"The order {1} was canceled successfully!", (actionResult as OkObjectResult).Value);
        }

        [Test]
        public async Task GetClientOrders_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _ordersControllerApi.GetClientOrders(0);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
            Assert.AreEqual("Invalid clientId", (actionResult as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task GetClientOrders_ShouldPass_WhenClientHasNoOrders()
        {
            //arrange
            List<Order> clientOrders = new List<Order>();

            _mockOrderRepository.Setup(m => m.GetOrdersByClientId(It.IsAny<int>())).Returns(Task.FromResult(clientOrders));

            //act
            var actionResult = await _ordersControllerApi.GetClientOrders(1);
            
            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual($"No order found for clientId:{1}!", (actionResult as OkObjectResult).Value);
        }

        [Test]
        public async Task GetClientOrders_ShouldPass_WhenClientHasOrders()
        {
            //arrange
            List<Order> orders = new List<Order>()
            {
                new Order
                {
                    Id=1,
                    ClientId=1,
                    Canceled=false,
                }
            };
            _mockOrderRepository.Setup(x => x.GetOrdersByClientId(It.IsAny<int>())).Returns(Task.FromResult(orders));

            //act
            var actionResult = await _ordersControllerApi.GetClientOrders(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);

        }

        [Test]
        public async Task GetCLientNumberOfOrders_ShouldPass_WhenPassingByInvalidId()
        {
            //act
            var actionResult = await _ordersControllerApi.GetClientNumberOfOrders(0);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
            Assert.AreEqual("Invalid clientId", (actionResult as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task GetCLientNumberOfOrders_ShouldPass_WhenClientHasNoOrders()
        {
            //arrange
            List<Order> clientOrders = new List<Order>();

            _mockOrderRepository.Setup(m => m.GetOrdersByClientId(It.IsAny<int>())).Returns(Task.FromResult(clientOrders));

            //act
            var actionResult = await _ordersControllerApi.GetClientNumberOfOrders(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual($"No order found for clientId:{1}!", (actionResult as OkObjectResult).Value);
        }

        [Test]
        public async Task GetCLientNumberOfOrders_ShouldPass_WhenClientHasOrders()
        {
            //arrange
            List<Order> orders = new List<Order>()
            {
                new Order
                {
                    Id=1,
                    ClientId=1,
                    Canceled=false,
                }
            };

            _mockOrderRepository.Setup(x => x.GetOrdersByClientId(It.IsAny<int>())).Returns(Task.FromResult(orders));

            //act
            var actionResult = await _ordersControllerApi.GetClientNumberOfOrders(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual(1, (actionResult as OkObjectResult).Value);
        }

        [Test]
        public async Task CheckIfOrderIsCanceled_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _ordersControllerApi.CheckIfOrderIsCanceled(0);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
            Assert.AreEqual("Invalid orderId", (actionResult as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task CheckIfOrderIsCanceled_ShouldPass_WhenCallingByUnexsistingId()
        {

            //arrange
            Order order = null;

            _mockOrderRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(order));

            //act
            var actionResult = await _ordersControllerApi.CheckIfOrderIsCanceled(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual($"No order found with id:{1}!", (actionResult as OkObjectResult).Value);
        }

        [Test]
        public async Task CheckIfOrderIsCanceled_ShouldPass_WhenCallingByExistingId_AndOrderIsCanceled()
        {
            //arrange
            Order order = new()
            {
                Id = 1,
                Canceled = true,
                ClientId = 1,
                Created = DateTime.Now
            };

            _mockOrderRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(order));

            //act
            var actionResult = await _ordersControllerApi.CheckIfOrderIsCanceled(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual("The order is canceled", (actionResult as OkObjectResult).Value);
        }

        [Test]
        public async Task CheckIfOrderIsCanceled_ShouldPass_WhenCallingByExistingId_AndOrderIsNotCanceled()
        {
            //arrange
            Order order = new()
            {
                Id = 1,
                Canceled = false,
                ClientId = 1,
                Created = DateTime.Now
            };

            _mockOrderRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(order));

            //act
            var actionResult = await _ordersControllerApi.CheckIfOrderIsCanceled(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            Assert.AreEqual("The order is not canceled", (actionResult as OkObjectResult).Value);
        }
    }

}
