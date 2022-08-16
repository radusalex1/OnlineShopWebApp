using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    public class OrderedProductControllerTests
    {
        private OrderedProduct _orderedProduct;
        private List<Order> _orders;
        private List<Product> _products;
        private List<OrderedProduct> _orderedProducts;
        private OrderedProductController _orderedProductController;
        private Mock<IStorageRepository> _mockStorageRepository;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<IOrderRepository> _mockOrderRepository;
        private Mock<IOrderedProductRepository> _mockOrderedProductRepository;


        [SetUp]
        public void Setup()
        {
            _orders = new()
            {
                new Order() { Id = 1, ClientId = 1, Canceled = false, Created = new DateTime(2022, 8, 15), TotalAmount = 100 },
                new Order() { Id = 2, ClientId = 2, Canceled = true, Created = new DateTime(2022, 8, 13), TotalAmount = 20 }
            };

            _products = new()
            {
                new Product() { Id = 1, Name = "Meat", Price = 50, ExpirationDate= new DateTime(2022, 8, 30)},
                new Product() { Id = 2, Name = "Bread", Price = 10, ExpirationDate= new DateTime(2022, 8, 20)}
            };

            _orderedProducts = new()
            {
                new OrderedProduct() { Id = 1, OrderId = 1, ProductId = 1, Quantity = 2},
                new OrderedProduct() { Id = 2, OrderId = 2, ProductId = 2, Quantity = 2},
            };

            _orderedProduct = _orderedProducts[0];

            _mockStorageRepository = new Mock<IStorageRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockOrderedProductRepository = new Mock<IOrderedProductRepository>();

            _orderedProductController = new OrderedProductController(_mockOrderedProductRepository.Object, _mockOrderRepository.Object,
                                                                   _mockProductRepository.Object, _mockStorageRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockStorageRepository.Reset();
            _mockProductRepository.Reset();
            _mockOrderRepository.Reset();
            _mockOrderedProductRepository.Reset();
        }


        [Test]
        public async Task GetOrderedProductShould()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orderedProducts));

            //act
            var actionResult = await _orderedProductController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetOrderedProduct_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            OrderedProduct? orderedProduct = null;

            _mockOrderedProductRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(orderedProduct));

            //act
            var actionResult = await _orderedProductController.Details(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetOrderedProduct_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(_orderedProduct));

            //act
            var actionResult = await _orderedProductController.Details(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetOrderedProduct_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _orderedProductController.Details(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateOrderedProduct_ShouldPass_WhenCreatingValidOrderedProduct()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.Add(It.IsAny<OrderedProduct>())).Returns(Task.FromResult(true));
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _orderedProductController.Create(_orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CreateOrderedProduct_ShouldPass_WhenCreatingInvalidOrderedProduct()
        {
            //arrange
            _orderedProduct.Quantity = -2;

            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(false));
            _mockProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_products));
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orders));

            //act
            var actionResult = await _orderedProductController.Create(_orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task CreateOrderedProduct_ShouldPass_WhenOrderedProductAlreadyExists()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_products));
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orders));

            //act
            var actionResult = await _orderedProductController.Create(_orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditOrderedProductPage_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _orderedProductController.Edit(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task EditOrderedProductPage_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            OrderedProduct? orderedProduct = null;

            _mockOrderedProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(orderedProduct));

            //act
            var actionResult = await _orderedProductController.Edit(3);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditOrderedProductPage_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_orderedProduct));
            _mockProductRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_products));
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orders));

            //act
            var actionResult = await _orderedProductController.Edit(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditOrderedProductPost_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _orderedProductController.Edit(3, _orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditOrderedProductPost_ShouldPass_WhenObjectDoesntExist()
        {
            //arrange
            _orderedProduct.Id = 3;

            _mockOrderedProductRepository.Setup(m => m.Update(It.IsAny<OrderedProduct>())).Throws<DbUpdateConcurrencyException>();
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                         .Returns(Task.FromResult(false));
            _mockOrderedProductRepository.Setup(m => m.GetQuantityForProductFromOrder(It.IsAny<int>(), It.IsAny<int>()))
                                         .Returns(Task.FromResult(3));
            _mockStorageRepository.Setup(m => m.IncreaseQuantity(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _orderedProductController.Edit(3, _orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void EditOrderedProductPost_ShouldPass_WhenThrowException()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.Update(It.IsAny<OrderedProduct>())).Throws<DbUpdateConcurrencyException>();
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                         .Returns(Task.FromResult(false));
            _mockOrderedProductRepository.Setup(m => m.GetQuantityForProductFromOrder(It.IsAny<int>(), It.IsAny<int>()))
                                         .Returns(Task.FromResult(3));
            _mockStorageRepository.Setup(m => m.IncreaseQuantity(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //assert
            Assert.That(async () => await _orderedProductController.Edit(1, _orderedProduct), Throws.TypeOf<DbUpdateConcurrencyException>());
        }

        [Test]
        public async Task EditOrderedProductPost_ShouldPass_WhenEditValidOrderedProduct()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.Update(It.IsAny<OrderedProduct>())).Returns(Task.FromResult(true));
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                         .Returns(Task.FromResult(false));
            _mockOrderedProductRepository.Setup(m => m.GetQuantityForProductFromOrder(It.IsAny<int>(), It.IsAny<int>()))
                                         .Returns(Task.FromResult(3));
            _mockStorageRepository.Setup(m => m.IncreaseQuantity(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _orderedProductController.Edit(1, _orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task EditOrderedProductPost_ShouldPass_WhenThereAreDuplicates()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                                         .Returns(Task.FromResult(true));
            _mockProductRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_products));
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orders));

            //act
            var actionResult = await _orderedProductController.Edit(1, _orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditOrderedProductPost_ShouldPass_WhenCallingWithNegativeQuantity()
        {
            //arrange
            _orderedProduct.Quantity = -1;

            _mockProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_products));
            _mockOrderRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orders));

            //act
            var actionResult = await _orderedProductController.Edit(1, _orderedProduct);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteOrderedProductPage_ShouldPass_WhenPassingNullId()
        {
            //act
            var actionResult = await _orderedProductController.Delete(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteOrderedProductPage_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            OrderedProduct? orderedProduct = null;

            _mockOrderedProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(orderedProduct));

            //act
            var actionResult = await _orderedProductController.Delete(3);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteOrderedProductPage_ShouldPass_WhenPassingValidId()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_orderedProduct));

            //act
            var actionResult = await _orderedProductController.Delete(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteOrderedProduct_ShouldPass_WhenContextIsEmpty()
        {
            //arrange
            List<OrderedProduct> orderedProducts = null;

            _mockOrderedProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(orderedProducts));

            //act
            var actionResult = await _orderedProductController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task DeleteOrderedProduct_ShouldPass_WhenIdIsValid()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_orderedProducts));

            //act
            var actionResult = await _orderedProductController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task OrderedProductExists_ShoudPass_WhenOrderedProductExists()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _orderedProductController.OrderedProductExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(true));
        }

        [Test]
        public async Task OrderedProductExists_ShoudPass_WhenOrderedProductDoesntExist()
        {
            //arrange
            _mockOrderedProductRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _orderedProductController.OrderedProductExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(false));
        }
    }
}
