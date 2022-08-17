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
    public class ProductControllerTest
    {
        private List<Product> _products;
        private Product? _nullProduct;
        private List<Product>? _emptyListOfProducts;
        private ProductsController _productsController;
        private Mock<IProductRepository> _mockProductRepository;

        [SetUp]
        public void Setup()
        {
            _products = new()
            {
                new Product() {
                    Id = 1,
                    Name = "test",
                    ExpirationDate=DateTime.Now,
                    Price=200
                }
            };

            _nullProduct = null;
            _emptyListOfProducts = null;

            _mockProductRepository = new Mock<IProductRepository>();
            _productsController = new ProductsController(_mockProductRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockProductRepository.Reset();
        }

        [Test]
        public async Task IndexMethodTest_ShouldPass_WhenIndexMethodIsCalled()
        {
            //arrange
            _mockProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_products));

            //act
            var actionResult = await _productsController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task CreateProduct_ShouldPass_WhenCreatingValidProduct()
        {
            //act
            var result = await _productsController.Create(_products[0]);

            //assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CreateProduct_ShouldPass_WhenCreatingInvalidProduct()
        {
            //arrange
            _mockProductRepository.Setup(m => m.Add(It.IsAny<Product>())).Returns(Task.FromResult(false));
            _mockProductRepository.Setup(n => n.IfExists(It.IsAny<string>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _productsController.Create(_products[0]);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetProduct_ShouldPass_WhenCallingByInvalidId()
        {
            //arrange
            _mockProductRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(_nullProduct));

            //act
            var actionResult = await _productsController.Details(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetProduct_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockProductRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(_products[0]));

            //act
            var actionResult = await _productsController.Details(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetProduct_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _productsController.Details(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task EditProduct_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _productsController.Edit(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task EditProduct_ShouldPass_WhenCallingByInvalidId()
        {
            //arrange
            _mockProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_nullProduct));

            //act
            var actionResult = await _productsController.Edit(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditProduct_ShouldPass_WhenCallingByValidId()
        {         
            //arrange
            _mockProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_products[0]));


            //act
            var actionResult = await _productsController.Edit(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditProductPost_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _productsController.Edit(2, _products[0]);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditProductPost_ShouldPass_WhenCallingWithInvalidProduct()
        {
            //arrange
            _products[0].Id = 2;

            _mockProductRepository.Setup(m => m.Update(It.IsAny<Product>())).Throws<DbUpdateConcurrencyException>();
            _mockProductRepository.Setup(m => m.CanUpdate(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _productsController.Edit(2, _products[0]);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditProductPost_ShouldPass_WhenThrowException()
        {
            //arrange
            _mockProductRepository.Setup(m => m.Update(It.IsAny<Product>())).Throws<DbUpdateConcurrencyException>();
            _mockProductRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockProductRepository.Setup(m => m.CanUpdate(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //assert
            Assert.That(async () => await _productsController.Edit(1, _products[0]), Throws.TypeOf<DbUpdateConcurrencyException>());
        }

        [Test]
        public async Task EditProductPost_ShouldPass_WhenEditValidProduct()
        {
            //arrange
            _mockProductRepository.Setup(m => m.Update(It.IsAny<Product>())).Returns(Task.FromResult(true));
            _mockProductRepository.Setup(m => m.CanUpdate(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _productsController.Edit(1, _products[0]);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task DeleteProductPage_ShouldPass_WhenPassingNullId()
        {
            //act
            var actionResult = await _productsController.Delete(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteProductPage_ShouldPass_WhenPassingInvalidId()
        {
            //arrange
            _mockProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_nullProduct));

            //act
            var actionResult = await _productsController.Delete(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteProductPage_ShouldPass_WhenPassingValidId()
        {
            //arrange        
            _mockProductRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_products[0]));

            //act
            var actionResult = await _productsController.Delete(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteProductPage_ShouldPass_WhenThereAreNoProducts()
        {
            //arrange
            _mockProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_emptyListOfProducts));

            //act
            var actionResult = await _productsController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task DeleteProduct_ShouldPass_WhenDeletingValidProduct()
        {         
            //arrange
            _mockProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_products));

            //act
            var actionResult = await _productsController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task ProductExists_ShoudPass_WhenThereAreDuplicates()
        {
            //arrange
            _mockProductRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _productsController.ProductExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(true));
        }

        [Test]
        public async Task ProductExists_ShoudPass_WhenThereAreNoDuplicates()
        {
            //arrange
            _mockProductRepository.Setup(m => m.IfExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _productsController.ProductExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(false));
        }
    }
}
