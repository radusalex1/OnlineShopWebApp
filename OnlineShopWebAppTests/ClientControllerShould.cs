using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    internal class ClientControllerShould
    {
        private ClientsController _clientController;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IGenderRepository> _mockGenderRepository;

        [SetUp]
        public void Setup()
        {
            _mockClientRepository = new Mock<IClientRepository>();
            _mockGenderRepository = new Mock<IGenderRepository>();
            _clientController = new ClientsController(_mockClientRepository.Object, _mockGenderRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            //_clientController = null;
            _mockClientRepository.Reset();
            _mockGenderRepository.Reset();
        }

        [Test]
        public async Task GetClientsTest()
        {
            //arrange
            List<Client> clients = new()
            {
            new Client() {
               Id =1,
               Name = "Radu",
               Street = "Street",
               City ="Brasov",
               Country ="Romania",
               PhoneNumber = "0725342567",
               GenderId = 1
            }};

            _mockClientRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(clients));

            //act
            var actionResult = await _clientController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }
    }
}
