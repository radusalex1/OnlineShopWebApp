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
    public class StorageControllerTests
    {
        private Storage _storage;
        private List<Storage> _storages;
        private List<Product> _product;
        private StoragesController _storageController;
        private Mock<IStorageRepository> _mockStorageRepository;
        private Mock<IProductRepository> _mockProductRepository;

        [SetUp]
        public void Setup()
        {
            _storages = new()
            {
                new Storage() {
                    Id = 1,
                    ProductId = 1,
                    Quantity =30
                }
            };

            _product = new()
            {
                new Product() { Id = 1, Name = "Meat", Price =50, ExpirationDate= new DateTime(2022, 8, 30)},
                new Product() { Id = 2, Name = "Bread", Price =10, ExpirationDate= new DateTime(2022, 8, 20)}
            };

            _storage = _storages[0];

            _mockStorageRepository = new Mock<IStorageRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _storageController = new StoragesController(_mockStorageRepository.Object, _mockProductRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockStorageRepository.Reset();
            _mockProductRepository.Reset();
        }


        [Test]
        public async Task GetStorageShould()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_storages));

            //act
            var actionResult = await _storageController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetStorage_ShouldPass_WhenObjectNotFound()
        {
            Storage? storage = null;

            //arrange
            _mockStorageRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(storage));

            //act
            var actionResult = await _storageController.Details(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetStorage_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(_storage));

            //act
            var actionResult = await _storageController.Details(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetStorage_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _storageController.Details(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateStorage_ShouldPass_WhenCreatingValidStorage()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.Add(It.IsAny<Storage>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _storageController.Create(_storage);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CreateStorage_ShouldPass_WhenCreatingInvalidStorage()
        {
            //arrange
            _storage.Quantity = -2;

            _mockProductRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_product));

            //act
            var actionResult = await _storageController.Create(_storage);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditStoragePage_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _storageController.Edit(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task EditStoragePage_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            Storage? storage = null;     

            _mockStorageRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(storage));

            //act
            var actionResult = await _storageController.Edit(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditStoragePage_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_storage));
            _mockProductRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_product));

            //act
            var actionResult = await _storageController.Edit(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditStoragePost_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _storageController.Edit(2, _storage);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditStoragePost_ShouldPass_WhenStorageDoesntExist()
        {
            //arrange
            _storage.Id = 2;

            _mockStorageRepository.Setup(m => m.Update(It.IsAny<Storage>())).Throws<DbUpdateConcurrencyException>();
            _mockStorageRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _storageController.Edit(2, _storage);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void EditStoragePost_ShouldPass_WhenThrowException()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.Update(It.IsAny<Storage>())).Throws<DbUpdateConcurrencyException>();
            _mockStorageRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //assert
            Assert.That(async () => await _storageController.Edit(1, _storage), Throws.TypeOf<DbUpdateConcurrencyException>());
        }

        [Test]
        public async Task EditStoragePost_ShouldPass_WhenEditValidStorage()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.Update(It.IsAny<Storage>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _storageController.Edit(1, _storage);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task EditStoragePost_ShouldPass_WhenCallingWithNegativeQuantity()
        {
            //arrange
            _storage.Quantity = -1;

            _mockProductRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_product));

            //act
            var actionResult = await _storageController.Edit(1, _storage);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteStoragePage_ShouldPass_WhenPassingNullId()
        {
            //act
            var actionResult = await _storageController.Delete(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteStoragePage_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            Storage? storage = null;

            _mockStorageRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(storage));

            //act
            var actionResult = await _storageController.Delete(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteStoragePage_ShouldPass_WhenPassingValidId()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_storage));

            //act
            var actionResult = await _storageController.Delete(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteStorage_ShouldPass_WhenThereAreNoStorages()
        {
            //arrange
            List<Storage> storages = null;

            _mockStorageRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(storages));

            //act
            var actionResult = await _storageController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task DeleteStorage_ShouldPass_WhenIdIsValid()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_storages));

            //act
            var actionResult = await _storageController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task StorageExists_ShoudPass_WhenStorageExists()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _storageController.StorageExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(true));
        }

        [Test]
        public async Task StorageExists_ShoudPass_WhenStorageDoesntExist()
        {
            //arrange
            _mockStorageRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _storageController.StorageExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(false));
        }
    }
}
