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
        private OrdersController _ordersController;
        private Mock<IOrderRepository> _mockOrderRepository;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IStorageRepository> _mockStorageRepository;
        private Mock<IOrdersProductRepository> _mockOrdersProductRepository;


        [SetUp]
        public void Setup()
        {
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

            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockClientRepository = new Mock<IClientRepository>();
            _mockStorageRepository = new Mock<IStorageRepository>();
            _mockOrdersProductRepository = new Mock<IOrdersProductRepository>();
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
        
    }
}
