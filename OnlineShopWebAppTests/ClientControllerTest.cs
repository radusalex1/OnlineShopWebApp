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
    public class ClientControllerTests
    {
        private Client _client;
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
                }
            };

            _genders = new()
            {
                new Gender() { Id = 1, GenderType = "Male"},
                new Gender() { Id = 2, GenderType ="Female"}
            };

            _client = _clients[0];

            _mockClientRepository = new Mock<IClientRepository>();
            _mockGenderRepository = new Mock<IGenderRepository>();
            _clientController = new ClientsController(_mockClientRepository.Object, _mockGenderRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockClientRepository.Reset();
            _mockGenderRepository.Reset();
        }

        [Test]
        public async Task GetClients_ShouldPass_WhenCallingIndexMethod()
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
        public async Task GetClient_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            Client? client = null;

            _mockClientRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Details(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetClient_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int>())).Returns(Task.FromResult(_client));

            //act
            var actionResult = await _clientController.Details(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task GetClient_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _clientController.Details(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task CreateClient_ShouldPass_WhenCreatingValidClient()
        {
            //arrange
            _mockClientRepository.Setup(m => m.Add(It.IsAny<Client>())).Returns(Task.FromResult(true));
            _mockClientRepository.Setup(n => n.IfExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _clientController.Create(_client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task CreateClient_ShouldPass_WhenCreatingInvalidClient()
        {
            //arrange
            _mockClientRepository.Setup(m => m.Add(It.IsAny<Client>())).Returns(Task.FromResult(false));

            _mockClientRepository.Setup(n => n.IfExists(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockGenderRepository.Setup(n => n.Get(It.IsAny<int>())).Returns(Task.FromResult(_genders[0]));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));


            //act
            var actionResult = await _clientController.Create(_client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditClientPage_ShouldPass_WhenCallingByNullId()
        {
            //act
            var actionResult = await _clientController.Edit(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task EditClientPage_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            Client? client = null;

            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Edit(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditClientPage_ShouldPass_WhenCallingByValidId()
        {
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_client));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));

            //act
            var actionResult = await _clientController.Edit(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task EditClientPost_ShouldPass_WhenCallingByInvalidId()
        {
            //act
            var actionResult = await _clientController.Edit(2, _client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task EditClientPost_ShouldPass_WhenClientNotFound()
        {
            //arrange
            _client.Id = 2;

            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Throws<DbUpdateConcurrencyException>();
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _clientController.Edit(2, _client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void EditClientPost_ShouldPass_WhenThrowException()
        {
            //arrange
            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Throws<DbUpdateConcurrencyException>();
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //assert
            Assert.That(async () => await _clientController.Edit(1, _client), Throws.TypeOf<DbUpdateConcurrencyException>());
        }

        [Test]
        public async Task EditClientPost_ShouldPass_WhenEditValidClient()
        {
            //arrange
            _mockClientRepository.Setup(m => m.Update(It.IsAny<Client>())).Returns(Task.FromResult(true));
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _clientController.Edit(1, _client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<RedirectToActionResult>());
        }

        [Test]
        public async Task EditClientPost_ShouldPass_WhenCallingWithInvalidClient()
        {
            //arrange
            _client.Id = 2;

            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(false));
            _mockGenderRepository.Setup(n => n.GetAll()).Returns(Task.FromResult(_genders));

            //act
            var actionResult = await _clientController.Edit(2, _client);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenPassingNullId()
        {
            //act
            var actionResult = await _clientController.Delete(null);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenObjectNotFound()
        {
            //arrange
            Client? client = null;

            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(client));

            //act
            var actionResult = await _clientController.Delete(2);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteClientPage_ShouldPass_WhenPassingValidId()
        {
            //arrange
            _mockClientRepository.Setup(m => m.Get(It.IsAny<int?>())).Returns(Task.FromResult(_client));

            //act
            var actionResult = await _clientController.Delete(1);

            //assert
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task DeleteClient_ShouldPass_WhenThereAreNoClients()
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
        public async Task DeleteClient_ShouldPass_WhenIdIsValid()
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
        public async Task ClientExists_ShoudPass_WhenClientExists()
        {
            //arrange
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<int>())).Returns(Task.FromResult(true));

            //act
            var actionResult = await _clientController.ClientExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(true));
        }

        [Test]
        public async Task ClientExists_ShoudPass_WhenClientDoesntExist()
        {
            //arrange
            _mockClientRepository.Setup(m => m.IfExists(It.IsAny<string>())).Returns(Task.FromResult(false));

            //act
            var actionResult = await _clientController.ClientExists(1);

            //assert
            Assert.That(actionResult, Is.EqualTo(false));
        }
    }
}
