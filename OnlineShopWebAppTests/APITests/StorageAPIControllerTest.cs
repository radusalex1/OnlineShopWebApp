using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers.APIControllers;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests.APITests
{
    [TestFixture]
    public class StorageAPIControllerTest
    {
        private StorageControllerApi _storageControllerApi;
        private Mock<IStorageRepository> _mockStorageRepository;

        [SetUp]
        public void Setup()
        {
            _mockStorageRepository = new Mock<IStorageRepository>();
            _storageControllerApi = new StorageControllerApi(_mockStorageRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockStorageRepository.Reset();
        }

        [Test]
        public async Task CheckAvailability_ShouldPass_WhenProductIdIsInvalid()
        {
            //act
            var actionResult = await _storageControllerApi.CheckAvailability(-1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CheckAvailability_ShouldPass_WhenProductIsNotInStock()
        {
            //act
            var actionResult = await _storageControllerApi.CheckAvailability(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CheckAvailability_ShouldPass_WhenProductIsInStock()
        {
            //arrange
            _mockStorageRepository.Setup(n => n.GetQuantityByProductId(It.IsAny<int>())).Returns(Task.FromResult(30));

            //act
            var actionResult = await _storageControllerApi.CheckAvailability(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
        }
    }
}
