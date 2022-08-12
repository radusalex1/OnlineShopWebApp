﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using OnlineShopWebApp.Controllers;
using OnlineShopWebApp.DataModels;
using OnlineShopWebApp.Repositories;

namespace OnlineShopWebAppTests
{
    [TestFixture]
    internal class ClientControllerTests
    {
        private List<Client> _clients;
        private List<Gender> _genders;
        private ClientsController _clientController;
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IGenderRepository> _mockGenderRepository;

        [SetUp]
        public void Setup()
        {
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
            }};

            _genders = new()
            {
            new Gender() { Id = 1, GenderType = "Male"},
            new Gender() { Id = 2, GenderType ="Female"}
            };

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
        public async Task GetClientsShould()
        {
            //arrange
            _mockClientRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_clients));

            //act
            var actionResult = await _clientController.Index();

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetClientByInvalidIdShould()
        {
            Client? client = null;

            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Details(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetClientByIdShould()
        {
            Client? client = _clients[0];
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Details(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetClientByNullIdShould()
        {
            //act
            var actionResult = await _clientController.Details(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateClientShould()
        {
            //arrange
            Client client = _clients[0];

            _mockClientRepository.Setup(m => m.Add(It.IsAny<Client>())).Returns(Task.FromResult(true));
            _mockClientRepository.Setup(n => n.IfExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _clientController.Create(client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CreateInvalidClientShould()
        {
            //arrange
            Client client = _clients[0];

            _mockClientRepository.Setup(m => m.Add(It.IsAny<Client>())).Returns(Task.FromResult(false));
            _mockClientRepository.Setup(n => n.IfExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockGenderRepository.Setup(n => n.Get(It.IsAny<int>())).Returns(Task.FromResult(_genders[0]));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));

            //act
            var actionResult = await _clientController.Create(client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task OpenEditPageWithNullIdShould()
        {
            //act
            var actionResult = await _clientController.Edit(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task OpenEditPageWithInvalidIdShould()
        {
            Client? client = null;
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Edit(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task OpenEditPageWithValidIdShould()
        {
            Client? client = _clients[0];
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));

            //act
            var actionResult = await _clientController.Edit(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditWithInvalidId()
        {
            Client? client = _clients[0];

            //act
            var actionResult = await _clientController.Edit(2, client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditNonExistingClient()
        {
            //arrange
            Client? client = _clients[0];
            client.Id = 2;

            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Throws<DbUpdateConcurrencyException>();

            //act
            var actionResult = await _clientController.Edit(2, client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void EditThrowsException()
        {
            //arrange
            Client? client = _clients[0];

            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Throws<DbUpdateConcurrencyException>();
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(true);

            //assert
            Assert.That(async () => await _clientController.Edit(1, client), Throws.TypeOf<DbUpdateConcurrencyException>());
        }

        [Test]
        public async Task EditClient()
        {
            //arrange
            Client? client = _clients[0];

            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _clientController.Edit(1, client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        //[Test]
        //public async Task EditInvalidClient()
        //{
        //    ////arrange
        //    //Client? client = _clients[0];
        //    //client.Name = null;

        //    //_mockClientRepository.Setup(mode

        //    ////act
        //    //var actionResult = await _clientController.Edit(1, client);

        //    ////assert
        //    //Assert.That(actionResult, Is.Not.Null);
        //    //Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        //}

        [Test]
        public async Task OpenDeletePageWithNullId()
        {
            //act
            var actionResult = await _clientController.Delete(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task OpenDeletePageWithInvalidId()
        {
            Client? client = null;

            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Delete(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task OpenDeletePageWith()
        {
            //arrange
            Client? client = _clients[0];

            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Delete(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteWhenContextIsNull()
        {
            //arrange
            List<Client> clients = null;

            _mockClientRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(clients));

            //act
            var actionResult = await _clientController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
        }

        [Test]
        public async Task DeleteClient()
        {
            //arrange

            _mockClientRepository.Setup(m => m.GetAll()).Returns(Task.FromResult(_clients));

            //act
            var actionResult = await _clientController.DeleteConfirmed(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public void ClientExists()
        {
            //arrange
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(true);

            //act
            var actionResult = _clientController.ClientExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(true));
        }

        [Test]
        public void ClientDoesntExists()
        {
            //arrange
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            //act
            var actionResult = _clientController.ClientExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(false));
        }
    }
}