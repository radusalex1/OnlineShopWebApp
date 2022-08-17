using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private List<Order> _orders;
        private List<Client> _clients;
        private List<Gender> _genders;
        private List<Order>? orders;
        private Order order;
        private Order nullOrder;
        private OrdersController _ordersController;
        private Mock<IOrderRepository> _mockOrderRepository;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IStorageRepository> _mockStorageRepository;
        private Mock<IOrderedProductRepository> _mockOrdersProductRepository;


        [SetUp]
        public void Setup()
        {
            orders = null;
            nullOrder = null;
            _orders = new()
            {
                new Order()
                {
                    Id=1,
                    ClientId=1,
                    Created=DateTime.Now,
                    TotalAmount=0,
                    Canceled=false
                }
            };

            order = _orders[0];

            _clients = new()
            {
                new Client() {
                    Id = 1,
                    Name = "Radu",
                    Street = "Street",
                    City ="Brasov",
                    Country ="Romania",
                    PhoneNumber = "0725342567",
                    GenderId = 1
                }
            };

            _genders = new()
            {
                new Gender() { Id = 1, GenderType = "Male"},
                new Gender() { Id = 2, GenderType ="Female"}
            };

            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockClientRepository = new Mock<IClientRepository>();
            _mockStorageRepository = new Mock<IStorageRepository>();
            _mockOrdersProductRepository = new Mock<IOrderedProductRepository>();
            _ordersController = new OrdersController(_mockOrderRepository.Object,
                                                     _mockClientRepository.Object,
                                                     _mockOrdersProductRepository.Object,
                                                     _mockStorageRepository.Object);

        }

        [TearDown]
        public void TearDown()
        {
            _mockStorageRepository.Reset();
            _mockOrdersProductRepository.Reset();
            _mockOrderRepository.Reset();
            _mockClientRepository.Reset();
        }

        [Test]
        public async Task GetOrders_ShouldPass_WhenCallingIndexMethod()
        {
            //arrange
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orders));

            //act
            var actionResult = await _ordersController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());

        }

        [Test]
        public async Task GetOrder_ShoundPass_WhenCallingByInvalidId()
        {
            //arrange
            _mockOrderRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(nullOrder));

            //act
            var actionResult = await _ordersController.Details(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetOrder_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockOrderRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(order));

            //act
            var actionResult = await _ordersController.Details(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetOrder_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _ordersController.Details(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateOrder_ShouldPass_WhenCreatingValidOrder()
        {
            //arrage
            _mockOrderRepository.Setup(m => m.Add(It.IsAny<Order>())).Returns(Task.FromResult(true));
            _mockOrderRepository.Setup(n => n.IfExists(It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _ordersController.Create(order);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CreateOrder_ShouldPass_WhenCreatingInvalidOrder()
        {
            //arrange
            order.Client = null;

            order.ClientId = 0;

            _mockClientRepository.Setup(x => x.GetAll()).Returns(Task.FromResult(_clients));

            //act
            var actionResult = await _ordersController.Create(order);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task DeleteOrderPage_ShouldPass_WhenPassingNullId()
        {
            //act
            var actionResult = await _ordersController.Delete(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteOrderPage_ShouldPass_WhenPassingInvalidId()
        {
            //arrange
            _mockOrderRepository.Setup(x => x.Get(It.IsAny<int?>())).Returns(Task.FromResult(nullOrder));

            //act
            var actionResult = await _ordersController.Delete(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteOrderPage_ShouldPass_WhenPassingValidId()
        {
            //arrange
            _mockOrderRepository.Setup(x => x.Get(It.IsAny<int>())).Returns(Task.FromResult(order));

            //act
            var actionResult = await _ordersController.Delete(1);


            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteOrderPage_ShouldPass_WhenThereAreNoOrders()
        {
            //arrange
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(orders));

            //act
            var actionResult = await _ordersController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task DeleteOrderPage_ShouldPass_WhenCallingDeletePostMethod()
        {
            //arrange
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orders));

            //act
            var actionResult = await _ordersController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }
    }
}
