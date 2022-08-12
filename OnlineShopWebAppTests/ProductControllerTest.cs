using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    internal class ProductControllerTest
    {
        private ProductsController _productsController;

        private Mock<IProductRepository> _mockProductRepository;

        [SetUp]
        public void Setup()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _productsController = new ProductsController(_mockProductRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockProductRepository.Reset();
        }

        [Test]
        public async Task GetProductsTest_ShouldPass_WhenIndexMethodIsCalled()
        {
            //arrage
            List<Product> products = new()
            {
                new Product()
                {
                    Id=1,
                    Name="Bicicleta",
                    ExpirationDate=DateTime.Now,
                    Description=string.Empty,
                    Price=2000
                }
            };


            _mockProductRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(products));

            //act
            var actionResult = await _productsController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task CreateProduct_ShouldPass_WhenCreatingValidProduct()
        {
            //arrange
            var product = new Product()
            {
                Id = 1,
                Name = null,
                Description = null,
                Price = -1
            };

            //act
            var result = await _productsController.Create(product);

            //assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }



    }
}
